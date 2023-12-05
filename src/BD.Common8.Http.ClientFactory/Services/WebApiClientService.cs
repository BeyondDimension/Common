namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// WebApiClient 基类服务，实现序列化相关，统一使用方式
/// <para>继承此类需要实现 <see cref="CreateClient"/></para>
/// </summary>
/// <param name="logger"></param>
/// <param name="httpPlatformHelper"></param>
/// <param name="newtonsoftJsonSerializer"></param>
public abstract partial class WebApiClientService(
    ILogger logger,
    IHttpPlatformHelperService? httpPlatformHelper,
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = null) : Log.I
{
#pragma warning disable SA1600 // Elements should be documented
    protected const string Obsolete_GetNJsonContent = "无特殊情况下应使用 GetSJsonContent，即 System.Text.Json";
    protected const string Obsolete_ReadFromNJsonAsync = "无特殊情况下应使用 ReadFromSJsonAsync，即 System.Text.Json";
    protected const string Obsolete_UseAsync = "无特殊情况下应使用 Async 异步的函数版本";
#pragma warning restore SA1600 // Elements should be documented

    /// <inheritdoc cref="IHttpPlatformHelperService"/>
    protected readonly IHttpPlatformHelperService? httpPlatformHelper = httpPlatformHelper;

    /// <inheritdoc cref="IHttpPlatformHelperService.UserAgent"/>
    internal virtual string? UserAgent => httpPlatformHelper?.UserAgent;

    /// <inheritdoc cref="WebApiClientSendArgs.Accept"/>
    protected virtual string Accept => MediaTypeNames.JSON;

    /// <inheritdoc cref="ILogger"/>
    protected readonly ILogger logger = logger;

    /// <inheritdoc/>
    ILogger Log.I.Logger => logger;

    /// <summary>
    /// 使用的默认文本编码，默认值为 <see cref="Encoding.UTF8"/>
    /// </summary>
    protected virtual Encoding DefaultEncoding => Encoding.UTF8;

    #region Json

    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = newtonsoftJsonSerializer;

    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    protected NewtonsoftJsonSerializer NewtonsoftJsonSerializer => newtonsoftJsonSerializer ??= new();

    /// <summary>
    /// 序列化是否必须使用 <see cref="SystemTextJsonSerializerContext"/>，即源生成的类型信息数据，避免运行时反射
    /// </summary>
    protected virtual bool RequiredJsonSerializerContext => true;

    /// <summary>
    /// 用于序列化的类型信息，由 Json 源生成
    /// </summary>
    protected virtual SystemTextJsonSerializerContext? JsonSerializerContext { get; }

    #endregion

    /// <summary>
    /// 当序列化出现错误时
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ex"></param>
    /// <param name="isSerializeOrDeserialize">是序列化还是反序列化</param>
    /// <param name="modelType">模型类型</param>
    protected virtual T? OnSerializerError<T>(Exception ex,
        bool isSerializeOrDeserialize,
        Type modelType) where T : notnull
    {
        // 记录错误时，不需要带上 requestUrl 等敏感信息
        if (isSerializeOrDeserialize)
        {
            logger.LogError(ex,
                "Error serializing request model class. (Parameter '{type}')",
                modelType);
        }
        else
        {
            logger.LogError(ex,
                "Error reading and deserializing the response content into an instance. (Parameter '{type}')",
                modelType);
        }
        return default;
    }

    #region Send

    /// <inheritdoc cref="IClientHttpClientFactory.CreateClient(string, HttpHandlerCategory)"/>
    protected abstract HttpClient CreateClient();

    #region TResponseBody

    /// <summary>
    /// 以异步操作发送 HTTP 请求，仅响应正文
    /// <para>响应正文泛型支持</para>
    /// <list type="bullet">
    /// <item><see cref="nil"/></item>
    /// <item><see cref="HttpStatusCode"/></item>
    /// <item><see cref="string"/></item>
    /// <item><see cref="byte"/> 数组</item>
    /// <item><see cref="Stream"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<TResponseBody?> SendAsync<TResponseBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        var result = await SendAsync<TResponseBody, nil>(args, default, cancellationToken);
        return result;
    }

    /// <summary>
    /// 以同步操作发送 HTTP 请求，仅响应正文
    /// <para>响应正文泛型支持</para>
    /// <list type="bullet">
    /// <item><see cref="nil"/></item>
    /// <item><see cref="HttpStatusCode"/></item>
    /// <item><see cref="string"/></item>
    /// <item><see cref="byte"/> 数组</item>
    /// <item><see cref="Stream"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(Obsolete_UseAsync)]
    public virtual TResponseBody? Send<TResponseBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        var result = Send<TResponseBody, nil>(args, default, cancellationToken);
        return result;
    }

    /// <summary>
    /// 以异步操作发送 HTTP 请求，响应正文与请求正文
    /// <list type="bullet">
    /// <item><see cref="nil"/></item>
    /// <item><see cref="HttpStatusCode"/></item>
    /// <item><see cref="string"/></item>
    /// <item><see cref="byte"/> 数组</item>
    /// <item><see cref="Stream"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<TResponseBody?> SendAsync<TResponseBody, TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TResponseBody : notnull
        where TRequestBody : notnull
    {
        if (args.VerifyRequestUri)
        {
            if (args.RequestUri.IsAbsoluteUri)
            {
                if (!String2.IsHttpUrl(args.RequestUriString))
                {
                    return default;
                }
            }
        }

        var client = args.GetHttpClient() ?? CreateClient();
        var isPolly = args.NumRetries > 0;
        if (isPolly)
        {
            var result = await Policy.HandleResult<SendResultWrapper<TResponseBody>>(
                    static x => x.Value == null && !x.IsStopped)
                .WaitAndRetryAsync(args.NumRetries, args.PollyRetryAttempt)
                .ExecuteAsync(_SendCoreAsync, cancellationToken);
            return result.Value;
            async Task<SendResultWrapper<TResponseBody>> _SendCoreAsync(CancellationToken cancellationToken)
            {
                var result = await SendCoreAsync<TResponseBody, TRequestBody>(client, args, requestBody, cancellationToken);
                return result;
            }
        }
        else
        {
            var result = await SendCoreAsync<TResponseBody, TRequestBody>(client, args, requestBody, cancellationToken);
            return result.Value;
        }
    }

    /// <summary>
    /// 以同步操作发送 HTTP 请求，响应正文与请求正文
    /// <list type="bullet">
    /// <item><see cref="nil"/></item>
    /// <item><see cref="HttpStatusCode"/></item>
    /// <item><see cref="string"/></item>
    /// <item><see cref="byte"/> 数组</item>
    /// <item><see cref="Stream"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(Obsolete_UseAsync)]
    public virtual TResponseBody? Send<TResponseBody, TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default) where TResponseBody : notnull where TRequestBody : notnull
    {
        if (args.VerifyRequestUri)
        {
            if (args.RequestUri.IsAbsoluteUri)
            {
                if (!String2.IsHttpUrl(args.RequestUriString))
                {
                    return default;
                }
            }
        }

        var client = args.GetHttpClient() ?? CreateClient();
        var isPolly = args.NumRetries > 0;
        if (isPolly)
        {
            var result = Policy.HandleResult<SendResultWrapper<TResponseBody>>(
                    static x => x.Value == null && !x.IsStopped)
                .WaitAndRetry(args.NumRetries, args.PollyRetryAttempt)
                .Execute(_SendCore, cancellationToken);
            return result.Value;
            SendResultWrapper<TResponseBody> _SendCore(CancellationToken cancellationToken)
            {
                var result = SendCore<TResponseBody, TRequestBody>(client, args, requestBody, cancellationToken);
                return result;
            }
        }
        else
        {
            var result = SendCore<TResponseBody, TRequestBody>(client, args, requestBody, cancellationToken);
            return result.Value;
        }
    }

    #endregion

    #region HttpStatusCode

    /// <summary>
    /// 以异步操作发送 HTTP 请求，获取 HTTP 状态码
    /// </summary>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<HttpStatusCode> SendFromStatusCodeAsync<TRequestBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TRequestBody : notnull
    {
        var result = await SendAsync<HttpStatusCode>(args, cancellationToken);
        return result;
    }

    /// <summary>
    /// 以异步操作发送 HTTP 请求，获取 HTTP 状态码，仅请求正文
    /// </summary>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<HttpStatusCode> SendFromStatusCodeAsync<TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default) where TRequestBody : notnull
    {
        var result = await SendAsync<HttpStatusCode, TRequestBody>(args, requestBody, cancellationToken);
        return result;
    }

    #endregion

    /// <summary>
    /// 以异步操作发送 HTTP 请求
    /// <list type="bullet">
    /// <item><see cref="nil"/></item>
    /// <item><see cref="HttpStatusCode"/></item>
    /// <item><see cref="string"/></item>
    /// <item><see cref="byte"/> 数组</item>
    /// <item><see cref="Stream"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="client"></param>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<SendResultWrapper<TResponseBody>> SendCoreAsync<TResponseBody, TRequestBody>(HttpClient client, WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        var disposeResponseMessage = true;
        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            requestMessage = args.GetHttpRequestMessage(this, cancellationToken);
            if (requestMessage.Content == null)
            {
                var requestContent = GetRequestContent<TResponseBody, TRequestBody>(args, requestBody, cancellationToken);
                if (requestContent.Value is not null)
                {
                    return requestContent.Value;
                }
                requestMessage.Content = requestContent.Content;
            }

            HttpClientExtensions.UseDefault(client, requestMessage);
            responseMessage = await client.SendAsync(requestMessage,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var responseContentType = typeof(TResponseBody);
            if (responseContentType == typeof(HttpStatusCode))
            {
                var result = Convert2.Convert<TResponseBody, HttpStatusCode>(responseMessage.StatusCode);
                return result;
            }
            if (responseContentType == typeof(nil))
            {
                return true;
            }
            if (responseMessage.Content != null)
            {
                if (responseContentType == typeof(string))
                {
                    var result = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
                    return (TResponseBody)(object)result;
                }
                else if (responseContentType == typeof(byte[]))
                {
                    var result = await responseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
                    return (TResponseBody)(object)result;
                }
                else if (responseContentType == typeof(Stream))
                {
                    disposeResponseMessage = false;
                    var result = await HttpResponseMessageContentStream.ReadAsStreamAsync(responseMessage, cancellationToken);
                    return (TResponseBody)(object)result;
                }
                TResponseBody? deserializeResult = default;
                var mime = responseMessage.Content.Headers.ContentType?.MediaType;
                if (string.IsNullOrWhiteSpace(mime))
                    mime = args.Accept;
                if (string.IsNullOrWhiteSpace(mime))
                    mime = Accept;
#pragma warning disable CS0618 // 类型或成员已过时
                switch (mime)
                {
                    case MediaTypeNames.JSON:
                        switch (args.JsonImplType)
                        {
                            case Serializable.JsonImplType.NewtonsoftJson:
                                deserializeResult = await ReadFromNJsonAsync<TResponseBody>(
                                    responseMessage.Content, cancellationToken: cancellationToken);
                                return deserializeResult;
                            case Serializable.JsonImplType.SystemTextJson:
                                deserializeResult = await ReadFromSJsonAsync<TResponseBody>(
                                    responseMessage.Content, null, cancellationToken);
                                return deserializeResult;
                            default:
                                throw ThrowHelper.GetArgumentOutOfRangeException(args.JsonImplType);
                        }
                    case MediaTypeNames.XML:
                    case MediaTypeNames.XML_APP:
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                        deserializeResult = await ReadFromXmlAsync<TResponseBody>(
                            responseMessage.Content, cancellationToken: cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                        return deserializeResult;
                    default:
                        deserializeResult = await ReadFromCustomDeserializeAsync<TResponseBody>(
                            responseMessage, mime, cancellationToken: cancellationToken);
                        return deserializeResult;
                }
#pragma warning restore CS0618 // 类型或成员已过时
            }
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e,
                $"{nameof(SendCoreAsync)} fail, method: {{method}}, requestUrl: {{requestUrl}}.", args.Method, args.RequestUriString);
            return false; // 可重试
        }
        finally
        {
            requestMessage?.Dispose();
            if (disposeResponseMessage)
            {
                responseMessage?.Dispose();
            }
        }
    }

    /// <summary>
    /// 以同步操作发送 HTTP 请求
    /// <list type="bullet">
    /// <item><see cref="nil"/></item>
    /// <item><see cref="HttpStatusCode"/></item>
    /// <item><see cref="string"/></item>
    /// <item><see cref="byte"/> 数组</item>
    /// <item><see cref="Stream"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="client"></param>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual SendResultWrapper<TResponseBody> SendCore<TResponseBody, TRequestBody>(HttpClient client, WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        var disposeResponseMessage = true;
        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            requestMessage = args.GetHttpRequestMessage(this, cancellationToken);
            if (requestMessage.Content == null)
            {
                var requestContent = GetRequestContent<TResponseBody, TRequestBody>(args, requestBody, cancellationToken);
                if (requestContent.Value is not null)
                {
                    return requestContent.Value;
                }
                requestMessage.Content = requestContent.Content;
            }

            HttpClientExtensions.UseDefault(client, requestMessage);
            responseMessage = client.Send(requestMessage,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var responseContentType = typeof(TResponseBody);
            if (responseContentType == typeof(HttpStatusCode))
            {
                var result = Convert2.Convert<TResponseBody, HttpStatusCode>(responseMessage.StatusCode);
                return result;
            }
            if (responseContentType == typeof(nil))
            {
                return true;
            }
            if (responseMessage.Content != null)
            {
                if (responseContentType == typeof(string))
                {
                    using var contentReadStream = responseMessage.Content.ReadAsStream(cancellationToken);
                    var contentReadByteArray = contentReadStream.ToByteArray();
                    var encoding = DefaultEncoding;
                    var result = encoding.GetString(contentReadByteArray);
                    return (TResponseBody)(object)result;
                }
                else if (responseContentType == typeof(byte[]))
                {
                    using var contentReadStream = responseMessage.Content.ReadAsStream(cancellationToken);
                    var result = contentReadStream.ToByteArray();
                    return (TResponseBody)(object)result;
                }
                else if (responseContentType == typeof(Stream))
                {
                    disposeResponseMessage = false;
                    var result = HttpResponseMessageContentStream.ReadAsStream(responseMessage, cancellationToken);
                    return (TResponseBody)(object)result;
                }
                TResponseBody? deserializeResult = default;
                var mime = responseMessage.Content.Headers.ContentType?.MediaType ?? args.Accept;
#pragma warning disable CS0618 // 类型或成员已过时
                switch (mime)
                {
                    case MediaTypeNames.JSON:
                        switch (args.JsonImplType)
                        {
                            case Serializable.JsonImplType.NewtonsoftJson:
                                deserializeResult = ReadFromNJson<TResponseBody>(
                                    responseMessage.Content, cancellationToken: cancellationToken);
                                return deserializeResult;
                            case Serializable.JsonImplType.SystemTextJson:
                                deserializeResult = ReadFromSJson<TResponseBody>(
                                    responseMessage.Content, null, cancellationToken);
                                return deserializeResult;
                            default:
                                throw ThrowHelper.GetArgumentOutOfRangeException(args.JsonImplType);
                        }
                    case MediaTypeNames.XML:
                    case MediaTypeNames.XML_APP:
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                        deserializeResult = ReadFromXml<TResponseBody>(
                            responseMessage.Content, cancellationToken: cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                        return deserializeResult;
                    default:
                        deserializeResult = ReadFromCustomDeserialize<TResponseBody>(
                            responseMessage, mime, cancellationToken: cancellationToken);
                        return deserializeResult;
                }
#pragma warning restore CS0618 // 类型或成员已过时
            }
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e,
                $"{nameof(SendCore)} fail, method: {{method}}, requestUrl: {{requestUrl}}.", args.Method, args.RequestUriString);
            return false; // 可重试
        }
        finally
        {
            requestMessage?.Dispose();
            if (disposeResponseMessage)
            {
                responseMessage?.Dispose();
            }
        }
    }

    /// <summary>
    /// 将模型类转换为 HTTP 请求内容，支持以下类型
    /// <list type="bullet">
    /// <item>application/json</item>
    /// <item>text/xml</item>
    /// <item>application/x-www-form-urlencoded</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual HttpContentWrapper<TResponseBody> GetRequestContent<TResponseBody, TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        if (typeof(TRequestBody) == typeof(nil))
            return default;
        var mime = args.Accept;
        if (string.IsNullOrWhiteSpace(mime))
            mime = Accept;
        HttpContentWrapper<TResponseBody> result;
#pragma warning disable CS0618 // 类型或成员已过时
        switch (mime)
        {
            case MediaTypeNames.JSON:
                switch (args.JsonImplType)
                {
                    case Serializable.JsonImplType.NewtonsoftJson:
                        result = GetNJsonContent<TResponseBody, TRequestBody>(requestBody);
                        return result;
                    case Serializable.JsonImplType.SystemTextJson:
                        result = GetSJsonContent<TResponseBody, TRequestBody>(requestBody);
                        return result;
                    default:
                        throw ThrowHelper.GetArgumentOutOfRangeException(args.JsonImplType);
                }
            case MediaTypeNames.XML:
            case MediaTypeNames.XML_APP:
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                result = GetXmlContent<TResponseBody, TRequestBody>(requestBody);
                return result;
            case MediaTypeNames.FormUrlEncoded:
                if (requestBody is not IEnumerable<KeyValuePair<string?, string?>> nameValueCollection)
                    throw new NotSupportedException("requestBody is not IEnumerable<KeyValuePair<string?, string?>> nameValueCollection.");
                result = new FormUrlEncodedContent(nameValueCollection);
                return result;
            default:
                result = GetCustomSerializeContent<TResponseBody, TRequestBody>(args, requestBody, mime, cancellationToken);
                return result;
        }
#pragma warning restore CS0618 // 类型或成员已过时
    }

    /// <summary>
    /// 可重写自定义其他 MIME 的序列化
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="mime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual HttpContentWrapper<TResponseBody> GetCustomSerializeContent<TResponseBody, TRequestBody>(
        WebApiClientSendArgs args,
        TRequestBody requestBody,
        string? mime,
        CancellationToken cancellationToken)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        throw ThrowHelper.GetArgumentOutOfRangeException(mime);
    }

    /// <summary>
    /// 可重写自定义其他 MIME 的反序列化
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="mime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<TResponseBody> ReadFromCustomDeserializeAsync<TResponseBody>(
        HttpResponseMessage responseMessage,
        string? mime,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        throw ThrowHelper.GetArgumentOutOfRangeException(mime);
    }

    /// <summary>
    /// 可重写自定义其他 MIME 的反序列化
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="mime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual TResponseBody ReadFromCustomDeserialize<TResponseBody>(
        HttpResponseMessage responseMessage,
        string? mime,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        throw ThrowHelper.GetArgumentOutOfRangeException(mime);
    }

    /// <summary>
    /// 发送 HTTP 请求结果包装类型
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    protected readonly record struct SendResultWrapper<TResponseBody> where TResponseBody : notnull
    {
        /// <summary>
        /// 发送请求响应的结果值
        /// </summary>
        public TResponseBody? Value { get; init; }

        /// <summary>
        /// 请求是否中止，比如取消，停止重试等
        /// </summary>
        public required bool IsStopped { get; init; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SendResultWrapper<TResponseBody>(bool isStopped) => isStopped ? new()
        {
            IsStopped = true,
        } : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SendResultWrapper<TResponseBody>(TResponseBody? value) => new()
        {
            IsStopped = true,
            Value = value,
        };
    }

    protected readonly record struct HttpContentWrapper<TResponseBody> where TResponseBody : notnull
    {
        /// <summary>
        /// 发送请求响应的结果值
        /// </summary>
        public TResponseBody? Value { get; init; }

        /// <inheritdoc cref="HttpContent"/>
        public HttpContent? Content { get; init; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HttpContentWrapper<TResponseBody>(HttpContent content) => new()
        {
            Content = content,
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HttpContentWrapper<TResponseBody>(TResponseBody? value) => new()
        {
            Value = value,
        };
    }

    #endregion
}
