using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcessDaemon.Forms;

namespace ProcessDaemon;

internal static class Program
{
    private const string MutexName = @"Global\ProcessDaemon_SingleInstance_Mutex_3F92C";

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_RESTORE = 9;

    [STAThread]
    static void Main()
    {
        // 全局单实例互斥体检测
        using var mutex = new Mutex(true, MutexName, out bool isOnlyInstance);

        if (!isOnlyInstance)
        {
            // 如果已存在实例，尝试唤醒已有窗口
            var current = Process.GetCurrentProcess();
            foreach (var proc in Process.GetProcessesByName(current.ProcessName))
            {
                if (proc.Id != current.Id && proc.MainWindowHandle != IntPtr.Zero)
                {
                    ShowWindow(proc.MainWindowHandle, SW_RESTORE);
                    SetForegroundWindow(proc.MainWindowHandle);
                    break;
                }
            }

            MessageBox.Show("程序已在运行中，请检查右下角系统托盘图标。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // 全局未捕获异常处理
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (sender, e) =>
        {
            MessageBox.Show($"线程未捕获异常: {e.Exception.Message}\n{e.Exception.StackTrace}", "程序错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"域未捕获异常: {ex.Message}\n{ex.StackTrace}", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        // 高 DPI 与界面样式初始化
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        }
        catch { }

        Application.Run(new MainForm());
    }
}
