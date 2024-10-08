#if !NETSTANDARD
namespace Microsoft.Win32;

public static partial class Registry2
{
#pragma warning disable CA1416 // 验证平台兼容性
    /// <inheritdoc cref="RegistryView.Default"/>
    public const RegistryView DefaultRegistryView = RegistryView.Default;

    /// <summary>
    /// 根据字符串，例如 HKCR/HKCU/HKLM/HKCC/HKPD 获取对应的 <see cref="RegistryHive"/>，如果找不到对应的则返回 <see langword="null"/>
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RegistryHive? GetRegistryHive(string hKey)
    {
        if (hKey != null && hKey.Length >= 4)
        {
            if ((hKey[0] == 'h' || hKey[0] == 'H') && (hKey[1] == 'k' || hKey[1] == 'K'))
            {
                switch (hKey[2])
                {
                    case 'c':
                    case 'C':
                        if (hKey[3] == 'r' || hKey[3] == 'R')
                        {
                            return RegistryHive.ClassesRoot;
                        }
                        else if (hKey[3] == 'u' || hKey[3] == 'U')
                        {
                            return RegistryHive.CurrentUser;
                        }
                        else if (hKey[3] == 'c' || hKey[3] == 'C')
                        {
                            return RegistryHive.CurrentConfig;
                        }
                        break;
                    case 'l':
                    case 'L':
                        if (hKey[3] == 'm' || hKey[3] == 'M')
                        {
                            return RegistryHive.LocalMachine;
                        }
                        break;
                    case 'p':
                    case 'P':
                        if (hKey[3] == 'd' || hKey[3] == 'D')
                        {
                            return RegistryHive.PerformanceData;
                        }
                        break;
                }
            }
        }
        return default;
    }
#pragma warning restore CA1416 // 验证平台兼容性

#if WINDOWS
    /// <summary>
    /// 根据字符串，例如 HKCR/HKCU/HKLM/HKCC/HKPD 获取对应的 <see cref="RegistryKey"/>，如果找不到对应的则返回 <see langword="null"/>
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="view"></param>
    /// <returns></returns>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// <param name="view"></param>
    /// <returns></returns>
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
    /// <param name="view"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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
    /// <param name="value"></param>
    /// <returns></returns>
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
    /// <param name="view"></param>
    /// <param name="value">Value, or empty to "clear"</param>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SetRegistryKey(
        string encodedPath,
        string? value = null) => SetRegistryKey(encodedPath, DefaultRegistryView, value);

    /// <summary>
    /// 删除注册表值
    /// </summary>
    /// <param name="encodedPath"></param>
    /// <param name="view"></param>
    /// <returns></returns>
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

#if WINDOWS
partial class Registry2
{
#if DEBUG
    static readonly Lazy<string> _regedit_exe = new(() =>
    {
        var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var regedit_exe = Path.Combine(windir.ThrowIsNull(), "regedit.exe");
        return regedit_exe;
    });

    /// <summary>
    /// %windir%\regedit.exe
    /// </summary>
    [Obsolete("use var regedit_exe = Path.Combine(windir.ThrowIsNull(), \"regedit.exe\");", true)]
    public static string Regedit => _regedit_exe.Value;

    /// <summary>
    /// 带参数(可选/null)启动 %windir%\regedit.exe 并等待退出后删除文件
    /// </summary>
    [Obsolete("use ShellHelper.StartProcessRegeditAsync", true)]
    public static async Task StartProcessRegeditAsync(
        string path,
        string contents,
        int millisecondsDelay = 3700)
    {
        var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var regedit_exe = Path.Combine(windir.ThrowIsNull(), "regedit.exe");
        File.WriteAllText(path, contents, Encoding.UTF8);
        var args = $"/s \"{path}\"";
        var p = Process2.Start(regedit_exe, args, workingDirectory: windir);
        await IOPath.TryDeleteInDelayAsync(p, path, millisecondsDelay, millisecondsDelay);
    }
#endif
}
#endif
#endif