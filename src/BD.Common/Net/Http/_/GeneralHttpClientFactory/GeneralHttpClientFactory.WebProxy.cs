// ReSharper disable once CheckNamespace
namespace System.Net.Http;

partial class GeneralHttpClientFactory
{
#if NETSTANDARD
    static IWebProxy? mDefaultProxy;
#endif

    /// <summary>
    /// 获取或设置全局 HTTP 代理。
    /// </summary>
    public static IWebProxy? DefaultProxy
    {
        get
        {
            IWebProxy? proxy;
#if NETSTANDARD
            proxy = mDefaultProxy;
#else
            proxy = HttpClient.DefaultProxy;
#endif
            return UseWebProxy(proxy) ? proxy : null;
        }

        set
        {
#if NETSTANDARD
            mDefaultProxy = UseWebProxy(value) ? value : null;
            //RefreshWebProxyInDefaultHttpClientFactory(mDefaultProxy);
#else
            HttpClient.DefaultProxy = value ?? HttpNoProxy.Instance;
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool UseWebProxy([NotNullWhen(true)] IWebProxy? proxy)
        => proxy != null && proxy.GetType().Name != nameof(HttpNoProxy);

    static void SetProxyToHandler(HttpMessageHandler handler, IWebProxy? proxy, bool useProxy)
    {
        try
        {
#if NETCOREAPP2_1_OR_GREATER
            if (handler is SocketsHttpHandler s)
            {
                s.Proxy = proxy;
                if (useProxy && s.UseProxy != useProxy) s.UseProxy = useProxy; // 仅启用代理时修改开关
                return;
            }
#endif
            if (handler is HttpClientHandler h)
            {
                h.Proxy = proxy;
                if (useProxy && h.UseProxy != useProxy) h.UseProxy = useProxy; // 仅启用代理时修改开关
            }
        }
        catch (InvalidOperationException)
        {
            // 如果 handler 被释放或者已启动会抛出此异常
            // https://github.com/dotnet/runtime/blob/v6.0.0/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/SocketsHttpHandler.cs#L31
        }
    }

    public static void SetProxyToHandler(IWebProxy? proxy, HttpMessageHandler handler)
    {
        var useProxy = UseWebProxy(proxy);
        SetProxyToHandler(handler, proxy, useProxy);
    }
}
