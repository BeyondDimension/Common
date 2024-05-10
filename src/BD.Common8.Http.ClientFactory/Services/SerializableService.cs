namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// 序列化服务，提供统一的序列化处理
/// </summary>
/// <param name="logger"></param>
/// <param name="newtonsoftJsonSerializer">如果需要使用 <see cref="Newtonsoft.Json"/> 则需要传递自定义实例或通过直接 new()，否则应保持为 <see langword="null"/></param>
public abstract partial class SerializableService(
    ILogger logger,
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = null) : Log.I
{
    /// <summary>
    /// 无特殊情况下应使用 GetSJsonContent，即 System.Text.Json
    /// </summary>
    protected const string Obsolete_GetNJsonContent = "无特殊情况下应使用 GetSJsonContent，即 System.Text.Json";

    /// <summary>
    /// 无特殊情况下应使用 ReadFromSJsonAsync，即 System.Text.Json
    /// </summary>
    protected const string Obsolete_ReadFromNJsonAsync = "无特殊情况下应使用 ReadFromSJsonAsync，即 System.Text.Json";

    /// <summary>
    /// 无特殊情况下应使用 Async 异步的函数版本
    /// </summary>
    protected const string Obsolete_UseAsync = "无特殊情况下应使用 Async 异步的函数版本";

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

    SystemTextJsonSerializerOptions? _JsonSerializerOptions;

    /// <summary>
    /// 使用的 <see cref="SystemTextJsonSerializerOptions"/>
    /// </summary>
    public virtual SystemTextJsonSerializerOptions UseJsonSerializerOptions
    {
        get
        {
            if (_JsonSerializerOptions == null)
            {
                var baseOptions = JsonSerializerOptions;
                _JsonSerializerOptions = Serializable.CreateOptions(baseOptions);
            }
            return _JsonSerializerOptions;
        }
    }

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
    /// 根据客户端 <see cref="Exception"/> 获取错误码
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    protected virtual ApiRspCode GetApiRspCodeByClientException(Exception ex)
    {
        if (ex is HttpRequestException && ex.InnerException is SocketException socketException)
        {
            switch (socketException.SocketErrorCode)
            {
                case SocketError.TimedOut:
                    return ApiRspCode.Timeout;
                case SocketError.ConnectionRefused:
                    {
                        // System.Net.Http.HttpRequestException: 由于目标计算机积极拒绝，无法连接。 (localhost:443)
                        // ---> System.Net.Sockets.SocketException (10061): 由于目标计算机积极拒绝，无法连接。
                        // at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.CreateException(SocketError error, Boolean forAsyncThrow)
                        return ApiRspCode.ConnectionRefused;
                    }
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

    /// <inheritdoc cref="OnError"/>
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
        var typeResponseBody = typeof(TResponseBody);
        if (typeResponseBody == typeof(nil))
            return default;

        var apiRspBase = GetApiRspBase<TResponseBody>(typeResponseBody);
        if (apiRspBase != null)
        {
            apiRspBase.Code = GetApiRspCodeByClientException(ex);
            apiRspBase.ClientException = ex;
            return (TResponseBody?)(object)apiRspBase;
        }

        return default;
    }

    #region Send

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
            return GetCustomSerializeContent<TResponseBody, TRequestBody>(args, requestBody, mime, cancellationToken);
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
        HttpContentWrapper<TResponseBody> result;
        switch (mime)
        {
            case MediaTypeNames.JSON:
                switch (args.JsonImplType)
                {
                    case Serializable.JsonImplType.NewtonsoftJson:
#pragma warning disable CS0618 // 类型或成员已过时
                        result = GetNJsonContent<TResponseBody, TRequestBody>(requestBody);
#pragma warning restore CS0618 // 类型或成员已过时
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
            case MediaTypeNames.Binary:
                if (requestBody is not byte[] byteArray)
                {
                    if (requestBody is ReadOnlyMemory<byte> readOnlyMemoryByte)
                    {
                        byteArray = readOnlyMemoryByte.ToArray();
                    }
                    if (requestBody is Memory<byte> memoryByte)
                    {
                        byteArray = memoryByte.ToArray();
                    }
                    if (requestBody is IEnumerable<byte> bytes)
                    {
                        byteArray = bytes.ToArray();
                    }
                    if (requestBody is Stream stream)
                    {
                        byteArray = stream.ToByteArray();
                    }
                    else
                    {
                        throw new NotSupportedException("requestBody is not byte[] byteArray.");
                    }
                }
                result = new ByteArrayContent(byteArray);
                return result;
            case MediaTypeNames.MessagePack:
                result = GetMessagePackContent<TResponseBody, TRequestBody>(requestBody, cancellationToken: cancellationToken);
                return result;
            case MediaTypeNames.MemoryPack:
                result = GetMemoryPackContent<TResponseBody, TRequestBody>(requestBody);
                return result;
            default:
                throw ThrowHelper.GetArgumentOutOfRangeException(mime);
        }
    }

    /// <summary>
    /// 可重写自定义其他 MIME 的反序列化
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="responseMessage"></param>
    /// <param name="mime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<TResponseBody?> ReadFromCustomDeserializeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        WebApiClientSendArgs args,
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