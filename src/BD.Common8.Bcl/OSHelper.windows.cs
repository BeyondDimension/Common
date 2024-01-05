namespace System;

partial class OSHelper
{
    /// <summary>
    /// 指示当前应用程序是否正在 Windows 11 或更高版本上运行。
    /// </summary>
    [SupportedOSPlatformGuard("windows10.0.22000")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWindows11AtLeast() =>
#if NETSTANDARD1_0 || __MACOS__ || __ANDROID__ || __IOS__ || __WATCHOS__ || __TVOS__
        false;
#else
        _IsWindows11AtLeast.Value;

    static readonly Lazy<bool> _IsWindows11AtLeast = new(() => OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000));
#endif
}