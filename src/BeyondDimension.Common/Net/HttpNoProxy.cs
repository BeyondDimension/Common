// ReSharper disable once CheckNamespace
namespace System.Net;

#if !NETFRAMEWORK

/// <summary>
/// https://github.com/dotnet/runtime/blob/v6.0.0/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/HttpNoProxy.cs
/// </summary>
public sealed class HttpNoProxy : IWebProxy
{
    public static readonly HttpNoProxy Instance = new();

    HttpNoProxy() { }

    public ICredentials? Credentials { get; set; }

    public Uri? GetProxy(Uri destination) => null;

    public bool IsBypassed(Uri host) => true;

    /// <summary>
    /// 检查当前 Web 代理对象是否为无代理
    /// </summary>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static bool IsNoProxy([NotNullWhen(false)] IWebProxy? proxy)
        => proxy == null || proxy.GetType().Name == nameof(HttpNoProxy);
}

#endif