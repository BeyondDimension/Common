using System.Security.Authentication;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class Compat
{
    // https://github.com/dotnet/runtime/issues/25722

    public const SslProtocols EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;

#if NETCOREAPP2_1_OR_GREATER && !BLAZOR && !TARGET_BROWSER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Compatibility(this SocketsHttpHandler handler)
    {
#if !NET7_0_OR_GREATER
        if (OperatingSystem2.IsWindows7())
        {
            handler.SslOptions.EnabledSslProtocols = EnabledSslProtocols;
        }
#endif
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Compatibility(this HttpClientHandler handler)
    {
#if !NET7_0_OR_GREATER
        if (OperatingSystem2.IsWindows7())
        {
            handler.SslProtocols = EnabledSslProtocols;
        }
#endif
    }
}
