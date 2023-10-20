#if WINDOWS7_0_OR_GREATER
#pragma warning disable CS8981
using winmdroot = Windows.Win32;
#pragma warning restore CS8981

namespace System.Extensions;

#pragma warning disable SA1600 // Elements should be documented

internal static partial class FreeMemoryExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void FreeMemory(
        this winmdroot.Networking.WinHttp.WINHTTP_PROXY_INFO proxyInfo)
    {
        char* lpszProxy = proxyInfo.lpszProxy;
        Marshal.FreeHGlobal((nint)lpszProxy);
        char* lpszProxyBypass = proxyInfo.lpszProxyBypass;
        Marshal.FreeHGlobal((nint)lpszProxyBypass);
        proxyInfo.lpszProxy = proxyInfo.lpszProxyBypass = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void FreeMemory(
        this winmdroot.Networking.WinHttp.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG proxyConfig)
    {
        char* lpszAutoConfigUrl = proxyConfig.lpszAutoConfigUrl;
        Marshal.FreeHGlobal((nint)lpszAutoConfigUrl);
        char* lpszProxy = proxyConfig.lpszProxy;
        Marshal.FreeHGlobal((nint)lpszProxy);
        char* lpszProxyBypass = proxyConfig.lpszProxyBypass;
        Marshal.FreeHGlobal((nint)lpszProxyBypass);
        proxyConfig.lpszAutoConfigUrl = proxyConfig.lpszProxy = proxyConfig.lpszProxyBypass = default;
    }
}
#endif
