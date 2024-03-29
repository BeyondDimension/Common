namespace System.Extensions;

/// <summary>
/// UseDefaultSendX 使用此扩展代替 Send，将 DefaultRequestVersion 与 DefaultVersionPolicy 赋值给 HttpRequestMessage
/// </summary>
public static partial class HttpClientExtensions
{
#if NETSTANDARD || NETFRAMEWORK
    /// <summary>
    /// 默认 Http 请求版本号
    /// </summary>
    static Version DefaultRequestVersion { get; set; } =
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        System.Net.HttpVersion.Version20;
#else
        new Version(2, 0);
#endif
#endif

    public static void UseDefault(HttpClient httpClient, HttpRequestMessage request)
    {
#if NETSTANDARD || NETFRAMEWORK
        request.Version = DefaultRequestVersion;
#else
        request.Version = httpClient.DefaultRequestVersion;
        request.VersionPolicy = httpClient.DefaultVersionPolicy;
#endif
    }

    /// <inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage)"/>
    public static Task<HttpResponseMessage> UseDefaultSendAsync(this HttpClient httpClient, HttpRequestMessage request)
    {
        UseDefault(httpClient, request);
        return httpClient.SendAsync(request);
    }

    /// <inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, HttpCompletionOption)"/>
    public static Task<HttpResponseMessage> UseDefaultSendAsync(this HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption)
    {
        UseDefault(httpClient, request);
        return httpClient.SendAsync(request, completionOption);
    }

    /// <inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, HttpCompletionOption, CancellationToken)"/>
    public static Task<HttpResponseMessage> UseDefaultSendAsync(this HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
        UseDefault(httpClient, request);
        return httpClient.SendAsync(request, completionOption, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)"/>
    public static Task<HttpResponseMessage> UseDefaultSendAsync(this HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        UseDefault(httpClient, request);
        return httpClient.SendAsync(request, cancellationToken);
    }

#if NET5_0_OR_GREATER
    /// <inheritdoc cref="HttpClient.Send(HttpRequestMessage)"/>
    public static HttpResponseMessage UseDefaultSend(this HttpClient httpClient, HttpRequestMessage request)
    {
        UseDefault(httpClient, request);
        return httpClient.Send(request);
    }

    /// <inheritdoc cref="HttpClient.Send(HttpRequestMessage, HttpCompletionOption)"/>
    public static HttpResponseMessage UseDefaultSend(this HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption)
    {
        UseDefault(httpClient, request);
        return httpClient.Send(request, completionOption);
    }

    /// <inheritdoc cref="HttpClient.Send(HttpRequestMessage, HttpCompletionOption, CancellationToken)"/>
    public static HttpResponseMessage UseDefaultSend(this HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
        UseDefault(httpClient, request);
        return httpClient.Send(request, completionOption, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.Send(HttpRequestMessage, CancellationToken)"/>
    public static HttpResponseMessage UseDefaultSend(this HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        UseDefault(httpClient, request);
        return httpClient.Send(request, cancellationToken);
    }
#endif
}