namespace System.Net.Http.Client;

/// <summary>
/// Cookie 隔离容器
/// </summary>
[Obsolete("use CookieClientHttpClientFactory")]
public sealed record class IsolatedCookieContainer : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IsolatedCookieContainer"/> class.
    /// </summary>
    /// <param name="container"></param>
    public IsolatedCookieContainer(CookieContainer? container)
    {
        Container = container ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsolatedCookieContainer"/> class.
    /// </summary>
    /// <param name="containerStream"></param>
    /// <param name="leaveOpen"></param>
    public IsolatedCookieContainer(Stream? containerStream, bool leaveOpen = false)
    {
        if (containerStream != null)
        {
            BinaryContainer = containerStream.ToByteArray();
            if (!leaveOpen)
            {
                containerStream.Dispose();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsolatedCookieContainer"/> class.
    /// </summary>
    /// <param name="containerBuffer"></param>
    public IsolatedCookieContainer(byte[]? containerBuffer)
    {
        BinaryContainer = containerBuffer!;
    }

    /// <summary>
    /// Cookie 容器，可使用 <see cref="CookieExtensions.WriteTo(CookieContainer?, Stream)"/> 将数据保存到流中
    /// </summary>
    [SystemTextJsonConverter(typeof(CookieContainerConverter))]
    public CookieContainer Container { get; set; } = new();

    /// <summary>
    /// 二进制数据的 <see cref="Container"/>
    /// </summary>
    [IgnoreDataMember]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
    [NewtonsoftJsonIgnore]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public byte[] BinaryContainer
    {
        get => Container.ToByteArray();
        set
        {
            if (value == null)
                return;
            using var state = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
            var reader = new MemoryPackReader(value.AsSpan(), state);
            IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
            CookieContainer? container = null;
            f.Deserialize(ref reader, ref container);
            Container = container ?? new();
        }
    }

    HttpMessageHandler? httpMessageHandler;
    HttpClient? httpClient;
    bool disposedValue;

    /// <inheritdoc cref="HttpMessageHandler"/>
    [IgnoreDataMember]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
    [NewtonsoftJsonIgnore]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public HttpMessageHandler Handler
    {
        get
        {
            if (httpMessageHandler == null)
            {
                httpMessageHandler = IClientHttpClientFactory.CreateHandler(useCookies: true);
            }
            return httpMessageHandler;
        }
        set => httpMessageHandler = value;
    }

    /// <inheritdoc cref="HttpClient"/>
    [IgnoreDataMember]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
    [NewtonsoftJsonIgnore]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public HttpClient Client
    {
        get
        {
            if (httpClient == null)
            {
                httpClient = IClientHttpClientFactory.CreateClient(Handler);
            }
            return httpClient;
        }
        set => httpClient = value;
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                httpClient?.Dispose();
                httpMessageHandler?.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            httpClient = null;
            httpMessageHandler = null;
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
}
