#if !(TARGET_BROWSER || BLAZOR)
#if ANDROID
using HttpHandlerType = Xamarin.Android.Net.AndroidMessageHandler;
#elif IOS || MACCATALYST
using HttpHandlerType = System.Net.Http.NSUrlSessionHandler;
#elif NETFRAMEWORK
using HttpHandlerType = System.Net.Http.HttpClientHandler;
#else
using HttpHandlerType = System.Net.Http.SocketsHttpHandler;
#endif
#endif

namespace System.Net.Http;

public abstract class HttpClientServiceBaseImpl : IDisposable
{
    bool disposedValue;
    protected readonly HttpClient client;

    public HttpClientServiceBaseImpl(HttpClient client)
    {
        this.client = client;
    }

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

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

#if !(TARGET_BROWSER || BLAZOR)
public abstract class HttpClientUseCookiesServiceBaseImpl : IDisposable
{
    protected readonly CookieContainer cookieContainer = new();

    protected HttpHandlerType Handler { get; private set; }

    Lazy<HttpClient> _client;
    bool disposedValue;

#pragma warning disable IDE1006 // 命名样式
    protected HttpClient client => _client.Value;
#pragma warning restore IDE1006 // 命名样式

    public HttpClientUseCookiesServiceBaseImpl()
    {
        Handler = CreateHandler(cookieContainer);
        _client = GetLazyHttpClient();
    }

    public HttpClientUseCookiesServiceBaseImpl(Func<CookieContainer, HttpHandlerType> func)
    {
        Handler = func(cookieContainer);
        _client = GetLazyHttpClient();
    }

    static HttpHandlerType CreateHandler(CookieContainer cookieContainer)
    {
        var handler = new HttpHandlerType
        {
            UseCookies = true,
            CookieContainer = cookieContainer,
#if NETFRAMEWORK
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
#else
            AutomaticDecompression = DecompressionMethods.All,
#endif
        };
        //#if !NETFRAMEWORK
        //        handler.Compatibility();
        //#endif
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
        Handler = CreateHandler(cookieContainer);
        _client = GetLazyHttpClient();
    }

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

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
#endif