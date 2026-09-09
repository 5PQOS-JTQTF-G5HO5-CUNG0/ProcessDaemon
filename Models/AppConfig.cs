using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProcessDaemon.Models;

/// <summary>
/// 应用程序配置模型
/// </summary>
public class AppConfig
{
    /// <summary>
    /// 目标可执行文件路径
    /// </summary>
    [JsonPropertyName("targetPath")]
    public string TargetPath { get; set; } = string.Empty;

    /// <summary>
    /// 每日自动关闭时间（格式：HH:mm:ss）
    /// </summary>
    [JsonPropertyName("stopTime")]
    public string StopTime { get; set; } = "04:00:00";

    /// <summary>
    /// 每日自动启动时间（格式：HH:mm:ss）
    /// </summary>
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = "04:05:00";

    /// <summary>
    /// 手动重启时的安全缓冲倒计时（秒）
    /// </summary>
    [JsonPropertyName("manualRestartBufferSeconds")]
    public int ManualRestartBufferSeconds { get; set; } = 5;

    /// <summary>
    /// 是否强杀整个子进程树（推荐 true，防止孤儿进程残留）
    /// </summary>
    [JsonPropertyName("killEntireProcessTree")]
    public bool KillEntireProcessTree { get; set; } = true;

    /// <summary>
    /// 目标非计划内异常退出时，是否自动尝试拉起自愈
    /// </summary>
    [JsonPropertyName("autoRecoverOnCrash")]
    public bool AutoRecoverOnCrash { get; set; } = true;

    /// <summary>
    /// 守护工具启动时是否自动开始监控
    /// </summary>
    [JsonPropertyName("autoStartDaemon")]
    public bool AutoStartDaemon { get; set; } = true;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 从文件加载配置，若文件不存在则返回默认配置
    /// </summary>
    public static AppConfig Load(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var config = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
                if (config != null)
                {
                    return config;
                }
            }
        }
        catch
        {
            // 读取异常时使用默认配置
        }

        return new AppConfig();
    }

    /// <summary>
    /// 保存配置到指定路径
    /// </summary>
    public void Save(string filePath)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}
