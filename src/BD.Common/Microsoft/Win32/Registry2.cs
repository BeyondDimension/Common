// ReSharper disable once CheckNamespace
namespace Microsoft.Win32;

public static partial class Registry2
{
#pragma warning disable CA1416 // 验证平台兼容性

    public const RegistryView DefaultRegistryView = RegistryView.Default;

    /// <summary>
    /// 根据字符串，例如 HKCR/HKCU/HKLM/HKCC/HKPD 获取对应的 <see cref="RegistryHive"/>，如果找不到对应的则返回 <see langword="null"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RegistryHive? GetRegistryHive(string hKey) => hKey.ToUpperInvariant() switch
    {
        "HKCR" => RegistryHive.ClassesRoot,
        "HKCU" => RegistryHive.CurrentUser,
        "HKLM" => RegistryHive.LocalMachine,
        "HKCC" => RegistryHive.CurrentConfig,
        "HKPD" => RegistryHive.PerformanceData,
        _ => default,
    };
#pragma warning restore CA1416 // 验证平台兼容性

#if WINDOWS

    /// <summary>
    /// 根据字符串，例如 HKCR/HKCU/HKLM/HKCC/HKPD 获取对应的 <see cref="RegistryKey"/>，如果找不到对应的则返回 <see langword="null"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RegistryKey? GetRegistryKey(string hKey, RegistryView view = DefaultRegistryView)
    {
        var registryHive = GetRegistryHive(hKey);
        if (registryHive.HasValue)
        {
            return view switch
            {
                RegistryView.Default => registryHive.Value switch
                {
                    RegistryHive.ClassesRoot => Registry.ClassesRoot,
                    RegistryHive.CurrentUser => Registry.CurrentUser,
                    RegistryHive.LocalMachine => Registry.LocalMachine,
                    RegistryHive.Users => Registry.Users,
                    RegistryHive.PerformanceData => Registry.PerformanceData,
                    RegistryHive.CurrentConfig => Registry.CurrentConfig,
                    _ => RegistryKey.OpenBaseKey(registryHive.Value, RegistryView.Default),
                },
                _ => RegistryKey.OpenBaseKey(registryHive.Value, view),
            };
        }
        return null;
    }

    /// <summary>
    /// 将已编码的注册表键值分解为不同部分
    /// </summary>
    /// <param name="encodedPath">HKXX\\path:SubKey</param>
    /// <param name="view"></param>
    /// <returns></returns>
    public static (RegistryKey? rootKey, string? path, string? subKey) ExplodeRegistryKey(
        string encodedPath,
        RegistryView view = DefaultRegistryView)
    {
        var rootKey = GetRegistryKey(encodedPath[..4], view); // Get HKXX
        encodedPath = encodedPath[5..]; // Remove HKXX\\
        var path = encodedPath.Split(":")[0];
        var subKey = encodedPath.Split(":")[1];
        return (rootKey, path, subKey);
    }

    /// <summary>
    /// 读取注册表键值（需要 Special Path）
    /// </summary>
    /// <param name="encodedPath">HKXX\\path:SubKey</param>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? ReadRegistryKey(
        string encodedPath,
        RegistryView view = DefaultRegistryView)
    {
        var (rootKey, path, subKey) = ExplodeRegistryKey(encodedPath, view);

        if (rootKey == default || path == default || subKey == default)
            return default;

        try
        {
            return rootKey.OpenSubKey(path)?.GetValue(subKey);
        }
        catch (Exception e)
        {
            Log.Error(nameof(Registry2), e, "ReadRegistryKey failed");
            return null;
        }
    }

    /// <summary>
    /// 尝试读取注册表键值（需要 Special Path）
    /// </summary>
    /// <param name="encodedPath">HKXX\\path:SubKey</param>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadRegistryKey(string encodedPath, RegistryView view, [NotNullWhen(true)] out object? value)
    {
        value = ReadRegistryKey(encodedPath, view);
        return value != null;
    }

    /// <summary>
    /// 尝试读取注册表键值（需要 Special Path）
    /// </summary>
    /// <param name="encodedPath">HKXX\\path:SubKey</param>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadRegistryKey(string encodedPath, [NotNullWhen(true)] out object? value)
    {
        value = ReadRegistryKey(encodedPath);
        return value != null;
    }

    /// <summary>
    /// 设置注册表键值（需要 Special Path）
    /// </summary>
    /// <param name="encodedPath">HKXX\\path:subkey</param>
    /// <param name="value">Value, or empty to "clear"</param>
    [SupportedOSPlatform("windows")]
    public static bool SetRegistryKey(
        string encodedPath,
        RegistryView view,
        string? value = null)
    {
        var (rootKey, path, subKey) = ExplodeRegistryKey(encodedPath, view);

        if (rootKey == default || path == default || subKey == default)
            return default;

        try
        {
            using var key = rootKey.CreateSubKey(path);
            if (string.IsNullOrEmpty(value))
            {
                key?.SetValue(subKey, string.Empty);
            }
            else
            {
                if (value.StartsWith("(hex)"))
                {
                    value = value[6..];
                    var valueBytes = Convert2.FromHexString(value);
                    key?.SetValue(subKey, valueBytes);
                }
                else
                {
                    key?.SetValue(subKey, value);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Log.Error(nameof(Registry2), e, "SetRegistryKey failed");
            return false;
        }
    }

    /// <summary>
    /// 设置注册表键值（需要 Special Path）
    /// </summary>
    /// <param name="encodedPath">HKXX\\path:subkey</param>
    /// <param name="value">Value, or empty to "clear"</param>
    [SupportedOSPlatform("windows")]
    public static bool SetRegistryKey(
        string encodedPath,
        string? value = null) => SetRegistryKey(encodedPath, DefaultRegistryView, value);

    [SupportedOSPlatform("windows")]
    public static bool DeleteRegistryKey(string encodedPath, RegistryView view = DefaultRegistryView)
    {
        var (rootKey, path, subKey) = ExplodeRegistryKey(encodedPath, view);

        if (rootKey == default || path == default || subKey == default)
            return default;

        try
        {
            using var key = rootKey.OpenSubKey(path, true);
            key?.DeleteValue(subKey);
        }
        catch (Exception e)
        {
            Log.Error(nameof(Registry2), e, "DeleteRegistryKey failed");
            return false;
        }

        return true;
    }

#endif
}
