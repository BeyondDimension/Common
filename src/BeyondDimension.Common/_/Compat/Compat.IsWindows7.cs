using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class Compat
{
    /// <summary>
    /// 指示当前应用程序是否正在 Windows 7(NT 6.1) 上运行。
    /// </summary>
#if NET6_0_OR_GREATER
    [SupportedOSPlatformGuard("windows7.0")]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWindows7() =>
#if NETSTANDARD1_0 || __MACOS__ || __ANDROID__ || __IOS__ || __WATCHOS__ || __TVOS__ || WINDOWS_UWP
        false;
#else
#if !(NET5_0_WINDOWS || NET6_0_WINDOWS || NET7_0_WINDOWS)
        IsWindows() &&
#endif
        _IsWindows7.Value;

    static readonly Lazy<bool> _IsWindows7 = new(IsWindows7_);

    static bool IsWindows7_()
    {
        var version_ = OSVersion();
        return version_.Major == 6 && version_.Minor == 1;
    }
#endif
}
