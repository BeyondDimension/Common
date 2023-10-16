#if DEBUG
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Net.Http;

/// <summary>
/// <see cref="HttpClient"/> 服务基类
/// </summary>
/// <typeparam name="TService"></typeparam>
/// <param name="loggerFactory"></param>
/// <param name="client"></param>
[Obsolete("HttpClientService use BD.Common8.WebApiClient, Http 服务 HttpClient/WebProxy/Cookie/Handler 配置应独立与服务之外", true)]
public abstract class HttpClientServiceBaseImpl<TService>(ILoggerFactory loggerFactory, HttpClient client) : IDisposable where TService : HttpClientServiceBaseImpl<TService>
{
    bool disposedValue;

    /// <inheritdoc cref="ILogger"/>
    protected readonly ILogger logger = loggerFactory.CreateLogger<TService>();

    /// <inheritdoc cref="HttpClient"/>
    protected readonly HttpClient client = client;

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                client.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 使用 <see cref="CookieContainer"/> 的 HttpClient 服务基类
/// </summary>
/// <typeparam name="TService"></typeparam>
[Obsolete("HttpClientService use BD.Common8.WebApiClient, Http 服务 HttpClient/WebProxy/Cookie/Handler 配置应独立与服务之外", true)]
public abstract class HttpClientUseCookiesServiceBaseImpl<TService> : IDisposable where TService : HttpClientUseCookiesServiceBaseImpl<TService>
{
    /// <summary>
    /// 当前 <see cref="HttpClient"/> 服务所使用的 Cookie 容器
    /// </summary>
    protected readonly CookieContainer cookieContainer = new();

    /// <inheritdoc cref="ILogger"/>
    protected readonly ILogger logger;

    /// <summary>
    /// 当前 <see cref="HttpClient"/> 服务所使用的 Handler
    /// </summary>
    protected HttpMessageHandler Handler { get; private set; }

    Lazy<HttpClient> _client;
    bool disposedValue;

#pragma warning disable IDE1006 // 命名样式
    /// <inheritdoc cref="HttpClient"/>
    protected HttpClient client => _client.Value;
#pragma warning restore IDE1006 // 命名样式

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientUseCookiesServiceBaseImpl{TService}"/> class.
    /// </summary>
    /// <param name="loggerFactory"></param>
    public HttpClientUseCookiesServiceBaseImpl(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<TService>();
        Handler = CreateHandler(cookieContainer);
        _client = GetLazyHttpClient();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientUseCookiesServiceBaseImpl{TService}"/> class.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="func"></param>
    public HttpClientUseCookiesServiceBaseImpl(ILoggerFactory loggerFactory, Func<CookieContainer, HttpMessageHandler> func)
    {
        logger = loggerFactory.CreateLogger<TService>();
        Handler = func(cookieContainer);
        _client = GetLazyHttpClient();
    }

    static HttpClientHandler CreateHandler(CookieContainer cookieContainer)
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = cookieContainer,
#if NETFRAMEWORK || NETSTANDARD
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
#else
            AutomaticDecompression = DecompressionMethods.All,
#endif
        };
        return handler;
    }

    Lazy<HttpClient> GetLazyHttpClient() => new(GetHttpClient);

    /// <inheritdoc cref="HttpClient"/>
    protected virtual HttpClient GetHttpClient() => new(Handler);

    /// <summary>
    /// 重新创建 <see cref="Handler"/> 与 <see cref="HttpClient"/>
    /// </summary>
    public virtual void Reset()
    {
        if (_client.IsValueCreated)
        {
            _client.Value.Dispose();
        }
        Handler.Dispose();
        Handler = CreateHandler(cookieContainer);
        _client = GetLazyHttpClient();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                if (_client.IsValueCreated)
                {
                    _client.Value.Dispose();
                }
                Handler.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
#endif