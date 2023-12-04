namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <summary>
    /// 发送 HTTP 请求的参数
    /// </summary>
    public record class SendArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendArgs"/> class.
        /// </summary>
        /// <param name="requestUriString"></param>
        public SendArgs([StringSyntax(StringSyntaxAttribute.Uri)] string requestUriString)
        {
            RequestUriString = requestUriString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendArgs"/> class.
        /// </summary>
        /// <param name="requestUri"></param>
        public SendArgs(Uri requestUri)
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
                {
                    return _RequestUriString;
                }
                if (RequestUri != null)
                {
                    return _RequestUriString = RequestUri.ToString();
                }
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
                {
                    return _RequestUri;
                }
                if (!string.IsNullOrWhiteSpace(_RequestUriString))
                {
                    return _RequestUri = new(_RequestUriString, UriKind.RelativeOrAbsolute);
                }
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
        public virtual HttpRequestMessage GetHttpRequestMessage(WebApiClientBaseService s, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequestMessage = new(Method, RequestUri);

            if (!string.IsNullOrEmpty(Accept))
            {
                httpRequestMessage.Headers.Accept.ParseAdd(Accept);
            }

            var content = GetRequestContent(s, cancellationToken);
            if (content != null)
            {
                httpRequestMessage.Content = content;
            }

            if (!EmptyUserAgent)
            {
                var userAgent = UserAgent;
                if (string.IsNullOrEmpty(userAgent))
                {
                    userAgent = s.UserAgent;
                }
                if (!string.IsNullOrEmpty(userAgent))
                {
                    httpRequestMessage.Headers.UserAgent.ParseAdd(userAgent);
                }
            }

            ConfigureRequestMessage?.Invoke(httpRequestMessage, this, cancellationToken);

            return httpRequestMessage;
        }

        /// <summary>
        /// 自定义的请求正文
        /// </summary>
        public Func<WebApiClientBaseService, SendArgs, CancellationToken, HttpContent?>? GetRequestContentDelegate { get; set; }

        /// <summary>
        /// 获取请求正文
        /// </summary>
        /// <param name="s"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual HttpContent? GetRequestContent(WebApiClientBaseService s, CancellationToken cancellationToken)
        {
            return GetRequestContentDelegate?.Invoke(s, this, cancellationToken);
        }

        /// <summary>
        /// 自定义处理请求消息委托
        /// </summary>
        public Action<HttpRequestMessage, SendArgs, CancellationToken>? ConfigureRequestMessage { get; init; }

        ///// <summary>
        ///// 自定义处理响应消息委托
        ///// </summary>
        //public Action<HttpResponseMessage, SendArgs, CancellationToken>? ConfigureResponseMessage { get; set; }

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

        /// <summary>
        /// 重试请求，最大重试次数，为 0 时不重试，默认值 0
        /// </summary>
        public int NumRetries { get; init; }

        /// <summary>
        /// 用于重试的间隔时间计算
        /// </summary>
        /// <param name="attemptNumber"></param>
        /// <returns></returns>
        public virtual TimeSpan PollyRetryAttempt(int attemptNumber)
        {
            var powY = attemptNumber % NumRetries;
            var timeSpan = TimeSpan.FromMilliseconds(Math.Pow(2, powY));
            int addS = attemptNumber / NumRetries;
            if (addS > 0) timeSpan = timeSpan.Add(TimeSpan.FromSeconds(addS));
            return timeSpan;
        }
    }

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
    public virtual async Task<TResponseBody?> SendAsync<TResponseBody>(SendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
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

        var client = CreateClient();
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
                var result = await SendCoreAsync<TResponseBody>(client, args, cancellationToken);
                return result;
            }
        }
        else
        {
            var result = await SendCoreAsync<TResponseBody>(client, args, cancellationToken);
            return result.Value;
        }
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
    [Obsolete(SerializableExtensions.Obsolete_UseAsync)]
    public virtual TResponseBody? Send<TResponseBody>(SendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
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

        var client = CreateClient();
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
                var result = SendCore<TResponseBody>(client, args, cancellationToken);
                return result;
            }
        }
        else
        {
            var result = SendCore<TResponseBody>(client, args, cancellationToken);
            return result.Value;
        }
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
    public virtual async Task<TResponseBody?> SendAsync<TResponseBody, TRequestBody>(SendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default) where TResponseBody : notnull where TRequestBody : notnull
    {
        args.GetRequestContentDelegate = (s, args, cancellationToken) => s.GetRequestContent(args, requestBody, cancellationToken);

        var result = await SendAsync<TResponseBody>(args, cancellationToken);
        return result;
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
    [Obsolete(SerializableExtensions.Obsolete_UseAsync)]
    public virtual TResponseBody? Send<TResponseBody, TRequestBody>(SendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default) where TResponseBody : notnull where TRequestBody : notnull
    {
        args.GetRequestContentDelegate = (s, args, cancellationToken) => s.GetRequestContent(args, requestBody, cancellationToken);

        var result = Send<TResponseBody>(args, cancellationToken);
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
    public virtual async Task<HttpStatusCode> SendFromStatusCodeAsync<TRequestBody>(SendArgs args, CancellationToken cancellationToken = default) where TRequestBody : notnull
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
    public virtual async Task<HttpStatusCode> SendFromStatusCodeAsync<TRequestBody>(SendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default) where TRequestBody : notnull
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
    /// <param name="client"></param>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<SendResultWrapper<TResponseBody>> SendCoreAsync<TResponseBody>(HttpClient client, SendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        var disposeResponseMessage = true;
        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            requestMessage = args.GetHttpRequestMessage(this, cancellationToken);
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
                var mime = responseMessage.Content.Headers.ContentType?.MediaType ?? args.Accept;
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
    /// <param name="client"></param>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual SendResultWrapper<TResponseBody> SendCore<TResponseBody>(HttpClient client, SendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        var disposeResponseMessage = true;
        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            requestMessage = args.GetHttpRequestMessage(this, cancellationToken);
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
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    HttpContent? GetRequestContent<TRequestBody>(SendArgs args, TRequestBody requestBody, CancellationToken cancellationToken) where TRequestBody : notnull
    {
        if (typeof(TRequestBody) == typeof(nil))
            return null;
        var mime = args.Accept;
        HttpContent? result;
#pragma warning disable CS0618 // 类型或成员已过时
        switch (mime)
        {
            case MediaTypeNames.JSON:
                switch (args.JsonImplType)
                {
                    case Serializable.JsonImplType.NewtonsoftJson:
                        result = GetNJsonContent(requestBody);
                        return result;
                    case Serializable.JsonImplType.SystemTextJson:
                        result = GetSJsonContent(requestBody);
                        return result;
                    default:
                        throw ThrowHelper.GetArgumentOutOfRangeException(args.JsonImplType);
                }
            case MediaTypeNames.XML:
            case MediaTypeNames.XML_APP:
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                result = GetXmlContent(requestBody);
                return result;
            case MediaTypeNames.FormUrlEncoded:
                if (requestBody is not IEnumerable<KeyValuePair<string?, string?>> nameValueCollection)
                    throw new NotSupportedException("requestBody is not IEnumerable<KeyValuePair<string?, string?>> nameValueCollection.");
                result = new FormUrlEncodedContent(nameValueCollection);
                return result;
            default:
                result = GetCustomSerializeContent(args, requestBody, mime, cancellationToken);
                return result;
        }
#pragma warning restore CS0618 // 类型或成员已过时
    }

    /// <summary>
    /// 可重写自定义其他 MIME 的序列化
    /// </summary>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="mime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual HttpContent? GetCustomSerializeContent<TRequestBody>(
        SendArgs args,
        TRequestBody requestBody,
        string? mime,
        CancellationToken cancellationToken) where TRequestBody : notnull
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
    [Obsolete(SerializableExtensions.Obsolete_UseAsync)]
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
    /// <typeparam name="T"></typeparam>
    protected readonly record struct SendResultWrapper<T> where T : notnull
    {
        /// <summary>
        /// 发送请求响应的结果值
        /// </summary>
        public T? Value { get; init; }

        /// <summary>
        /// 请求是否中止，比如取消，停止重试等
        /// </summary>
        public required bool IsStopped { get; init; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SendResultWrapper<T>(bool isStopped) => isStopped ? new()
        {
            IsStopped = true,
        } : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SendResultWrapper<T>(T? value) => new()
        {
            IsStopped = true,
            Value = value,
        };
    }

    /// <summary>
    /// 包装 Http 响应正文流，流释放时候跟随释放响应消息
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="inner"></param>
    protected sealed class HttpResponseMessageContentStream(HttpResponseMessage responseMessage, Stream inner) : DelegatingStream(inner)
    {
        HttpResponseMessage? responseMessage = responseMessage;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                responseMessage?.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            responseMessage = null;
        }

        /// <inheritdoc cref="HttpContent.ReadAsStreamAsync(CancellationToken)"/>
        public static async Task<Stream> ReadAsStreamAsync(
            HttpResponseMessage responseMessage,
            CancellationToken cancellationToken = default)
        {
            var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var result = new HttpResponseMessageContentStream(responseMessage, stream);
            return result;
        }

        /// <inheritdoc cref="HttpContent.ReadAsStream(CancellationToken)"/>
        public static Stream ReadAsStream(
            HttpResponseMessage responseMessage,
            CancellationToken cancellationToken = default)
        {
            var stream = responseMessage.Content.ReadAsStream(cancellationToken);
            var result = new HttpResponseMessageContentStream(responseMessage, stream);
            return result;
        }
    }
}
