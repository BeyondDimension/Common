#if !NET5_0_OR_GREATER

namespace System;

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

public static partial class OperatingSystem2
{
    /// <summary>
    /// 指示当前应用程序是否正在 macOS 上运行。
    /// </summary>
    [SupportedOSPlatformGuard("macos")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMacOS() =>
#if NET5_0_WINDOWS || NET6_0_WINDOWS || NET7_0_WINDOWS || __ANDROID__ || __TVOS__ || __WATCHOS__ || WINDOWS_UWP
        false;
#elif __MACOS__ || NET6_0_MACOS10_14
        true;
#elif NET5_0 || NET6_0 || NET7_0 || NET6_0_MACCATALYST
        OperatingSystem.IsMacOS();
#elif NET471_OR_GREATER
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#else
        Environment.OSVersion.Platform == PlatformID.MacOSX;
#endif

    /// <summary>
    /// 指示当前应用程序是否正在 Windows 上运行。
    /// </summary>
    [SupportedOSPlatformGuard("windows")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWindows() =>
#if NETSTANDARD1_0 || __MACOS__ || __ANDROID__ || __IOS__ || __WATCHOS__ || __TVOS__
        false;
#elif NET5_0_WINDOWS || NET6_0_WINDOWS || NET7_0_WINDOWS || WINDOWS_UWP
        true;
#elif NET5_0 || NET6_0 || NET7_0
        OperatingSystem.IsWindows();
#elif NET471_OR_GREATER
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
        Environment.OSVersion.Platform == PlatformID.Win32NT;
#endif
}
#endif