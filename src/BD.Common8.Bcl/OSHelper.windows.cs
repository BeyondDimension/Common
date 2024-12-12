#if WINDOWS
using Windows.ApplicationModel;
#endif

namespace System;

partial class OSHelper
{
    /// <summary>
    /// 指示当前应用程序是否正在 Windows 11 或更高版本上运行。
    /// </summary>
    [SupportedOSPlatformGuard("Windows10.0.22000")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWindows11AtLeast() =>
#if NETSTANDARD1_0 || __MACOS__ || __ANDROID__ || __IOS__ || __WATCHOS__ || __TVOS__
        false;
#else
        _IsWindows11AtLeast.Value;

    static readonly Lazy<bool> _IsWindows11AtLeast = new(() => OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000));
#endif

#if WINDOWS
    static string? GetAppUserModelId()
    {
        try
        {
            // 当由未打包的应用程序使用时，访问此属性会引发 InvalidOperationException 和 HRESULT，指示“进程没有包标识”。80073D54
#pragma warning disable CA1416 // 验证平台兼容性
            var appInfo = AppInfo.Current;
#pragma warning restore CA1416 // 验证平台兼容性
            if (appInfo != null)
            {
                return appInfo.AppUserModelId;
            }
        }
        catch
        {
            try
            {
                return $"{Package.Current.Id.FamilyName}!App";
            }
            catch
            {
            }
        }
        return null;
    }

    const string Shell_AppsFolder_ = @"Shell:AppsFolder\{0}";

    /// <summary>
    /// 获取 UWP 应用程序的 Shell:AppsFolder 路径，例如：Shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App
    /// <para>返回 <see langword="null"/> 时表示获取当前应用程序的 AppUserModelId 失败，当前应用程序缺少表标识</para>
    /// </summary>
    /// <param name="appUserModelId">传递 <see langword="null"/> 时将使用当前应用程序的 AppUserModelId</param>
    /// <returns></returns>
    public static string? GetShellAppsFolderPath(string? appUserModelId = null)
    {
        appUserModelId ??= GetAppUserModelId();
        return appUserModelId == null ? null : string.Format(Shell_AppsFolder_, appUserModelId);
    }
#endif
}