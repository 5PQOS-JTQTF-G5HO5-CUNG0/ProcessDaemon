namespace ProcessDaemon.Models;

/// <summary>
/// 守护进程运行状态枚举
/// </summary>
public enum DaemonStatus
{
    /// <summary>
    /// 已停止监控
    /// </summary>
    Stopped,

    /// <summary>
    /// 目标程序正常运行中
    /// </summary>
    Running,

    /// <summary>
    /// 已按计划关闭目标程序，等待启动时间
    /// </summary>
    WaitingStart,

    /// <summary>
    /// 手动重启倒计时安全缓冲中
    /// </summary>
    Restarting
}
