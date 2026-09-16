using System.Security.Cryptography;
using System.Text.Json;

namespace ProcessDaemon.Licensing;

/// <summary>
/// 许可证与离线授权管理器
/// </summary>
public static class LicenseManager
{
    /// <summary>
    /// 内置 ECDSA (NIST P-256) 验签公钥 (由 tools/Keygen 生成)
    /// </summary>
    public const string PublicKeyPem = """
-----BEGIN PUBLIC KEY-----
MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEhTFSkFI0FPAqTJwNie5cowlHPqoT
j6UA+PTVYGGLVUjxCqgVIIMegRwFTuDHapAYOjq72dXLxrCZRjgYuXuRCw==
-----END PUBLIC KEY-----
""";

    /// <summary>
    /// 客户端绑定的应用程序唯一标识 (用于跨软件授权隔离)
    /// </summary>
    public const string TargetAppId = "ProcessDaemon";

    private const string LicenseFileName = "license.lic";
    private static LicenseValidationResult? _lastValidationResult;

    /// <summary>
    /// 获取用户数据目录下的许可证文件路径 (%LOCALAPPDATA%\ProcessDaemon\license.lic)
    /// </summary>
    public static string UserLicenseFilePath
    {
        get
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appData, "ProcessDaemon", LicenseFileName);
        }
    }

    /// <summary>
    /// 获取程序根目录下的许可证文件路径 (便携模式)
    /// </summary>
    public static string AppBaseLicenseFilePath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LicenseFileName);

    /// <summary>
    /// 解析并获取有效的许可证文件路径 (优先已存在的文件)
    /// </summary>
    public static string ResolveLicenseFilePath()
    {
        if (File.Exists(AppBaseLicenseFilePath))
        {
            return AppBaseLicenseFilePath;
        }

        return UserLicenseFilePath;
    }

    /// <summary>
    /// 读取本地存储的激活码 (若不存在则返回 null)
    /// </summary>
    public static string? ReadStoredLicenseCode()
    {
        try
        {
            if (File.Exists(AppBaseLicenseFilePath))
            {
                string code = File.ReadAllText(AppBaseLicenseFilePath).Trim();
                if (!string.IsNullOrWhiteSpace(code)) return code;
            }

            if (File.Exists(UserLicenseFilePath))
            {
                string code = File.ReadAllText(UserLicenseFilePath).Trim();
                if (!string.IsNullOrWhiteSpace(code)) return code;
            }
        }
        catch { }

        return null;
    }

    /// <summary>
    /// 验证指定激活码或本地已存储的激活凭证
    /// </summary>
    /// <param name="rawCode">待验证的激活码字符串，若为 null 则验证本地凭证</param>
    /// <param name="forceRefresh">是否强制重新验签而不使用上次缓存</param>
    public static LicenseValidationResult ValidateLicense(string? rawCode = null, bool forceRefresh = false)
    {
        if (rawCode == null && !forceRefresh && _lastValidationResult != null)
        {
            // 如果上一次验证结果已过期，则重新校验，否则返回缓存
            if (_lastValidationResult.IsValid && !_lastValidationResult.IsPermanent)
            {
                if (_lastValidationResult.Payload != null && _lastValidationResult.Payload.ExpireAt < DateTime.UtcNow)
                {
                    // 已过期，重新走全量校验流程
                }
                else
                {
                    return _lastValidationResult;
                }
            }
            else
            {
                return _lastValidationResult;
            }
        }

        string code = rawCode ?? ReadStoredLicenseCode() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            var res = LicenseValidationResult.Failure(LicenseStatus.NotActivated, "尚未激活授权，请输入激活码完成激活。");
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        string[] parts = code.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            var res = LicenseValidationResult.Failure(LicenseStatus.InvalidFormat, "激活码格式无效 (需包含载荷与有效数字签名)。");
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        byte[] payloadBytes;
        byte[] signatureBytes;

        try
        {
            payloadBytes = Convert.FromBase64String(parts[0]);
            signatureBytes = Convert.FromBase64String(parts[1]);
        }
        catch
        {
            var res = LicenseValidationResult.Failure(LicenseStatus.InvalidFormat, "激活码解码失败，内容已损坏。");
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        // 1. ECDSA P-256 非对称验签
        try
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(PublicKeyPem);
            bool isSignatureValid = ecdsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256);
            if (!isSignatureValid)
            {
                var res = LicenseValidationResult.Failure(LicenseStatus.InvalidSignature, "数字签名校验失败！激活码非官方签发或已被篡改。");
                if (rawCode == null) _lastValidationResult = res;
                return res;
            }
        }
        catch (Exception ex)
        {
            var res = LicenseValidationResult.Failure(LicenseStatus.InvalidSignature, $"密码学校验异常: {ex.Message}");
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        // 2. 解析载荷 JSON
        LicensePayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<LicensePayload>(payloadBytes);
        }
        catch (Exception ex)
        {
            var res = LicenseValidationResult.Failure(LicenseStatus.Corrupted, $"授权载荷反序列化失败: {ex.Message}");
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        if (payload == null)
        {
            var res = LicenseValidationResult.Failure(LicenseStatus.Corrupted, "授权载荷为空。");
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        // 3. 校验软件标识 (AppId)
        if (!string.Equals(payload.AppId, TargetAppId, StringComparison.OrdinalIgnoreCase))
        {
            var res = LicenseValidationResult.Failure(
                LicenseStatus.AppIdMismatch,
                $"该激活码适用于软件 [{payload.AppId}]，无法用于当前程序 [{TargetAppId}]。",
                payload
            );
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        // 4. 校验绑定机器码 (MachineCode)
        string currentMachineCode = MachineFingerprint.GetMachineCode();
        if (!string.Equals(payload.MachineCode, currentMachineCode, StringComparison.OrdinalIgnoreCase))
        {
            var res = LicenseValidationResult.Failure(
                LicenseStatus.MachineMismatch,
                $"此激活码绑定的设备 ({payload.MachineCode}) 与本机机器码 ({currentMachineCode}) 不一致！",
                payload
            );
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        // 5. 校验授权有效期 (ExpireAt)
        if (payload.ExpireAt < DateTime.UtcNow)
        {
            var res = LicenseValidationResult.Failure(
                LicenseStatus.Expired,
                $"授权已于 {payload.ExpireAt.ToLocalTime():yyyy-MM-dd HH:mm:ss} 过期，请联系管理员续签。",
                payload
            );
            if (rawCode == null) _lastValidationResult = res;
            return res;
        }

        // 全部校验通过
        var successResult = LicenseValidationResult.Success(payload);
        if (rawCode == null) _lastValidationResult = successResult;
        return successResult;
    }

    /// <summary>
    /// 应用并持久化新激活码
    /// </summary>
    public static LicenseValidationResult ApplyActivationCode(string rawCode)
    {
        string cleanCode = rawCode.Trim();
        var validation = ValidateLicense(cleanCode, forceRefresh: true);
        if (!validation.IsValid)
        {
            return validation;
        }

        // 写入存储
        SaveLicenseFile(cleanCode);
        _lastValidationResult = validation;

        return validation;
    }

    /// <summary>
    /// 持久化激活凭据文件
    /// </summary>
    private static void SaveLicenseFile(string code)
    {
        bool saved = false;

        // 优先保存到程序目录 (如果是便携版且有写入权限)
        try
        {
            File.WriteAllText(AppBaseLicenseFilePath, code);
            saved = true;
        }
        catch { }

        // 写入 LocalApplicationData 作为稳固备份
        try
        {
            string dir = Path.GetDirectoryName(UserLicenseFilePath)!;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(UserLicenseFilePath, code);
            saved = true;
        }
        catch (Exception ex)
        {
            if (!saved) throw new IOException($"无法保存授权凭据文件: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 清除本地授权凭据
    /// </summary>
    public static void RemoveStoredLicense()
    {
        try
        {
            if (File.Exists(AppBaseLicenseFilePath)) File.Delete(AppBaseLicenseFilePath);
        }
        catch { }

        try
        {
            if (File.Exists(UserLicenseFilePath)) File.Delete(UserLicenseFilePath);
        }
        catch { }

        _lastValidationResult = null;
    }
}
