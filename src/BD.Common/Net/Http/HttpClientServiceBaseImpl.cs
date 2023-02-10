#if NETFRAMEWORK
using SocketsHttpHandler = System.Net.Http.HttpClientHandler;
#endif

namespace System.Net.Http;

public abstract class HttpClientServiceBaseImpl
{
    protected readonly HttpClient client;

    public HttpClientServiceBaseImpl(HttpClient client)
    {
        this.client = client;
    }
}

#if !(TARGET_BROWSER || BLAZOR)
public abstract class HttpClientUseCookiesServiceBaseImpl
{
    protected readonly CookieContainer cookieContainer = new();

    protected SocketsHttpHandler Handler { get; private set; }

    Lazy<HttpClient> _client;

#pragma warning disable IDE1006 // 命名样式
    protected HttpClient client => _client.Value;
#pragma warning restore IDE1006 // 命名样式

    public HttpClientUseCookiesServiceBaseImpl()
    {
        Handler = GetSocketsHttpHandler(cookieContainer);
        _client = GetLazyHttpClient();
    }

    static SocketsHttpHandler GetSocketsHttpHandler(CookieContainer cookieContainer)
    {
        var handler = new SocketsHttpHandler
        {
            UseCookies = true,
            CookieContainer = cookieContainer,
#if NETFRAMEWORK
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
#else
            AutomaticDecompression = DecompressionMethods.All,
#endif
        };
#if !NETFRAMEWORK
        handler.Compatibility();
#endif
        return handler;
    }

    Lazy<HttpClient> GetLazyHttpClient() => new(GetHttpClient);

    protected virtual HttpClient GetHttpClient() => new(Handler);

    public virtual void Reset()
    {
        if (_client.IsValueCreated)
        {
            _client.Value.Dispose();
        }
        Handler.Dispose();
        Handler = GetSocketsHttpHandler(cookieContainer);
        _client = GetLazyHttpClient();
    }
}
#endif