// https://github.com/dotnet/runtime/blob/v7.0.3/src/libraries/Microsoft.Extensions.Http/src/DefaultHttpClientFactory.cs

using Fusillade;
using Splat;
#if ANDROID
using HttpHandlerType = Xamarin.Android.Net.AndroidMessageHandler;
#elif IOS || MACCATALYST
using HttpHandlerType = System.Net.Http.NSUrlSessionHandler;
#else
using HttpHandlerType = System.Net.Http.SocketsHttpHandler;
#endif
using IHttpClientFactory = System.Net.Http.Client.IHttpClientFactory;

// ReSharper disable once CheckNamespace
namespace BD.Common.Services.Implementation;

public class FusilladeHttpClientFactory : IHttpClientFactory, IDisposable
{
    bool disposedValue;
    readonly HttpMessageHandler handler;
    readonly ConcurrentDictionary<(string, HttpHandlerCategory), HttpClient> activeClients = new();

    public FusilladeHttpClientFactory() : this(CreateHandler())
    {

    }

    public FusilladeHttpClientFactory(HttpMessageHandler handler)
    {
        this.handler = handler;
        Locator.CurrentMutable.RegisterConstant(handler, typeof(HttpMessageHandler));
    }

    public static HttpMessageHandler CreateHandler()
    {
        HttpHandlerType handler = new()
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.Brotli | DecompressionMethods.GZip,
        };
        return handler;
    }

    protected virtual HttpClient CreateClient(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler);

        try
        {
            client.Timeout = GeneralHttpClientFactory.DefaultTimeout;
        }
        catch
        {

        }

        return client;
    }

    HttpClient IHttpClientFactory.CreateClient(string name, HttpHandlerCategory category)
    {
        if (!category.IsDefined())
            category = HttpHandlerCategory.Default;

        ValueTuple<string, HttpHandlerCategory> key = new(name, category);

        if (activeClients.TryGetValue(key, out var result))
            return result;

        HttpMessageHandler handler = category switch
        {
            HttpHandlerCategory.Speculative => NetCache.Speculative,
            HttpHandlerCategory.UserInitiated => NetCache.UserInitiated,
            HttpHandlerCategory.Background => NetCache.Background,
            HttpHandlerCategory.Offline => NetCache.Offline,
            _ => this.handler,
        };

        return activeClients[key] = CreateClient(handler);
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                foreach (var activeClient in activeClients.Values)
                {
                    try
                    {
                        activeClient.Dispose();
                    }
                    catch
                    {

                    }
                }

                try
                {
                    handler.Dispose();
                }
                catch
                {

                }

                try
                {
                    NetCache.Speculative?.Dispose();
                }
                catch
                {

                }

                try
                {
                    NetCache.UserInitiated?.Dispose();
                }
                catch
                {

                }

                try
                {
                    NetCache.Background?.Dispose();
                }
                catch
                {

                }

                try
                {
                    NetCache.Offline?.Dispose();
                }
                catch
                {

                }
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
