using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class Compat
{
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
#elif __HAVE_RUNTIME_INFORMATION__ || NET471 || NET48 || NET481 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_6 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP1_0_OR_GREATER
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
        Environment.OSVersion.Platform == PlatformID.Win32NT;
#endif
}
