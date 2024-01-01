namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// WebApiClient 基类服务，实现序列化相关，统一使用方式
/// <para>继承此类需要实现 <see cref="CreateClient"/></para>
/// </summary>
/// <param name="logger"></param>
/// <param name="httpPlatformHelper"></param>
/// <param name="newtonsoftJsonSerializer">如果需要使用 <see cref="Newtonsoft.Json"/> 则需要传递自定义实例或通过直接 new()，否则应保持为 <see langword="null"/></param>
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
    /// 用于序列化的类型信息，由 Json 源生成，值指向 SystemTextJsonSerializerContext.Default.Options，由实现类重写
    /// </summary>
    protected virtual SystemTextJsonSerializerOptions JsonSerializerOptions
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        => SystemTextJsonSerializerOptions.Default;
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    #endregion

    /// <summary>
    /// 将响应正文泛型转换为 <see cref="ApiRspBase"/>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <returns></returns>
    protected virtual ApiRspBase? GetApiRspBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type typeResponseBody)
    {
        ApiRspBase? apiRspBase = null;
        if (typeResponseBody.IsGenericType)
        {
            var typeResponseBodyGenericDef = typeResponseBody.GetGenericTypeDefinition();
            if (typeResponseBodyGenericDef == typeof(ApiRspImpl<>))
            {
                apiRspBase = ((ApiRspBase)Activator.CreateInstance(typeResponseBody)!)!;
            }
        }
        else if (typeResponseBody == typeof(ApiRspImpl))
        {
            apiRspBase = new ApiRspImpl();
        }
        return apiRspBase;
    }

    /// <inheritdoc cref="OnSerializerError{TResponseBody}(Exception, bool, Type, bool?, HttpStatusCode?, bool)"/>
    protected virtual TResponseBody OnSerializerErrorReApiRspBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        Exception ex,
        bool isSerializeOrDeserialize,
        Type modelType) where TResponseBody : ApiRspBase
    {
        var errorResult = OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize, modelType);
        errorResult.ThrowIsNull(); // 泛型为 ApiRspBase 派生时不返回 null
        return errorResult;
    }

    /// <summary>
    /// 当序列化出现错误时
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="ex"></param>
    /// <param name="isSerializeOrDeserialize">是序列化还是反序列化</param>
    /// <param name="modelType">模型类型</param>
    /// <param name="isSuccessStatusCode"></param>
    /// <param name="statusCode"></param>
    /// <param name="showLog"></param>
    protected virtual TResponseBody? OnSerializerError<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        Exception ex,
        bool isSerializeOrDeserialize,
        Type modelType,
        bool? isSuccessStatusCode = null,
        HttpStatusCode? statusCode = null,
        bool showLog = true) where TResponseBody : notnull
    {
        // 记录错误时，不需要带上 requestUrl 等敏感信息
        string msg;
        if (isSerializeOrDeserialize)
        {
            msg = $"Error serializing request model class. (Parameter '{modelType}')";
        }
        else
        {
            msg = $"Error reading and deserializing the response content into an instance. (Parameter '{modelType}')";
        }

        try
        {
            var typeResponseBody = typeof(TResponseBody);
            if (typeResponseBody == typeof(nil))
                return default;

            var apiRspBase = GetApiRspBase<TResponseBody>(typeResponseBody);
            if (apiRspBase != null)
            {
                if (isSuccessStatusCode.HasValue && statusCode.HasValue)
                {
                    if (!isSuccessStatusCode.Value)
                    {
                        showLog = false;
                        apiRspBase.Code = (ApiRspCode)statusCode.Value;
                        apiRspBase.InternalMessage = apiRspBase.GetMessage();
                        return (TResponseBody?)(object)apiRspBase;
                    }
                }

                apiRspBase.Code = ApiRspCode.ClientDeserializeFail;
                apiRspBase.ClientException = ex;
                apiRspBase.InternalMessage = msg.Replace("{type}", modelType.ToString());
                return (TResponseBody?)(object)apiRspBase;
            }

            return default;
        }
        finally
        {
#pragma warning disable CA2254 // 模板应为静态表达式
            if (showLog)
            {
                logger.LogError(ex, msg);
            }
#pragma warning restore CA2254 // 模板应为静态表达式
        }
    }

    /// <summary>
    /// 是否启用日志当请求出现错误时
    /// </summary>
    protected virtual bool EnableLogOnError { get; } = true;

    /// <summary>
    /// 根据客户端 <see cref="Exception"/> 获取错误码
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    protected virtual ApiRspCode GetApiRspCodeByClientException(Exception ex)
    {
        if (ex is HttpRequestException && ex.InnerException is SocketException socketException)
        {
            if (socketException.SocketErrorCode == SocketError.ConnectionRefused)
            {
                // System.Net.Http.HttpRequestException: 由于目标计算机积极拒绝，无法连接。 (localhost:443)
                // ---> System.Net.Sockets.SocketException (10061): 由于目标计算机积极拒绝，无法连接。
                // at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.CreateException(SocketError error, Boolean forAsyncThrow)
                return ApiRspCode.Timeout;
            }
        }

        if (ex is TaskCanceledException && ex.InnerException is TimeoutException)
        {
            // System.Threading.Tasks.TaskCanceledException: The request was canceled due to the configured HttpClient.Timeout of 2.9 seconds elapsing.
            // ---> System.TimeoutException: A task was canceled.
            // ---> System.Threading.Tasks.TaskCanceledException: A task was canceled.
            // at System.Threading.Tasks.TaskCompletionSourceWithCancellation`1.WaitWithCancellationAsync(CancellationToken cancellationToken)
            // at System.Net.Http.HttpConnectionPool.SendWithVersionDetectionAndRetryAsync(HttpRequestMessage request, Boolean async, Boolean doRequestAuth, CancellationToken cancellationToken)
            return ApiRspCode.Timeout;
        }

        return ex.GetKnownType() switch
        {
            ExceptionKnownType.Canceled => ApiRspCode.Canceled,
            ExceptionKnownType.TaskCanceled => ApiRspCode.TaskCanceled,
            ExceptionKnownType.OperationCanceled => ApiRspCode.OperationCanceled,
            ExceptionKnownType.CertificateNotYetValid => ApiRspCode.CertificateNotYetValid,
            _ => ApiRspCode.ClientException,
        };
    }

    /// <inheritdoc cref="OnError{TResponseBody}(Exception, WebApiClientSendArgs, string)"/>
    protected virtual TResponseBody OnErrorReApiRspBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        Exception ex,
        WebApiClientSendArgs args,
        [CallerMemberName] string callerMemberName = "") where TResponseBody : ApiRspBase
    {
        var errorResult = OnError<TResponseBody>(ex, args, callerMemberName);
        errorResult.ThrowIsNull(); // 泛型为 ApiRspBase 派生时不返回 null

        return errorResult;
    }

    /// <summary>
    /// 当请求出现错误时
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="ex"></param>
    /// <param name="args"></param>
    /// <param name="callerMemberName"></param>
    /// <returns></returns>
    protected virtual TResponseBody? OnError<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        Exception ex,
        WebApiClientSendArgs args,
        [CallerMemberName] string callerMemberName = "") where TResponseBody : notnull
    {
        if (EnableLogOnError)
        {
            logger.LogError(ex,
                $"{{callerMemberName}} fail, method: {{method}}, requestUrl: {{requestUrl}}.",
                callerMemberName,
                args.Method,
                args.RequestUriString);
        }

        var typeResponseBody = typeof(TResponseBody);
        if (typeResponseBody == typeof(nil))
            return default;

        var apiRspBase = GetApiRspBase<TResponseBody>(typeResponseBody);
        if (apiRspBase != null)
        {
            apiRspBase.Code = GetApiRspCodeByClientException(ex);
            apiRspBase.ClientException = ex;
            apiRspBase.InternalMessage = apiRspBase.GetMessage();
            return (TResponseBody?)(object)apiRspBase;
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
                        OnSerializerError<TResponseBody>(e,
                            isSerializeOrDeserialize: false,
                            typeof(TResponseBody),
                            hrmcae.Response.IsSuccessStatusCode,
                            hrmcae.Response.StatusCode,
                            showLog: false);
                    }
                    else
                    {
                        OnError<TResponseBody>(e, args);
                    }
                    break;
                }
                if (hasItem)
                    yield return item;
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
            args.StatusCode = responseMessage.StatusCode;

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
                        default:
                            deserializeResult = await ReadFromCustomDeserializeAsync<TResponseBody>(
                                responseMessage, mime, cancellationToken: cancellationToken);
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
            requestMessage = args.GetHttpRequestMessage(this, cancellationToken);
            if (requestMessage.Content == null)
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
            args.StatusCode = responseMessage.StatusCode;

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
    protected virtual HttpContentWrapper<TResponseBody> GetRequestContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        if (typeof(TRequestBody) == typeof(nil))
            return default;
        var mime = args.ContentType;
        if (string.IsNullOrWhiteSpace(mime))
            mime = MediaTypeNames.JSON;
        try
        {
#pragma warning disable CS0618 // 类型或成员已过时
            HttpContentWrapper<TResponseBody> result;
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
        catch (Exception ex)
        {
            return OnSerializerError<TResponseBody>(ex,
                isSerializeOrDeserialize: true,
                typeof(TRequestBody));
        }
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
    protected virtual HttpContentWrapper<TResponseBody> GetCustomSerializeContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(
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
    protected virtual Task<TResponseBody> ReadFromCustomDeserializeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
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
    protected virtual TResponseBody ReadFromCustomDeserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        HttpResponseMessage responseMessage,
        string? mime,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        throw ThrowHelper.GetArgumentOutOfRangeException(mime);
    }

    protected readonly record struct HttpContentWrapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody> where TResponseBody : notnull
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
