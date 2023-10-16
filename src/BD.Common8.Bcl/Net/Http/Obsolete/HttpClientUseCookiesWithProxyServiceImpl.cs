#if DEBUG
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Net.Http;

/// <summary>
/// 使用 <see cref="IWebProxy"/> 与 <see cref="CookieContainer"/> 的 HttpClient 服务基类
/// </summary>
/// <typeparam name="TService"></typeparam>
[Obsolete("HttpClientService use BD.Common8.WebApiClient, Http 服务 HttpClient/WebProxy/Cookie/Handler 配置应独立与服务之外", true)]
public abstract class HttpClientUseCookiesWithProxyServiceImpl<TService> : HttpClientUseCookiesServiceBaseImpl<TService> where TService : HttpClientUseCookiesWithProxyServiceImpl<TService>
{
    /// <summary>
    /// 动态代理包装类，通过此包装实现动态切换代理的实现，以应用不同的代理配置
    /// </summary>
    [Obsolete("System.Net.DynamicWebProxy", true)]
    public sealed class DynamicWebProxy : IWebProxy
    {
        /// <summary>
        /// 代理的实现
        /// </summary>
        public IWebProxy InnerProxy { get; set; } = HttpNoProxy.Instance;

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

    /// <summary>
    /// 代理的配置参数
    /// </summary>
    [Obsolete("System.Net.IWebProxySettings", true)]
    public interface IAppSettings
    {
        /// <summary>
        /// 代理地址
        /// </summary>
        string? WebProxyAddress { get; set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        int? WebProxyPort { get; set; }

        /// <summary>
        /// 代理的身份验证的用户名
        /// </summary>
        string? WebProxyUserName { get; set; }

        /// <summary>
        /// 代理的身份验证的密码
        /// </summary>
        string? WebProxyPassword { get; set; }
    }

    readonly Func<HttpClientHandler, HttpClient>? func;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientUseCookiesWithProxyServiceImpl{TService}"/> class.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="func"></param>
    public HttpClientUseCookiesWithProxyServiceImpl(ILoggerFactory loggerFactory, Func<HttpClientHandler, HttpClient>? func) : base(loggerFactory)
    {
        this.func = func;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientUseCookiesWithProxyServiceImpl{TService}"/> class.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="func"></param>
    public HttpClientUseCookiesWithProxyServiceImpl(ILoggerFactory loggerFactory, Func<CookieContainer, HttpMessageHandler> func) : base(loggerFactory, func)
    {
    }

    /// <inheritdoc/>
    protected override HttpClient GetHttpClient() => (func != null && Handler is HttpClientHandler handler) ? func(handler) : base.GetHttpClient();
}

/// <summary>
/// TODO
/// </summary>
[Obsolete("HttpClientService use BD.Common8.WebApiClient, Http 服务 HttpClient/WebProxy/Cookie/Handler 配置应独立与服务之外", true)]
public abstract class HttpClientUseCookiesWithDynamicProxyServiceImpl /*: HttpClientUseCookiesWithProxyServiceImpl*/
{
    //    readonly DynamicWebProxy? proxy;
    //    readonly IDisposable? options_disposable;
    //    readonly bool useProxy;

    //    public HttpClientUseCookiesWithDynamicProxyServiceImpl(IServiceProvider? s, ILogger logger) : base((Func<HttpHandlerType, HttpClient>?)null, logger)
    //    {
    //        if (s == null) return;
    //        var o = s.GetService<IOptionsMonitor<IAppSettings>>();
    //        var options = o == null ? default : o.CurrentValue;
    //        useProxy = true;
    //        proxy = new DynamicWebProxy()
    //        {
    //            InnerProxy = GetWebProxy(options),
    //        };
    //        SetHandler();

    //        options_disposable = o?.OnChange(o =>
    //        {
    //            proxy.InnerProxy = GetWebProxy(o);
    //        });
    //    }

    //    public HttpClientUseCookiesWithDynamicProxyServiceImpl(Func<CookieContainer, HttpMessageHandler> func, ILogger logger) : base(func, logger)
    //    {

    //    }

    //    void SetHandler()
    //    {
    //        if (!useProxy) return;
    //        if (Handler is HttpHandlerType handler)
    //        {
    //            handler.UseProxy = true;
    //            handler.Proxy = proxy;
    //        }
    //    }

    //    public override void Reset()
    //    {
    //        if (!useProxy) return;
    //        base.Reset();
    //        SetHandler();
    //    }

    //    public HttpClientUseCookiesWithDynamicProxyServiceImpl(Func<HttpHandlerType, HttpClient>? func, ILogger logger) : base(func, logger)
    //    {

    //    }

    //    protected virtual IWebProxy GetDefaultProxy()
    //    {
    //        var value = HttpClient.DefaultProxy;
    //        if (value != null) return value;
    //        return HttpNoProxy.Instance;
    //    }

    //    public void RefreshProxy(IWebProxy webProxy)
    //    {
    //        proxy!.InnerProxy = webProxy;
    //    }

    //    protected virtual IWebProxy GetWebProxy(IAppSettings? o)
    //    {
    //        if (string.IsNullOrWhiteSpace(o?.WebProxyAddress))
    //            return GetDefaultProxy();

    //        ICredentials? credentials = null;
    //        if (!string.IsNullOrEmpty(o.WebProxyUserName) && !string.IsNullOrEmpty(o.WebProxyPassword))
    //            credentials = new NetworkCredential(o.WebProxyUserName, o.WebProxyPassword);

    //        var url = $"{o.WebProxyAddress}:{o.WebProxyPort}";
    //        return new WebProxy(url, true, null, credentials);
    //    }

    //    bool disposedValue;

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (!disposedValue)
    //        {
    //            if (disposing)
    //            {
    //                options_disposable?.Dispose();
    //            }

    //            disposedValue = true;
    //        }

    //        base.Dispose(disposing);
    //    }

    //    public static IServiceCollection TryAddAppSettings<TAppSettings>(
    //        IServiceCollection services)
    //            where TAppSettings : IAppSettings
    //    {
    //#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
    //        services.TryAddSingleton(s =>
    //          (IOptionsMonitor<IAppSettings>)s.GetRequiredService<IOptionsMonitor<TAppSettings>>());
    //#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
    //        return services;
    //    }

    //    public static IServiceCollection AddTransientService<TAppSettings,
    //        TService,
    //        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
    //        IServiceCollection services)
    //            where TAppSettings : IAppSettings
    //            where TService : class
    //            where TImplementation : class, TService
    //    {
    //#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
    //        services.TryAddSingleton(s =>
    //          (IOptionsMonitor<IAppSettings>)s.GetRequiredService<IOptionsMonitor<TAppSettings>>());
    //#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
    //        services.AddTransient<TService, TImplementation>();
    //        return services;
    //    }
}
#endif