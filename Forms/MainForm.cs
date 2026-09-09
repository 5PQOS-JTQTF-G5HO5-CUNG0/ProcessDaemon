using System.Diagnostics;
using ProcessDaemon.Models;
using ProcessDaemon.Services;

namespace ProcessDaemon.Forms;

public partial class MainForm : Form
{
    private readonly string _configFilePath;
    private AppConfig _config;
    private readonly DaemonManager _daemon;
    private bool _isExplicitExit = false;

    public MainForm()
    {
        InitializeComponent();

        // 默认与可执行文件同目录的 config.json
        _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        _config = AppConfig.Load(_configFilePath);

        // 加载程序与系统托盘专属图标
        var appIcon = LoadAppIcon();
        if (appIcon != null)
        {
            this.Icon = appIcon;
            notifyIcon.Icon = appIcon;
        }

        _daemon = new DaemonManager(_config);

        // 绑定事件
        BindEvents();

        // 初始化界面数值
        LoadConfigToUi();

        // 时钟更新
        uiClockTimer.Tick += (s, e) =>
        {
            lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
        };
        uiClockTimer.Start();

        // 自动启动监控
        this.Load += (s, e) =>
        {
            if (_config.AutoStartDaemon)
            {
                StartDaemonInternal();
            }
            else
            {
                UpdateStatusBadge(DaemonStatus.Stopped, "守护未启动 (等待手动开启)");
            }
        };
    }

    private void BindEvents()
    {
        // 守护引擎回调
        _daemon.StatusChanged += (status, detail) =>
        {
            this.BeginInvoke(() => UpdateStatusBadge(status, detail));
        };

        _daemon.TrackedPidChanged += pid =>
        {
            this.BeginInvoke(() =>
            {
                lblPid.Text = pid.HasValue ? $"托管 PID: {pid.Value}" : "托管 PID: -";
                UpdateTrayToolTip();
            });
        };

        _daemon.RestartCountdownChanged += remaining =>
        {
            this.BeginInvoke(() =>
            {
                if (remaining > 0)
                {
                    btnManualRestart.Enabled = false;
                    btnManualRestart.Text = $"重启中({remaining}s)...";
                    lblStatusText.Text = $"正在安全缓冲倒计时: {remaining} 秒";
                }
                else
                {
                    btnManualRestart.Enabled = true;
                    btnManualRestart.Text = "手动立即重启";
                }
            });
        };

        _daemon.LogMessageReceived += (msg, level) =>
        {
            this.BeginInvoke(() => AppendLog(msg, level));
        };

        // 窗体操作
        btnBrowseTarget.Click += BtnBrowseTarget_Click;
        btnSaveConfig.Click += BtnSaveConfig_Click;
        btnToggleDaemon.Click += BtnToggleDaemon_Click;
        btnManualRestart.Click += async (s, e) => await _daemon.ManualRestartAsync();
        btnClearLogs.Click += (s, e) => rtbLogs.Clear();

        // 托盘菜单
        menuShow.Click += (s, e) => RestoreWindow();
        menuRestart.Click += async (s, e) => await _daemon.ManualRestartAsync();
        menuExit.Click += (s, e) =>
        {
            _isExplicitExit = true;
            this.Close();
        };

        // 托盘双击
        notifyIcon.DoubleClick += (s, e) => RestoreWindow();

        // 窗口拦截关闭 -> 驻留托盘
        this.FormClosing += MainForm_FormClosing;
    }

    private void LoadConfigToUi()
    {
        txtTargetPath.Text = _config.TargetPath;

        if (TimeSpan.TryParse(_config.StopTime, out var stopTime))
        {
            dtpStopTime.Value = DateTime.Today.Add(stopTime);
        }

        if (TimeSpan.TryParse(_config.StartTime, out var startTime))
        {
            dtpStartTime.Value = DateTime.Today.Add(startTime);
        }

        numBufferSeconds.Value = Math.Clamp(_config.ManualRestartBufferSeconds, 1, 300);
        chkKillTree.Checked = _config.KillEntireProcessTree;
        chkAutoRecover.Checked = _config.AutoRecoverOnCrash;
        chkAutoStart.Checked = _config.AutoStartDaemon;
    }

    private void SaveUiToConfig()
    {
        _config.TargetPath = txtTargetPath.Text.Trim();
        _config.StopTime = dtpStopTime.Value.ToString("HH:mm:ss");
        _config.StartTime = dtpStartTime.Value.ToString("HH:mm:ss");
        _config.ManualRestartBufferSeconds = (int)numBufferSeconds.Value;
        _config.KillEntireProcessTree = chkKillTree.Checked;
        _config.AutoRecoverOnCrash = chkAutoRecover.Checked;
        _config.AutoStartDaemon = chkAutoStart.Checked;

        _config.Save(_configFilePath);
        AppendLog("配置已保存并写入 config.json", "SUCCESS");
    }

    private void BtnBrowseTarget_Click(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Title = "选择目标程序可执行文件",
            Filter = "应用程序 (*.exe;*.bat;*.cmd)|*.exe;*.bat;*.cmd|所有文件 (*.*)|*.*",
            Multiselect = false
        };

        if (ofd.ShowDialog(this) == DialogResult.OK)
        {
            txtTargetPath.Text = ofd.FileName;
        }
    }

    private void BtnSaveConfig_Click(object? sender, EventArgs e)
    {
        SaveUiToConfig();
    }

    private void BtnToggleDaemon_Click(object? sender, EventArgs e)
    {
        if (_daemon.Status == DaemonStatus.Stopped)
        {
            SaveUiToConfig();
            StartDaemonInternal();
        }
        else
        {
            _daemon.StopDaemon();
            btnToggleDaemon.Text = "启动守护";
            btnToggleDaemon.BackColor = Color.FromArgb(235, 255, 235);
        }
    }

    private void StartDaemonInternal()
    {
        _daemon.StartDaemon();
        btnToggleDaemon.Text = "停止守护";
        btnToggleDaemon.BackColor = Color.FromArgb(255, 235, 235);
    }

    private void UpdateStatusBadge(DaemonStatus status, string detail)
    {
        lblStatusText.Text = detail;

        switch (status)
        {
            case DaemonStatus.Running:
                lblStatusBadge.BackColor = Color.FromArgb(40, 167, 69); // 绿色
                btnToggleDaemon.Text = "停止守护";
                btnToggleDaemon.BackColor = Color.FromArgb(255, 235, 235);
                break;
            case DaemonStatus.WaitingStart:
                lblStatusBadge.BackColor = Color.FromArgb(255, 193, 7); // 黄色
                btnToggleDaemon.Text = "停止守护";
                btnToggleDaemon.BackColor = Color.FromArgb(255, 235, 235);
                break;
            case DaemonStatus.Restarting:
                lblStatusBadge.BackColor = Color.FromArgb(23, 162, 184); // 青色
                btnToggleDaemon.Text = "停止守护";
                btnToggleDaemon.BackColor = Color.FromArgb(255, 235, 235);
                break;
            case DaemonStatus.Stopped:
            default:
                lblStatusBadge.BackColor = Color.Gray;
                btnToggleDaemon.Text = "启动守护";
                btnToggleDaemon.BackColor = Color.FromArgb(235, 255, 235);
                break;
        }

        UpdateTrayToolTip();
    }

    private void UpdateTrayToolTip()
    {
        var pidStr = _daemon.TrackedPid.HasValue ? $"PID: {_daemon.TrackedPid.Value}" : "无进程";
        var toolTip = $"进程守护器 [{_daemon.Status}] - {pidStr}";
        if (toolTip.Length > 63) toolTip = toolTip.Substring(0, 63); // 托盘文本长度上限限制
        notifyIcon.Text = toolTip;
    }

    private void AppendLog(string message, string level)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var color = level switch
        {
            "ERROR" => Color.FromArgb(255, 99, 71),
            "WARN" => Color.FromArgb(255, 165, 0),
            "SUCCESS" => Color.FromArgb(144, 238, 144),
            "ACTION" => Color.FromArgb(173, 216, 230),
            _ => Color.FromArgb(220, 220, 220)
        };

        rtbLogs.SelectionStart = rtbLogs.TextLength;
        rtbLogs.SelectionLength = 0;
        rtbLogs.SelectionColor = Color.Gray;
        rtbLogs.AppendText($"[{timestamp}] ");

        rtbLogs.SelectionColor = color;
        rtbLogs.AppendText($"[{level.PadRight(7)}] {message}\r\n");

        rtbLogs.ScrollToCaret();
    }

    private void RestoreWindow()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.BringToFront();
        this.Activate();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_isExplicitExit && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.Hide();
            notifyIcon.ShowBalloonTip(2000, "守护后台运行", "程序已最小化至系统托盘，双击图标可恢复窗口。", ToolTipIcon.Info);
        }
        else
        {
            notifyIcon.Visible = false;
            _daemon.Dispose();
            uiClockTimer.Stop();
        }
    }

    private static Icon? LoadAppIcon()
    {
        try
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream("ProcessDaemon.app.ico");
            if (stream != null)
            {
                return new Icon(stream);
            }
        }
        catch { }

        try
        {
            var localIco = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
            if (File.Exists(localIco))
            {
                return new Icon(localIco);
            }
        }
        catch { }

        try
        {
            var mainModule = Process.GetCurrentProcess().MainModule?.FileName;
            if (!string.IsNullOrEmpty(mainModule) && File.Exists(mainModule))
            {
                var icon = Icon.ExtractAssociatedIcon(mainModule);
                if (icon != null) return icon;
            }
        }
        catch { }

        return SystemIcons.Application;
    }
}
