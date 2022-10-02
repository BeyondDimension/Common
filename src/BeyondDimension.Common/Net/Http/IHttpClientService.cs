#if NETFRAMEWORK
using SocketsHttpHandler = System.Net.Http.HttpClientHandler;
#endif

namespace System.Net.Http;

public interface IHttpClientService
{
    HttpClient HttpClient { get; }
}

public abstract class HttpClientServiceImpl : IHttpClientService
{
    protected readonly HttpClient client;

    public HttpClientServiceImpl(HttpClient client)
    {
        this.client = client;
    }

    HttpClient IHttpClientService.HttpClient => client;
}

#if !(TARGET_BROWSER || BLAZOR)
public abstract class HttpClientUseCookiesServiceImpl : IHttpClientService
{
    protected readonly CookieContainer cookieContainer = new();
    protected readonly HttpClient client;

    protected virtual void ConfigurePrimaryHttpMessageHandler(HttpClientHandler handler)
    {

    }

    public HttpClientUseCookiesServiceImpl()
    {
        var handler = new HttpClientHandler
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
        ConfigurePrimaryHttpMessageHandler(handler);
        client = new(handler);
    }

    HttpClient IHttpClientService.HttpClient => client;
}
#endif