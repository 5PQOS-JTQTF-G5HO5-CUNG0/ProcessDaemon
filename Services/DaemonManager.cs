using System.Diagnostics;
using ProcessDaemon.Models;

namespace ProcessDaemon.Services;

/// <summary>
/// 核心守护调度管理服务
/// </summary>
public class DaemonManager : IDisposable
{
    private readonly AppConfig _config;
    private System.Threading.Timer? _pollTimer;
    private readonly object _syncLock = new();

    private Process? _trackedProcess;
    private int? _trackedPid;
    private DaemonStatus _status = DaemonStatus.Stopped;
    private bool _isIntentionalKill;
    private bool _isRestarting;

    // 防止同秒重复触发记录
    private string _lastStopTriggerStamp = string.Empty;
    private string _lastStartTriggerStamp = string.Empty;

    public DaemonStatus Status => _status;
    public int? TrackedPid => _trackedPid;

    public event Action<DaemonStatus, string>? StatusChanged;
    public event Action<string, string>? LogMessageReceived; // (message, level)
    public event Action<int>? RestartCountdownChanged;
    public event Action<int?>? TrackedPidChanged;

    public DaemonManager(AppConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// 启动后台轮询守护
    /// </summary>
    public void StartDaemon()
    {
        lock (_syncLock)
        {
            if (_pollTimer != null) return;

            Log("守护引擎已启动，秒级调度轮询就绪", "INFO");

            // 若配置了目标程序，且当前未运行，尝试接管或初次启动
            if (File.Exists(_config.TargetPath))
            {
                // 先尝试检测系统现有同路径进程接管
                if (!TryAttachExistingProcess())
                {
                    Log("未检测到现有存活实例，准备初次启动目标程序...", "INFO");
                    LaunchTargetProcess();
                }
            }
            else
            {
                Log($"目标程序文件不存在: {_config.TargetPath}，请在设置中配置有效路径", "WARN");
                SetStatus(DaemonStatus.Stopped, "目标程序路径无效");
            }

            // 500ms 间隔轮询，确保不会因为时钟跳变漏掉整秒命中
            _pollTimer = new System.Threading.Timer(OnTimerTick, null, 0, 500);
        }
    }

    /// <summary>
    /// 停止守护（不杀进程，仅脱钩退出监控）
    /// </summary>
    public void StopDaemon()
    {
        lock (_syncLock)
        {
            _pollTimer?.Dispose();
            _pollTimer = null;

            if (_trackedProcess != null)
            {
                try
                {
                    _trackedProcess.EnableRaisingEvents = false;
                }
                catch { }
            }

            SetStatus(DaemonStatus.Stopped, "守护已手动停止");
            Log("守护引擎已停止监控", "INFO");
        }
    }

    /// <summary>
    /// 后台秒级心跳轮询
    /// </summary>
    private void OnTimerTick(object? state)
    {
        if (_isRestarting || _status == DaemonStatus.Stopped) return;

        var now = DateTime.Now;
        var currentTimeStr = now.ToString("HH:mm:ss");
        var todayStr = now.ToString("yyyy-MM-dd");

        // 1. 命中关闭时间
        if (currentTimeStr == _config.StopTime)
        {
            var triggerKey = $"{todayStr}_{_config.StopTime}";
            if (_lastStopTriggerStamp != triggerKey)
            {
                _lastStopTriggerStamp = triggerKey;
                Log($"命中每日计划关闭时间 [{_config.StopTime}]，开始执行关停...", "ACTION");
                HandleScheduledStop();
            }
        }

        // 2. 命中启动时间
        if (currentTimeStr == _config.StartTime)
        {
            var triggerKey = $"{todayStr}_{_config.StartTime}";
            if (_lastStartTriggerStamp != triggerKey)
            {
                _lastStartTriggerStamp = triggerKey;
                Log($"命中每日计划启动时间 [{_config.StartTime}]，开始拉起目标程序...", "ACTION");
                HandleScheduledStart();
            }
        }
    }

    /// <summary>
    /// 命中关闭时间的处理
    /// </summary>
    private void HandleScheduledStop()
    {
        lock (_syncLock)
        {
            _isIntentionalKill = true;
            KillTrackedProcess();
            SetStatus(DaemonStatus.WaitingStart, "已计划停止，等待启动时间");
        }
    }

    /// <summary>
    /// 命中启动时间的处理
    /// </summary>
    private void HandleScheduledStart()
    {
        lock (_syncLock)
        {
            LaunchTargetProcess();
        }
    }

    /// <summary>
    /// 手动突发立即重启
    /// </summary>
    public async Task ManualRestartAsync()
    {
        if (_isRestarting)
        {
            Log("正在重启中，请勿重复触发", "WARN");
            return;
        }

        _isRestarting = true;
        SetStatus(DaemonStatus.Restarting, "手动重启中");
        Log("收到手动突发重启指令，正在强杀目标进程...", "ACTION");

        await Task.Run(async () =>
        {
            try
            {
                lock (_syncLock)
                {
                    _isIntentionalKill = true;
                    KillTrackedProcess();
                }

                // 启动安全缓冲倒计时
                var bufferSeconds = Math.Max(1, _config.ManualRestartBufferSeconds);
                Log($"进入安全缓冲等待，倒计时 {bufferSeconds} 秒...", "INFO");

                for (int i = bufferSeconds; i > 0; i--)
                {
                    RestartCountdownChanged?.Invoke(i);
                    await Task.Delay(1000);
                }

                RestartCountdownChanged?.Invoke(0);
                Log("缓冲等待结束，正在重新拉起目标程序...", "ACTION");

                lock (_syncLock)
                {
                    LaunchTargetProcess();
                }
            }
            catch (Exception ex)
            {
                Log($"手动重启过程异常: {ex.Message}", "ERROR");
                SetStatus(DaemonStatus.Stopped, "重启失败");
            }
            finally
            {
                _isRestarting = false;
            }
        });
    }

    /// <summary>
    /// 拉起目标程序
    /// </summary>
    public bool LaunchTargetProcess()
    {
        if (string.IsNullOrWhiteSpace(_config.TargetPath) || !File.Exists(_config.TargetPath))
        {
            Log($"启动失败，目标程序不存在: {_config.TargetPath}", "ERROR");
            SetStatus(DaemonStatus.Stopped, "目标程序路径无效");
            return false;
        }

        try
        {
            var workDir = Path.GetDirectoryName(_config.TargetPath) ?? string.Empty;

            var startInfo = new ProcessStartInfo
            {
                FileName = _config.TargetPath,
                Arguments = string.Empty,
                WorkingDirectory = workDir,
                UseShellExecute = true
            };

            _isIntentionalKill = false;
            var proc = Process.Start(startInfo);
            if (proc != null)
            {
                BindTrackedProcess(proc);
                Log($"目标程序启动成功，PID: {proc.Id}", "SUCCESS");
                SetStatus(DaemonStatus.Running, "正常运行中");
                return true;
            }
            else
            {
                Log("启动失败，Process.Start 返回空实例", "ERROR");
                SetStatus(DaemonStatus.Stopped, "启动失败");
                return false;
            }
        }
        catch (Exception ex)
        {
            Log($"拉起目标程序抛出异常: {ex.Message}", "ERROR");
            SetStatus(DaemonStatus.Stopped, "启动异常");
            return false;
        }
    }

    /// <summary>
    /// 强杀当前跟踪的目标进程（全进程树可选）
    /// </summary>
    public void KillTrackedProcess()
    {
        if (_trackedPid == null)
        {
            Log("当前无跟踪记录的 PID", "INFO");
            return;
        }

        int pid = _trackedPid.Value;
        try
        {
            var proc = Process.GetProcessById(pid);
            if (!proc.HasExited)
            {
                Log($"正在强杀进程 [PID: {pid}] (全进程树: {_config.KillEntireProcessTree})...", "WARN");
                proc.Kill(entireProcessTree: _config.KillEntireProcessTree);
                proc.WaitForExit(3000); // 最长等待 3 秒释放
                Log($"进程 [PID: {pid}] 已彻底终止", "SUCCESS");
            }
        }
        catch (ArgumentException)
        {
            // 进程已不存在
            Log($"进程 [PID: {pid}] 已经退出", "INFO");
        }
        catch (Exception ex)
        {
            Log($"强杀进程 [PID: {pid}] 异常: {ex.Message}", "ERROR");
        }
        finally
        {
            UnbindTrackedProcess();
        }
    }

    /// <summary>
    /// 尝试接管系统中已在运行的目标实例
    /// </summary>
    private bool TryAttachExistingProcess()
    {
        try
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(_config.TargetPath);
            var processes = Process.GetProcessesByName(fileNameWithoutExt);
            foreach (var proc in processes)
            {
                try
                {
                    if (proc.MainModule?.FileName.Equals(_config.TargetPath, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        BindTrackedProcess(proc);
                        Log($"成功接管现有运行中的目标进程，PID: {proc.Id}", "SUCCESS");
                        SetStatus(DaemonStatus.Running, "已接管运行");
                        return true;
                    }
                }
                catch
                {
                    // 权限限制可能导致某些进程 MainModule 无法访问，直接尝试比较
                    BindTrackedProcess(proc);
                    Log($"接管同名现有运行实例，PID: {proc.Id}", "SUCCESS");
                    SetStatus(DaemonStatus.Running, "已接管运行");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Log($"尝试接管现有进程失败: {ex.Message}", "DEBUG");
        }

        return false;
    }

    private void BindTrackedProcess(Process proc)
    {
        _trackedProcess = proc;
        _trackedPid = proc.Id;
        TrackedPidChanged?.Invoke(_trackedPid);

        try
        {
            proc.EnableRaisingEvents = true;
            proc.Exited += OnProcessExited;
        }
        catch (Exception ex)
        {
            Log($"绑定进程退出事件警告: {ex.Message}", "DEBUG");
        }
    }

    private void UnbindTrackedProcess()
    {
        if (_trackedProcess != null)
        {
            try
            {
                _trackedProcess.Exited -= OnProcessExited;
                _trackedProcess.Dispose();
            }
            catch { }
            _trackedProcess = null;
        }

        _trackedPid = null;
        TrackedPidChanged?.Invoke(null);
    }

    /// <summary>
    /// 进程退出捕获事件
    /// </summary>
    private void OnProcessExited(object? sender, EventArgs e)
    {
        int? exitedPid = _trackedPid;
        UnbindTrackedProcess();

        // 如果是主动关停或正在手动重启，不当作异常崩溃
        if (_isIntentionalKill || _isRestarting)
        {
            return;
        }

        Log($"[警告] 目标程序 [PID: {exitedPid}] 非预期意外退出！", "WARN");

        // 崩溃自愈保护
        if (_config.AutoRecoverOnCrash && _pollTimer != null)
        {
            Log("触发异常崩溃自愈策略，3 秒后重新拉起目标程序...", "ACTION");
            Task.Delay(3000).ContinueWith(_ =>
            {
                lock (_syncLock)
                {
                    if (_status != DaemonStatus.Stopped && !_isRestarting)
                    {
                        LaunchTargetProcess();
                    }
                }
            });
        }
        else
        {
            SetStatus(DaemonStatus.Stopped, "进程已意外退出");
        }
    }

    private void SetStatus(DaemonStatus status, string detail)
    {
        _status = status;
        StatusChanged?.Invoke(status, detail);
    }

    private void Log(string message, string level)
    {
        LogMessageReceived?.Invoke(message, level);
    }

    public void Dispose()
    {
        StopDaemon();
    }
}
