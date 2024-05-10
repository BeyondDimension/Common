namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// WebApiClient 基类服务
/// <para>注意：继承此类需要实现 <see cref="CreateClient"/></para>
/// <para>对于使用 <see cref="IClientHttpClientFactory"/> 工厂构建 <see cref="HttpClient"/> 的服务基类应使用 <see cref="WebApiClientFactoryService"/></para>
/// </summary>
/// <param name="logger"></param>
/// <param name="httpPlatformHelper"></param>
/// <param name="newtonsoftJsonSerializer">如果需要使用 <see cref="Newtonsoft.Json"/> 则需要传递自定义实例或通过直接 new()，否则应保持为 <see langword="null"/></param>
public abstract partial class WebApiClientService(
    ILogger logger,
    IHttpPlatformHelperService? httpPlatformHelper,
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = null) : SerializableService(logger, newtonsoftJsonSerializer)
{
    /// <inheritdoc cref="IHttpPlatformHelperService"/>
    protected readonly IHttpPlatformHelperService? httpPlatformHelper = httpPlatformHelper;

    /// <inheritdoc cref="IHttpPlatformHelperService.UserAgent"/>
    internal virtual string? UserAgent => httpPlatformHelper?.UserAgent;

    /// <inheritdoc cref="WebApiClientSendArgs.Accept"/>
    protected virtual string Accept => MediaTypeNames.JSON;

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
    public virtual async Task<TResponseBody?> SendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
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
    public virtual TResponseBody? Send<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
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
    public virtual async Task<TResponseBody?> SendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
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
        var result = await SendCoreAsync<TResponseBody, TRequestBody>(client, args, requestBody, cancellationToken);
        if (result is ApiRspBase apiRspBase)
        {
            apiRspBase.Url = args.RequestUriString;
        }
        return result;
    }

    /// <summary>
    /// 以异步操作发送 HTTP 请求，仅响应正文，返回异步迭代器
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual IAsyncEnumerable<TResponseBody?> SendAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        var result = SendAsAsyncEnumerable<TResponseBody, nil>(args, default, cancellationToken);
        return result;
    }

    /// <summary>
    /// 以异步操作发送 HTTP 请求，响应正文与请求正文，返回异步迭代器
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual IAsyncEnumerable<TResponseBody?> SendAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TResponseBody : notnull
        where TRequestBody : notnull
    {
        if (args.VerifyRequestUri)
        {
            if (args.RequestUri.IsAbsoluteUri)
            {
                if (!String2.IsHttpUrl(args.RequestUriString))
                {
                    return default(EmptyAsyncEnumerable<TResponseBody?>);
                }
            }
        }

        var client = args.GetHttpClient() ?? CreateClient();
        return _SendCoreAsAsyncEnumerable();

        async IAsyncEnumerable<TResponseBody?> _SendCoreAsAsyncEnumerable()
        {
            var result = await SendCoreAsAsyncEnumerable<TResponseBody, TRequestBody>(client, args, requestBody, cancellationToken);

            await using var enumerator = result.GetAsyncEnumerator(cancellationToken);
            TResponseBody? item = default!;
            bool hasItem = true;
            while (hasItem)
            {
                try
                {
                    hasItem = await enumerator.MoveNextAsync().ConfigureAwait(false);
                    if (hasItem)
                    {
                        item = enumerator.Current;
                    }
                    else
                    {
                        result = default;
                    }
                }
                catch (Exception e)
                {
                    if (result is HttpResponseMessageContentAsyncEnumerable<TResponseBody> hrmcae)
                    {
                        item = OnSerializerError<TResponseBody>(e,
                            isSerializeOrDeserialize: false,
                            typeof(TResponseBody),
                            hrmcae.Response.IsSuccessStatusCode,
                            hrmcae.Response.StatusCode,
                            showLog: false);
                    }
                    else
                    {
                        item = OnError<TResponseBody>(e, args);
                    }
                    if (EqualityComparer<TResponseBody>.Default
                        .Equals(item, default))
                    {
                        break;
                    }
                    hasItem = true;
                }
                if (hasItem)
                {
                    if (item is ApiRspBase apiRspBase)
                    {
                        apiRspBase.Url = args.RequestUriString;
                    }
                    yield return item;
                }
            }
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
    public virtual TResponseBody? Send<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default) where TResponseBody : notnull where TRequestBody : notnull
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
        var result = SendCore<TResponseBody, TRequestBody>(client, args, requestBody, cancellationToken);
        if (result is ApiRspBase apiRspBase)
        {
            apiRspBase.Url = args.RequestUriString;
        }
        return result;
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
    public virtual async Task<HttpStatusCode> SendFromStatusCodeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TRequestBody : notnull
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
    public virtual async Task<HttpStatusCode> SendFromStatusCodeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default) where TRequestBody : notnull
    {
        var result = await SendAsync<HttpStatusCode, TRequestBody>(args, requestBody, cancellationToken);
        return result;
    }

    #endregion

    #region SendCoreX

    /// <summary>
    /// 可重写拦截自定义处理响应
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="response"></param>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<(bool isIntercept, TResponseBody? responseBody)> HandleHttpResponseMessage<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(
        HttpResponseMessage response,
        WebApiClientSendArgs args,
        TRequestBody requestBody,
        CancellationToken cancellationToken = default)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        await Task.CompletedTask;
        return (false, default);
    }

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
    protected virtual async Task<TResponseBody?> SendCoreAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(HttpClient client, WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        var disposeResponseMessage = true;
        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            requestMessage = await args.GetHttpRequestMessage(this, cancellationToken);
            if (typeof(TRequestBody) != typeof(nil) && requestMessage.Content == null)
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
            args.StatusCode = responseMessage.StatusCode;

            (bool isIntercept, TResponseBody? responseBody) = await HandleHttpResponseMessage<TResponseBody, TRequestBody>(responseMessage, args, requestBody, cancellationToken);
            if (isIntercept)
            {
                return responseBody;
            }

            var responseContentType = typeof(TResponseBody);
            if (responseContentType == typeof(HttpStatusCode))
            {
                var result = Convert2.Convert<TResponseBody, HttpStatusCode>(responseMessage.StatusCode);
                return result;
            }
            if (responseContentType == typeof(nil))
            {
                return default;
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
                else if (responseContentType == typeof(Stream) || responseContentType == typeof(HttpResponseMessageContentStream))
                {
                    disposeResponseMessage = false;
                    var result = await HttpResponseMessageContentStream.ReadAsStreamAsync(responseMessage, cancellationToken);
                    return (TResponseBody)(object)result;
                }
                else if (responseContentType == typeof(HttpResponseMessage))
                {
                    disposeResponseMessage = false;
                    return (TResponseBody)(object)responseMessage;
                }
                TResponseBody? deserializeResult = default;
                try
                {
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
                                        responseMessage.Content, cancellationToken);
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
                        case MediaTypeNames.MessagePack:
                            deserializeResult = await ReadFromMessagePackAsync<TResponseBody>(
                                responseMessage.Content, cancellationToken: cancellationToken);
                            return deserializeResult;
                        case MediaTypeNames.MemoryPack:
                            deserializeResult = await ReadFromMemoryPackAsync<TResponseBody>(
                                responseMessage.Content, cancellationToken: cancellationToken);
                            return deserializeResult;
                        default:
                            deserializeResult = await ReadFromCustomDeserializeAsync<TResponseBody>(
                                args, responseMessage, mime, cancellationToken: cancellationToken);
                            return deserializeResult;
                    }
#pragma warning restore CS0618 // 类型或成员已过时
                }
                catch (Exception ex)
                {
                    deserializeResult = OnSerializerError<TResponseBody>(
                        ex,
                        isSerializeOrDeserialize: false,
                        typeof(TResponseBody),
                        responseMessage.IsSuccessStatusCode,
                        responseMessage.StatusCode);
                }
                return deserializeResult;
            }
            return default;
        }
        catch (Exception e)
        {
            var errorResult = OnError<TResponseBody>(e, args);
            return errorResult;
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
    /// 以异步操作发送 HTTP 请求，返回异步迭代器
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
    protected virtual async Task<IAsyncEnumerable<TResponseBody?>> SendCoreAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(HttpClient client, WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        var disposeResponseMessage = true;
        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            requestMessage = await args.GetHttpRequestMessage(this, cancellationToken);
            if (typeof(TRequestBody) != typeof(nil) && requestMessage.Content == null)
            {
                var requestContent = GetRequestContent<TResponseBody, TRequestBody>(args, requestBody, cancellationToken);
                if (requestContent.Value is not null)
                {
                    return EmptyAsyncEnumerable<TResponseBody>.ToAsyncEnumerable(requestContent.Value);
                }
                requestMessage.Content = requestContent.Content;
            }

            HttpClientExtensions.UseDefault(client, requestMessage);
            responseMessage = await client.SendAsync(requestMessage,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            args.StatusCode = responseMessage.StatusCode;

            var responseContentType = typeof(TResponseBody);
            if (responseContentType == typeof(HttpStatusCode))
            {
                var result = Convert2.Convert<TResponseBody, HttpStatusCode>(responseMessage.StatusCode);
                return EmptyAsyncEnumerable<TResponseBody>.ToAsyncEnumerable(result);
            }
            if (responseContentType == typeof(nil))
            {
                return default(EmptyAsyncEnumerable<TResponseBody?>);
            }
            if (responseMessage.Content != null)
            {
                IAsyncEnumerable<TResponseBody?> deserializeResult = default(EmptyAsyncEnumerable<TResponseBody>);
                var mime = responseMessage.Content.Headers.ContentType?.MediaType;
                if (string.IsNullOrWhiteSpace(mime))
                    mime = args.Accept;
                if (string.IsNullOrWhiteSpace(mime))
                    mime = Accept;
                switch (mime)
                {
                    case MediaTypeNames.JSON:
                        switch (args.JsonImplType)
                        {
                            case Serializable.JsonImplType.SystemTextJson:
                                deserializeResult = ReadFromSJsonAsAsyncEnumerable<TResponseBody>(
                                    responseMessage.Content, cancellationToken);
                                break;
                            default:
                                throw ThrowHelper.GetArgumentOutOfRangeException(args.JsonImplType);
                        }
                        break;
                    default:
                        throw ThrowHelper.GetArgumentOutOfRangeException(mime);
                        //deserializeResult = await ReadFromCustomDeserializeAsync<TResponseBody>(
                        //    responseMessage, mime, cancellationToken: cancellationToken);
                        //return deserializeResult;
                }
                return HttpResponseMessageContentAsyncEnumerable<TResponseBody>.Parse(deserializeResult, responseMessage, ref disposeResponseMessage);
            }
            return default(EmptyAsyncEnumerable<TResponseBody?>);
        }
        catch (Exception e)
        {
            var errorResult = OnError<TResponseBody>(e, args);
            return errorResult is null ?
                default(EmptyAsyncEnumerable<TResponseBody?>) :
                EmptyAsyncEnumerable<TResponseBody>.ToAsyncEnumerable(errorResult);
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
    protected virtual TResponseBody? SendCore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(HttpClient client, WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        var disposeResponseMessage = true;
        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            requestMessage = args.GetHttpRequestMessage(this, cancellationToken).GetAwaiter().GetResult();
            if (typeof(TRequestBody) != typeof(nil) && requestMessage.Content == null)
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
            args.StatusCode = responseMessage.StatusCode;

            (bool isIntercept, TResponseBody? responseBody) = HandleHttpResponseMessage<TResponseBody, TRequestBody>(responseMessage, args, requestBody, cancellationToken).GetAwaiter().GetResult();
            if (isIntercept)
            {
                return responseBody;
            }

            var responseContentType = typeof(TResponseBody);
            if (responseContentType == typeof(HttpStatusCode))
            {
                var result = Convert2.Convert<TResponseBody, HttpStatusCode>(responseMessage.StatusCode);
                return result;
            }
            if (responseContentType == typeof(nil))
            {
                return default;
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
                else if (responseContentType == typeof(Stream) || responseContentType == typeof(HttpResponseMessageContentStream))
                {
                    disposeResponseMessage = false;
                    var result = HttpResponseMessageContentStream.ReadAsStream(responseMessage, cancellationToken);
                    return (TResponseBody)(object)result;
                }
                else if (responseContentType == typeof(HttpResponseMessage))
                {
                    disposeResponseMessage = false;
                    return (TResponseBody)(object)responseMessage;
                }
                TResponseBody? deserializeResult = default;
                try
                {
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
                                    break;
                                case Serializable.JsonImplType.SystemTextJson:
                                    deserializeResult = ReadFromSJson<TResponseBody>(
                                        responseMessage.Content, cancellationToken);
                                    break;
                                default:
                                    throw ThrowHelper.GetArgumentOutOfRangeException(args.JsonImplType);
                            }
                            break;
                        case MediaTypeNames.XML:
                        case MediaTypeNames.XML_APP:
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                            deserializeResult = ReadFromXml<TResponseBody>(
                                responseMessage.Content, cancellationToken: cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                            break;
                        default:
                            deserializeResult = ReadFromCustomDeserialize<TResponseBody>(
                                responseMessage, mime, cancellationToken: cancellationToken);
                            break;
                    }
#pragma warning restore CS0618 // 类型或成员已过时
                }
                catch (Exception ex)
                {
                    deserializeResult = OnSerializerError<TResponseBody>(
                        ex,
                        isSerializeOrDeserialize: false,
                        typeof(TResponseBody),
                        responseMessage.IsSuccessStatusCode,
                        responseMessage.StatusCode);
                }
                return deserializeResult;
            }
            return default;
        }
        catch (Exception e)
        {
            var errorResult = OnError<TResponseBody>(e, args);
            return errorResult;
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

    #endregion

    #endregion
}
