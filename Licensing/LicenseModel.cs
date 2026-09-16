using System.Text.Json.Serialization;

namespace ProcessDaemon.Licensing;

/// <summary>
/// 许可证载荷数据结构 (与 Keygen 签发数据一致)
/// </summary>
public sealed class LicensePayload
{
    [JsonPropertyName("AppId")]
    public string AppId { get; set; } = string.Empty;

    [JsonPropertyName("MachineCode")]
    public string MachineCode { get; set; } = string.Empty;

    [JsonPropertyName("ExpireAt")]
    public DateTime ExpireAt { get; set; }

    [JsonPropertyName("IssuedAt")]
    public DateTime IssuedAt { get; set; }

    [JsonPropertyName("CustomerId")]
    public string? CustomerId { get; set; }
}

/// <summary>
/// 授权状态枚举
/// </summary>
public enum LicenseStatus
{
    /// <summary>授权有效</summary>
    Valid,
    /// <summary>未找到授权凭证</summary>
    NotActivated,
    /// <summary>激活码格式错误</summary>
    InvalidFormat,
    /// <summary>非对称数字签名校验失败 (数据被篡改或伪造)</summary>
    InvalidSignature,
    /// <summary>软件 AppId 不匹配</summary>
    AppIdMismatch,
    /// <summary>机器码不匹配 (非本机授权)</summary>
    MachineMismatch,
    /// <summary>授权已过期</summary>
    Expired,
    /// <summary>数据解析异常</summary>
    Corrupted
}

/// <summary>
/// 授权校验综合结果
/// </summary>
public sealed class LicenseValidationResult
{
    public bool IsValid => Status == LicenseStatus.Valid;

    public LicenseStatus Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public LicensePayload? Payload { get; init; }

    /// <summary>
    /// 是否为永久授权 (ExpireAt >= 9999-01-01 或等于 DateTime.MaxValue)
    /// </summary>
    public bool IsPermanent => Payload != null && Payload.ExpireAt.Year >= 3000;

    /// <summary>
    /// 授权剩余天数 (永久授权返回 int.MaxValue)
    /// </summary>
    public int DaysRemaining
    {
        get
        {
            if (Payload == null) return 0;
            if (IsPermanent) return int.MaxValue;
            var diff = (Payload.ExpireAt.ToUniversalTime() - DateTime.UtcNow).TotalDays;
            return Math.Max(0, (int)Math.Ceiling(diff));
        }
    }

    /// <summary>
    /// 格式化有效期文本描述
    /// </summary>
    public string ExpirationDisplay
    {
        get
        {
            if (Payload == null) return "未激活";
            if (IsPermanent) return "永久授权 (无期限)";
            return $"{Payload.ExpireAt.ToLocalTime():yyyy-MM-dd HH:mm:ss} (剩余 {DaysRemaining} 天)";
        }
    }

    public static LicenseValidationResult Success(LicensePayload payload) => new()
    {
        Status = LicenseStatus.Valid,
        Message = "授权有效",
        Payload = payload
    };

    public static LicenseValidationResult Failure(LicenseStatus status, string message, LicensePayload? payload = null) => new()
    {
        Status = status,
        Message = message,
        Payload = payload
    };
}
