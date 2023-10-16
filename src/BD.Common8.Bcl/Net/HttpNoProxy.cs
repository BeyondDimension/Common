namespace System.Net;

/// <summary>
/// 无代理
/// <para>https://github.com/dotnet/runtime/blob/v6.0.0/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/HttpNoProxy.cs</para>
/// </summary>
public sealed class HttpNoProxy : IWebProxy
{
    /// <inheritdoc cref="HttpNoProxy"/>
    public static readonly HttpNoProxy Instance = new();

    HttpNoProxy() { }

    /// <inheritdoc/>
    public ICredentials? Credentials { get; set; }

    /// <inheritdoc/>
    public Uri? GetProxy(Uri destination) => null;

    /// <inheritdoc/>
    public bool IsBypassed(Uri host) => true;

    /// <summary>
    /// 检查当前 Web 代理对象是否为无代理
    /// </summary>
    /// <param name="proxy"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNoProxy([NotNullWhen(false)] IWebProxy? proxy)
        => proxy == null || proxy.GetType().Name == nameof(HttpNoProxy);
}