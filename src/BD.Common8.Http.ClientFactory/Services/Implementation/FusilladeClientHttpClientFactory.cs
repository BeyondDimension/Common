using IClientHttpClientFactory = System.Net.Http.Client.IClientHttpClientFactory;

namespace BD.Common8.Http.ClientFactory.Services.Implementation;

using Punchclock;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

/// <summary>
/// 使用 Fusillade 实现的 <see cref="IClientHttpClientFactory"/>
/// <para>https://github.com/dotnet/runtime/blob/v7.0.3/src/libraries/Microsoft.Extensions.Http/src/DefaultHttpClientFactory.cs</para>
/// </summary>
public class FusilladeClientHttpClientFactory : IClientHttpClientFactory, IDisposable
{
    bool disposedValue;

    readonly HttpMessageHandler handler;

    readonly ConcurrentDictionary<(string, HttpHandlerCategory), HttpClient> activeClients = new();

    /// <summary>
    /// 默认的 <see cref="HttpMessageHandler"/> 实例的字典
    /// /// </summary>
    internal static readonly Dictionary<string, DefaultHttpClientBuilder> Builders = [];

    /// <summary>
    /// 初始化 <see cref="FusilladeClientHttpClientFactory"/> 的实例
    /// </summary>
    /// <param name="registerConstant">是否将 <see cref="HttpMessageHandler"/> 注册到服务定位器</param>
    public FusilladeClientHttpClientFactory(bool registerConstant = true) : this(IClientHttpClientFactory.CreateHandler())
    {
    }

    /// <summary>
    /// 初始化 <see cref="FusilladeClientHttpClientFactory"/> 的实例
    /// </summary>
    /// <param name="handler">自定义的 <see cref="HttpMessageHandler"/> 实例</param>
    /// <param name="registerConstant">是否将 <see cref="HttpMessageHandler"/> 注册到服务定位器</param>
    public FusilladeClientHttpClientFactory(HttpMessageHandler handler)
    {
        this.handler = handler;
        OperationQueue operationQueue = new(12);
        NetCache.Speculative = new RateLimitedHttpMessageHandler2(handler, Priority.Speculative, 0, 5242880L, opQueue: operationQueue);
        NetCache.UserInitiated = new RateLimitedHttpMessageHandler2(handler, Priority.UserInitiated, opQueue: operationQueue);
        NetCache.Background = new RateLimitedHttpMessageHandler2(handler, Priority.Background, opQueue: operationQueue);
    }

    /// <summary>
    /// 创建一个 <see cref="HttpClient"/> 实例并设置默认超时时间
    /// </summary>
    protected virtual HttpClient CreateClient(HttpMessageHandler handler)
    {
        var client = IClientHttpClientFactory.CreateClient(handler);
        return client;
    }

    /// <summary>
    /// 创建或获取基于给定名称和分类的 HttpClient 实例
    /// </summary>
    HttpClient IClientHttpClientFactory.CreateClient(string name, HttpHandlerCategory category)
    {
        if (!category.IsDefined())
            category = HttpHandlerCategory.Default;

        ValueTuple<string, HttpHandlerCategory> key = new(name, category);

        if (activeClients.TryGetValue(key, out var result))
            return result;

        HttpMessageHandler? handler = default;

        if (Builders.TryGetValue(name, out var builder))
            if (builder.ConfigureHandler != null)
                handler = builder.ConfigureHandler(GetHandler);

        HttpMessageHandler GetHandler()
        {
            HttpMessageHandler handler = category switch
            {
                HttpHandlerCategory.Speculative => NetCache.Speculative,
                HttpHandlerCategory.UserInitiated => NetCache.UserInitiated,
                HttpHandlerCategory.Background => NetCache.Background,
                HttpHandlerCategory.Offline => NetCache.Offline,
                _ => this.handler,
            };
            return handler;
        }

        handler ??= GetHandler();
        var client = CreateClient(handler);
        switch (category)
        {
            case HttpHandlerCategory.Offline:
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(IClientHttpClientFactory.DefaultLocalTimeoutFromSeconds);
                }
                catch
                {
                }
                break;
        }

        if (builder != default)
            builder.ConfigureClient?.Invoke(client);

        return activeClients[key] = client;
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                foreach (var activeClient in activeClients.Values)
                    try
                    {
                        activeClient.Dispose();
                    }
                    catch
                    {
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

    /// <inheritdoc/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 将 <see cref="FusilladeClientHttpClientFactory"/> 添加到服务集合中
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IServiceCollection AddFusilladeHttpClientFactory(
        IServiceCollection services,
        HttpMessageHandler? handler)
    {
        IClientHttpClientFactory.AddHttpClientDelegateValue = static (services, type) =>
        {
            services.AddFusilladeHttpClient(TypeNameHelper.GetTypeDisplayName(type));
        };
        FusilladeClientHttpClientFactory factory = handler == null ? new() :
            new(handler);
        services.AddSingleton(factory);
        services.AddSingleton<IClientHttpClientFactory>(factory);

        services.AddSingleton<CookieClientHttpClientFactory>();

        return services;
    }
}

sealed class InflightRequest(Action onFullyCancelled)
{
    int _refCount = 1;

    public AsyncSubject<HttpResponseMessage> Response { get; protected set; }
        = new AsyncSubject<HttpResponseMessage>();

    public void AddRef() => Interlocked.Increment(ref _refCount);

    public void Cancel()
    {
        if (Interlocked.Decrement(ref _refCount) <= 0)
        {
            onFullyCancelled();
        }
    }
}

/// <summary>
/// A http handler which will limit the rate at which we can read.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RateLimitedHttpMessageHandler2"/> class.
/// </remarks>
/// <param name="handler">The handler we are wrapping.</param>
/// <param name="basePriority">The base priority of the request.</param>
/// <param name="priority">The priority of the request.</param>
/// <param name="maxBytesToRead">The maximum number of bytes we can read.</param>
/// <param name="opQueue">The operation queue on which to run the operation.</param>
/// <param name="cacheResultFunc">A method that is called if we need to get cached results.</param>
sealed class RateLimitedHttpMessageHandler2(HttpMessageHandler handler, Priority basePriority, int priority = 0, long? maxBytesToRead = null, OperationQueue? opQueue = null, Func<HttpRequestMessage, HttpResponseMessage, string, CancellationToken, Task>? cacheResultFunc = null) : LimitingHttpMessageHandler(handler)
{
    readonly int _priority = (int)basePriority + priority;
    readonly Dictionary<string, InflightRequest> _inflightResponses = new();
    long? _maxBytesToRead = maxBytesToRead;

    /// <summary>
    /// Generates a unique key for a <see cref="HttpRequestMessage"/>.
    /// This assists with the caching.
    /// </summary>
    /// <param name="originalRequestUri"></param>
    /// <param name="request">The request to generate a unique key for.</param>
    /// <returns>The unique key.</returns>
    public static string UniqueKeyForRequest(
        string originalRequestUri,
        HttpRequestMessage request)
    {
        // https://github.com/reactiveui/Fusillade/blob/2.4.67/src/Fusillade/RateLimitedHttpMessageHandler.cs#L54-L89

        using var s = new MemoryStream();
        s.Write(Encoding.UTF8.GetBytes(originalRequestUri));
        s.Write("\r\n"u8);
        s.Write(Encoding.UTF8.GetBytes(request.Method.Method));
        s.Write("\r\n"u8);
        static void Write(Stream s, IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                var str = item.ToString();
                if (!string.IsNullOrEmpty(str))
                    s.Write(Encoding.UTF8.GetBytes(str));
                s.Write("|"u8);
            }
        }
        Write(s, request.Headers.Accept);
        s.Write("\r\n"u8);
        Write(s, request.Headers.AcceptEncoding);
        s.Write("\r\n"u8);
        var referrer = request.Headers.Referrer;
        if (referrer == default)
            s.Write("http://example"u8);
        else
            s.Write(Encoding.UTF8.GetBytes(referrer.ToString()));
        s.Write("\r\n"u8);
        Write(s, request.Headers.UserAgent);
        s.Write("\r\n"u8);
        if (request.Headers.Authorization != null)
        {
            var parameter = request.Headers.Authorization.Parameter;
            if (!string.IsNullOrEmpty(parameter))
                s.Write(Encoding.UTF8.GetBytes(parameter));
            s.Write(Encoding.UTF8.GetBytes(request.Headers.Authorization.Scheme));
            s.Write("\r\n"u8);
        }
        s.Position = 0;
        var bytes = SHA384.HashData(s);
        var str = bytes.ToHexString();
        return str;
    }

    /// <summary>
    /// Generates a unique key for a <see cref="HttpRequestMessage"/>.
    /// This assists with the caching.
    /// </summary>
    /// <param name="request">The request to generate a unique key for.</param>
    /// <returns>The unique key.</returns>
    public static string UniqueKeyForRequest(HttpRequestMessage request)
    {
        var requestUriString = ImageHttpClientServiceImpl.ImageHttpRequestMessage.GetOriginalRequestUri(request);
        return UniqueKeyForRequest(requestUriString, request); ;
    }

    /// <inheritdoc />
    public override void ResetLimit(long? maxBytesToRead = null) => _maxBytesToRead = maxBytesToRead;

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var method = request.Method;
        if (method != HttpMethod.Get && method != HttpMethod.Head && method != HttpMethod.Options)
        {
            return base.SendAsync(request, cancellationToken);
        }

        var cacheResult = cacheResultFunc;
        if (cacheResult == null && NetCache.RequestCache != null)
        {
            cacheResult = NetCache.RequestCache.Save;
        }

        if (_maxBytesToRead < 0)
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
#if NETSTANDARD2_0
            tcs.SetCanceled();
#else
            tcs.SetCanceled(cancellationToken);
#endif
            return tcs.Task;
        }

        var key = UniqueKeyForRequest(request);
        var realToken = new CancellationTokenSource();
        var ret = new InflightRequest(() =>
        {
            lock (_inflightResponses)
            {
                _inflightResponses.Remove(key);
            }

            realToken.Cancel();
        });

        lock (_inflightResponses)
        {
            if (_inflightResponses.ContainsKey(key))
            {
                var val = _inflightResponses[key];
                val.AddRef();
                cancellationToken.Register(val.Cancel);

                return val.Response.ToTask(cancellationToken);
            }

            _inflightResponses[key] = ret;
        }

        cancellationToken.Register(ret.Cancel);

        var queue = new OperationQueue();

        queue.Enqueue(
            _priority,
            null!,
            async () =>
            {
                try
                {
                    var resp = await base.SendAsync(request, realToken.Token).ConfigureAwait(false);

                    if (_maxBytesToRead != null && resp.Content?.Headers.ContentLength != null)
                    {
                        _maxBytesToRead -= resp.Content.Headers.ContentLength;
                    }

                    if (cacheResult != null && resp.Content != null)
                    {
                        var ms = new MemoryStream();
#if NET5_0_OR_GREATER
                        var stream = await resp.Content.ReadAsStreamAsync(realToken.Token).ConfigureAwait(false);
#else
                        var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
                        await stream.CopyToAsync(ms, 32 * 1024, realToken.Token).ConfigureAwait(false);

                        realToken.Token.ThrowIfCancellationRequested();

                        var newResp = new HttpResponseMessage();
                        foreach (var kvp in resp.Headers)
                        {
                            newResp.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                        }

                        var newContent = new ByteArrayContent(ms.ToArray());
                        foreach (var kvp in resp.Content.Headers)
                        {
                            newContent.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                        }

                        newResp.Content = newContent;

                        resp = newResp;
                        await cacheResult(request, resp, key, realToken.Token).ConfigureAwait(false);
                    }

                    return resp;
                }
                finally
                {
                    lock (_inflightResponses)
                    {
                        _inflightResponses.Remove(key);
                    }
                }
            },
            realToken.Token).ToObservable().Subscribe(ret.Response);

        return ret.Response.ToTask(cancellationToken);
    }
}