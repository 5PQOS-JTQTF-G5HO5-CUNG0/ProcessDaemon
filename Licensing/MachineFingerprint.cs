using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace ProcessDaemon.Licensing;

/// <summary>
/// 机器硬件特征码提供程序 (一机一码指纹提取)
/// </summary>
public static class MachineFingerprint
{
    private static string? _cachedMachineCode;

    /// <summary>
    /// 获取当前计算机的唯一机器码 (格式: XXXX-XXXX-XXXX-XXXX)
    /// </summary>
    public static string GetMachineCode()
    {
        if (!string.IsNullOrEmpty(_cachedMachineCode))
        {
            return _cachedMachineCode;
        }

        var sb = new StringBuilder();

        // 1. Windows MachineGuid (系统安装唯一标识，标准用户权限即可只读访问)
        string machineGuid = ReadMachineGuid();
        sb.Append("MG:").Append(machineGuid).Append(';');

        // 2. 处理器特征标识
        string cpuId = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") 
            ?? Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") 
            ?? "GENERIC_CPU";
        sb.Append("CPU:").Append(cpuId).Append(';');

        // 3. 系统驱动器卷序列号
        string volumeSerial = GetSystemDriveVolumeSerial();
        sb.Append("VOL:").Append(volumeSerial).Append(';');

        // 4. 计算机名称 (作为辅助混合因子)
        sb.Append("PC:").Append(Environment.MachineName);

        // 计算复合 SHA256 哈希
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));

        // 取前 8 字节 (16 个十六进制字符)，格式化为 4x4 大写字符串
        string hex = Convert.ToHexString(hashBytes, 0, 8); // 16 字符
        _cachedMachineCode = $"{hex[0..4]}-{hex[4..8]}-{hex[8..12]}-{hex[12..16]}".ToUpperInvariant();

        return _cachedMachineCode;
    }

    private static string ReadMachineGuid()
    {
        try
        {
            using var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
            var val = key?.GetValue("MachineGuid")?.ToString();
            if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
        }
        catch { }

        try
        {
            using var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
            var val = key?.GetValue("MachineGuid")?.ToString();
            if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
        }
        catch { }

        return "UNKNOWN_GUID";
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetVolumeInformation(
        string rootPathName,
        StringBuilder? volumeNameBuffer,
        int volumeNameSize,
        out uint volumeSerialNumber,
        out uint maximumComponentLength,
        out uint fileSystemFlags,
        StringBuilder? fileSystemNameBuffer,
        int nFileSystemNameSize);

    private static string GetSystemDriveVolumeSerial()
    {
        try
        {
            string systemRoot = Path.GetPathRoot(Environment.SystemDirectory) ?? @"C:\";
            if (!systemRoot.EndsWith(Path.DirectorySeparatorChar))
            {
                systemRoot += Path.DirectorySeparatorChar;
            }

            if (GetVolumeInformation(systemRoot, null, 0, out uint serial, out _, out _, null, 0))
            {
                return serial.ToString("X8");
            }
        }
        catch { }

        return "SYS_VOL_FALLBACK";
    }
}
