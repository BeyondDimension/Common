#if WINDOWS_UWP
using Windows.System.Profile;
#endif

// ReSharper disable once CheckNamespace
namespace System;

public static partial class Compat
{
    /// <summary>
    /// 获取当前系统版本号。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Version OSVersion() =>
#if NETSTANDARD1_0
        throw new PlatformNotSupportedException();
#elif NETSTANDARD1_1 || WINDOWS_UWP
        _WindowsVersion.Value;
#elif __HAVE_XAMARIN_ESSENTIALS__
        DeviceInfo.Version;
#else
        Environment.OSVersion.Version;
#endif

#if NETSTANDARD1_1
    [StructLayout(LayoutKind.Sequential)]
    struct RTL_OSVERSIONINFOEX
    {
        internal uint dwOSVersionInfoSize;
        internal uint dwMajorVersion;
        internal uint dwMinorVersion;
        internal uint dwBuildNumber;
        internal uint dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string szCSDVersion;
    }
    
    [DllImport("ntdll")]
    static extern int RtlGetVersion(ref RTL_OSVERSIONINFOEX lpVersionInformation);
    
    static Version RtlGetVersion()
    {
        RTL_OSVERSIONINFOEX v = new RTL_OSVERSIONINFOEX();
        v.dwOSVersionInfoSize = (uint)Marshal.SizeOf(v);
        if (RtlGetVersion(ref v) == 0)
        {
            return new Version((int)v.dwMajorVersion, (int)v.dwMinorVersion, (int)  .dwBuildNumber);
        }
        else
        {
            throw new Exception("RtlGetVersion failed!");
        }
    }

    static readonly Lazy<Version> _WindowsVersion = new(RtlGetVersion);

#elif WINDOWS_UWP
    static readonly Lazy<Version> _WindowsVersion = new(WindowsVersion_);

    static Version WindowsVersion_()
    {
        var sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
        var v = ulong.Parse(sv);
        var major = (v & 0xFFFF000000000000L) >> 48;
        var minor = (v & 0x0000FFFF00000000L) >> 32;
        var build = (v & 0x00000000FFFF0000L) >> 16;
        var revision = v & 0x000000000000FFFFL;
        return new Version(
            Convert.ToInt32(major),
            Convert.ToInt32(minor),
            Convert.ToInt32(build),
            Convert.ToInt32(revision));
    }
#endif
}
