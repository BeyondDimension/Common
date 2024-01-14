namespace BD.Common8.Http.ClientFactory.Models;

/// <summary>
/// 发送 HTTP 请求的参数，此类型不支持序列化
/// <para>用于 <see cref="WebApiClientService"/> 服务中的发送</para>
/// </summary>
public record class WebApiClientSendArgs : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiClientSendArgs"/> class.
    /// </summary>
    /// <param name="requestUriString"></param>
    public WebApiClientSendArgs([StringSyntax(StringSyntaxAttribute.Uri)] string requestUriString)
    {
        _RequestUriString = requestUriString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiClientSendArgs"/> class.
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="requestUriString"></param>
    public WebApiClientSendArgs(Uri requestUri, [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUriString = null)
    {
        _RequestUri = requestUri;
        _RequestUriString = requestUriString;
    }

    string? _RequestUriString;
    Uri? _RequestUri;

    /// <summary>
    /// 请求地址，字符串
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public string RequestUriString
    {
        get
        {
            if (_RequestUriString != null)
                return _RequestUriString;
            if (RequestUri != null)
                return _RequestUriString = RequestUri.ToString();
            throw new ArgumentNullException(nameof(_RequestUriString));
        }
        internal set => _RequestUriString = value;
    }

    /// <summary>
    /// 请求地址，<see cref="Uri"/>
    /// </summary>
    public Uri RequestUri
    {
        get
        {
            if (_RequestUri != null)
                return _RequestUri;
            if (!string.IsNullOrWhiteSpace(_RequestUriString))
                return _RequestUri = new(_RequestUriString, UriKind.RelativeOrAbsolute);
            throw new ArgumentNullException(nameof(_RequestUri));
        }
        internal set => _RequestUri = value;
    }

    /// <summary>
    /// 请求方法，默认值为 <see cref="HttpMethod.Get"/>
    /// </summary>
    public virtual HttpMethod Method { get; init; } = HttpMethod.Get;

    /// <summary>
    /// 创建一个 <see cref="HttpRequestMessage"/>，请求消息实例经过 Send 后不可重发
    /// </summary>
    /// <param name="s"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<HttpRequestMessage> GetHttpRequestMessage(WebApiClientService s, CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = new(Method, RequestUri);

        if (!string.IsNullOrEmpty(Accept))
            httpRequestMessage.Headers.Accept.ParseAdd(Accept);

        if (!EmptyUserAgent)
        {
            var userAgent = UserAgent;
            if (string.IsNullOrEmpty(userAgent))
                userAgent = s.UserAgent;
            if (!string.IsNullOrEmpty(userAgent))
                httpRequestMessage.Headers.UserAgent.ParseAdd(userAgent);
        }

        if (ConfigureRequestMessage != null)
        {
            await ConfigureRequestMessage(httpRequestMessage, this, cancellationToken);
        }

        return httpRequestMessage;
    }

    /// <summary>
    /// 自定义处理请求消息委托
    /// </summary>
    public Func<HttpRequestMessage, WebApiClientSendArgs, CancellationToken, Task>? ConfigureRequestMessage { get; init; }

    /// <summary>
    /// 是否验证 RequestUri 是否为 Http 地址
    /// </summary>
    public bool VerifyRequestUri { get; init; } = true;

    /// <inheritdoc cref="Accept"/>
    protected string? mAccept;

    /// <summary>
    /// Accept 请求头用来告知（服务器）客户端可以处理的内容类型，这种内容类型用 MIME 类型来表示。
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Accept</para>
    /// </summary>
    public string? Accept { get => mAccept; init => mAccept = value; }

    /// <inheritdoc cref="ContentType"/>
    protected string? mContentType;

    /// <summary>
    /// Content-Type 实体头部用于指示资源的 MIME 类型 media type 。
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Content-Type</para>
    /// </summary>
    public string? ContentType { get => mContentType; init => mContentType = value; }

    /// <summary>
    /// 选择 Json 序列化的实现，默认值 <see cref="Serializable.JsonImplType.SystemTextJson"/>
    /// </summary>
    public Serializable.JsonImplType JsonImplType { get; init; } = Serializable.JsonImplType.SystemTextJson;

    /// <summary>
    /// 使用自定义 UserAgent 值
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// 是否使用空的 UserAgent 值
    /// </summary>
    public bool EmptyUserAgent { get; init; }

    HttpClient? client;
    bool disposedValue;

    /// <summary>
    /// 获取 <see cref="HttpClient"/>, 如果返回 <see langword="null"/> 将使用服务上的 <see cref="IClientHttpClientFactory.CreateClient(string, HttpHandlerCategory)"/>
    /// </summary>
    /// <returns></returns>
    public virtual HttpClient? GetHttpClient()
    {
        if (client != null)
            return client;

        return null;
    }

    /// <inheritdoc cref="HttpStatusCode"/>
    public HttpStatusCode StatusCode { get; set; }

    /// <inheritdoc cref="HttpResponseMessage.IsSuccessStatusCode"/>
    public bool IsSuccessStatusCode => ((int)StatusCode >= 200) && ((int)StatusCode <= 299);

    /// <summary>
    /// 设置用作发送的 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="httpClient"></param>
    public virtual void SetHttpClient(HttpClient httpClient) => client = httpClient;

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            client = null; // HttpClient 由工厂管理，此处释放仅取消引用
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
