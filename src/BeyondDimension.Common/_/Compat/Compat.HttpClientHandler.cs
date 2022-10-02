using System.Security.Authentication;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class Compat
{
    // https://github.com/dotnet/runtime/issues/25722

    const SslProtocols EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;

#if NETCOREAPP2_1_OR_GREATER && !BLAZOR && !TARGET_BROWSER
    public static void Compatibility(this SocketsHttpHandler handler)
    {
        if (IsWindows7())
        {
            handler.SslOptions.EnabledSslProtocols = EnabledSslProtocols;
        }
    }
#endif

    public static void Compatibility(this HttpClientHandler handler)
    {
        if (IsWindows7())
        {
            handler.SslProtocols = EnabledSslProtocols;
        }
    }
}
