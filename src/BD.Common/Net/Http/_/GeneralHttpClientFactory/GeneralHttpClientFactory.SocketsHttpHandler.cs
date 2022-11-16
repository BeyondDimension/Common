#if !NETSTANDARD

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

partial class GeneralHttpClientFactory
{
    public static SocketsHttpHandler CreateSocketsHttpHandler(SocketsHttpHandler? handler = null)
    {
        handler ??= new();
        Compat.Compatibility(handler);
        return handler;
    }
}

#endif