namespace System.Net.Http;

public abstract class HttpClientUseCookiesWithProxyServiceImpl : HttpClientUseCookiesServiceBaseImpl
{
    public sealed class DynamicWebProxy : IWebProxy
    {
        public IWebProxy InnerProxy { get; set; } = HttpNoProxy.Instance;

        public ICredentials? Credentials
        {
            get => InnerProxy.Credentials;
            set => InnerProxy.Credentials = value;
        }

        public Uri? GetProxy(Uri destination) => InnerProxy.GetProxy(destination);

        public bool IsBypassed(Uri host) => InnerProxy.IsBypassed(host);
    }

    public interface IAppSettings
    {
        string? WebProxyAddress { get; set; }

        int? WebProxyPort { get; set; }

        string? WebProxyUserName { get; set; }

        string? WebProxyPassword { get; set; }
    }

    protected readonly ILogger logger;
    readonly Func<SocketsHttpHandler, HttpClient>? func;

    public HttpClientUseCookiesWithProxyServiceImpl(Func<SocketsHttpHandler, HttpClient>? func, ILogger logger)
    {
        this.func = func;
        this.logger = logger;
    }

    protected override HttpClient GetHttpClient()
        => func == null ? base.GetHttpClient() : func(Handler);

    //#region Cookie 序列化与反序列化

    // 直接使用 MemoryPack

    //static readonly MessagePackSerializerOptions cookieOptions = MessagePackSerializerOptions.Standard
    //        .WithCompression(MessagePackCompression.Lz4BlockArray)
    //        .WithResolver(CompositeResolver.Create(
    //            new IMessagePackFormatter[] { new CookieFormatter() },
    //            new IFormatterResolver[] { StandardResolver.Instance }));

    //public static byte[]? Serialize(CookieCollection? cookies)
    //{
    //    if (cookies == null) return null;
    //    var buffer = MessagePackSerializer.Serialize(cookies, cookieOptions);
    //    return buffer;
    //}

    //public static string? SerializeString(CookieCollection? cookies)
    //{
    //    var buffer = Serialize(cookies);
    //    if (buffer == null) return null;
    //    var result = buffer.Base64Encode();
    //    return result;
    //}

    //public static CookieCollection? Deserialize(byte[]? buffer)
    //{
    //    if (buffer == null) return null;
    //    var result = MessagePackSerializer.Deserialize<CookieCollection>(buffer, cookieOptions);
    //    return result;
    //}

    //public static CookieCollection? DeserializeString(string? value)
    //{
    //    if (string.IsNullOrEmpty(value)) return null;
    //    var buffer = value.Base64DecodeToByteArray();
    //    var result = Deserialize(buffer);
    //    return result;
    //}

    //#endregion
}

public abstract class HttpClientUseCookiesWithDynamicProxyServiceImpl : HttpClientUseCookiesWithProxyServiceImpl, IDisposable
{
    readonly DynamicWebProxy? proxy;
    protected bool disposedValue;
    readonly IDisposable? options_disposable;

    public HttpClientUseCookiesWithDynamicProxyServiceImpl(IServiceProvider? s, ILogger logger) : base(null, logger)
    {
        if (s == null) return;
        var o = s.GetService<IOptionsMonitor<IAppSettings>>();
        var options = o == null ? default : o.CurrentValue;
        proxy = new DynamicWebProxy()
        {
            InnerProxy = GetWebProxy(options),
        };
        SetHandler();

        options_disposable = o?.OnChange(o =>
        {
            proxy.InnerProxy = GetWebProxy(o);
        });
    }

    void SetHandler()
    {
        Handler.UseProxy = true;
        Handler.Proxy = proxy;
    }

    public override void Reset()
    {
        base.Reset();
        SetHandler();
    }

    public HttpClientUseCookiesWithDynamicProxyServiceImpl(Func<SocketsHttpHandler, HttpClient>? func, ILogger logger) : base(func, logger)
    {

    }

    protected virtual IWebProxy GetDefaultProxy()
    {
        var value = HttpClient.DefaultProxy;
        if (value != null) return value;
        return HttpNoProxy.Instance;
    }

    public void RefreshProxy(IWebProxy webProxy)
    {
        proxy!.InnerProxy = webProxy;
    }

    protected virtual IWebProxy GetWebProxy(IAppSettings? o)
    {
        if (string.IsNullOrWhiteSpace(o?.WebProxyAddress))
            return GetDefaultProxy();

        ICredentials? credentials = null;
        if (!string.IsNullOrEmpty(o.WebProxyUserName) && !string.IsNullOrEmpty(o.WebProxyPassword))
            credentials = new NetworkCredential(o.WebProxyUserName, o.WebProxyPassword);

        var url = $"{o.WebProxyAddress}:{o.WebProxyPort}";
        return new WebProxy(url, true, null, credentials);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                options_disposable?.Dispose();
                client.Dispose();
                Handler.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public static IServiceCollection TryAddAppSettings<TAppSettings>(
        IServiceCollection services)
            where TAppSettings : IAppSettings
    {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        services.TryAddSingleton(s =>
          (IOptionsMonitor<IAppSettings>)s.GetRequiredService<IOptionsMonitor<TAppSettings>>());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        return services;
    }

    public static IServiceCollection AddTransientService<TAppSettings,
        TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        IServiceCollection services)
            where TAppSettings : IAppSettings
            where TService : class
            where TImplementation : class, TService
    {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        services.TryAddSingleton(s =>
          (IOptionsMonitor<IAppSettings>)s.GetRequiredService<IOptionsMonitor<TAppSettings>>());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        services.AddTransient<TService, TImplementation>();
        return services;
    }
}