namespace System.Net;

/// <summary>
/// 动态代理包装类，通过此包装实现动态切换代理的实现，以应用不同的代理配置
/// </summary>
public sealed class DynamicWebProxy : IWebProxy
{
    /// <summary>
    /// 代理的实现
    /// </summary>
    public IWebProxy InnerProxy { get; set; } =
#if NETCOREAPP3_0_OR_GREATER
        HttpClient.DefaultProxy;
#else
        HttpNoProxy.Instance;
#endif

    /// <inheritdoc cref="IWebProxy.Credentials"/>
    public ICredentials? Credentials
    {
        get => InnerProxy.Credentials;
        set => InnerProxy.Credentials = value;
    }

    /// <inheritdoc cref="IWebProxy.GetProxy(Uri)"/>
    public Uri? GetProxy(Uri destination) => InnerProxy.GetProxy(destination);

    /// <inheritdoc cref="IWebProxy.IsBypassed(Uri)"/>
    public bool IsBypassed(Uri host) => InnerProxy.IsBypassed(host);
}