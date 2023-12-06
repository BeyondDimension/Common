namespace BD.Common8.Http.ClientFactory.Models;

/// <summary>
/// 发送 HTTP 请求的参数，此类型不支持序列化
/// <para>用于 <see cref="WebApiClientService"/> 服务中的发送</para>
/// </summary>
public record class WebApiClientSendArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiClientSendArgs"/> class.
    /// </summary>
    /// <param name="requestUriString"></param>
    public WebApiClientSendArgs([StringSyntax(StringSyntaxAttribute.Uri)] string requestUriString)
    {
        RequestUriString = requestUriString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiClientSendArgs"/> class.
    /// </summary>
    /// <param name="requestUri"></param>
    public WebApiClientSendArgs(Uri requestUri)
    {
        RequestUri = requestUri;
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
    public virtual HttpRequestMessage GetHttpRequestMessage(WebApiClientService s, CancellationToken cancellationToken = default)
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

        ConfigureRequestMessage?.Invoke(httpRequestMessage, this, cancellationToken);

        return httpRequestMessage;
    }

    /// <summary>
    /// 自定义处理请求消息委托
    /// </summary>
    public Action<HttpRequestMessage, WebApiClientSendArgs, CancellationToken>? ConfigureRequestMessage { get; init; }

    /// <summary>
    /// 是否验证 RequestUri 是否为 Http 地址
    /// </summary>
    public bool VerifyRequestUri { get; init; } = true;

    /// <summary>
    /// Accept 请求头用来告知（服务器）客户端可以处理的内容类型，这种内容类型用 MIME 类型来表示。
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Accept</para>
    /// </summary>
    public string? Accept { get; init; }

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

    /// <inheritdoc cref="IsolatedCookieContainer"/>
    public IsolatedCookieContainer? IsolatedCookie { get; set; }

    /// <summary>
    /// 获取 <see cref="HttpClient"/>, 如果返回 <see langword="null"/> 将使用服务上的 <see cref="IClientHttpClientFactory.CreateClient(string, HttpHandlerCategory)"/>
    /// </summary>
    /// <returns></returns>
    public virtual HttpClient? GetHttpClient()
    {
        if (IsolatedCookie != null)
            return IsolatedCookie.Client;
        return null;
    }
}
