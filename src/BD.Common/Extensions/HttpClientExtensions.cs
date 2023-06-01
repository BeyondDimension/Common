using System.Xml.Serialization;
#if __HAVE_N_JSON__
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
#endif
using SJsonSerializer = System.Text.Json.JsonSerializer;
using SJsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

// ReSharper disable once CheckNamespace
namespace System;

public static class HttpClientExtensions
{
#if __HAVE_N_JSON__
    static readonly Lazy<JsonSerializer> jsonSerializerLazy = new(() => new());
#endif

    public const Serializable.JsonImplType DefJsonImplType =
#if __HAVE_N_JSON__
        Serializable.JsonImplType.NewtonsoftJson;
#else
        Serializable.JsonImplType.SystemTextJson;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async ValueTask<T?> DeserializeUtf8JsonAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        HttpResponseMessage response,
        SJsonSerializerOptions? options = null,
        JsonTypeInfo<T>? jsonTypeInfo = null,
        CancellationToken cancellationToken = default)
    {
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        T? value;
        if (jsonTypeInfo != null)
        {
            value = await SJsonSerializer.DeserializeAsync<T>(stream, jsonTypeInfo,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        else
        {
            value = await SJsonSerializer.DeserializeAsync<T>(stream, options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.XmlSerializer(Type)")]
    static async Task<T?> DeserializeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(bool isJsonOrXml,
        JsonSerializer? jsonSerializer,
        HttpResponseMessage response,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default)
    {
#if !__HAVE_N_JSON__
        throw new NotSupportedException();
#endif
        encoding ??= Encoding.UTF8;
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream, encoding);
        if (isJsonOrXml)
        {
#if __HAVE_N_JSON__
            jsonSerializer ??= jsonSerializerLazy.Value;
            using var json = new JsonTextReader(reader);
            return jsonSerializer.Deserialize<T>(json);
#else
            throw new NotSupportedException();
#endif
        }
        else
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T?)xmlSerializer.Deserialize(reader);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.XmlSerializer(Type)")]
    static async Task<T?> SendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this HttpClient client,
        ILogger logger,
        JsonSerializer? jsonSerializer,
        bool isCheckHttpUrl,
        string? requestUri,
        Func<HttpRequestMessage> requestFactory,
        string? accept,
        //bool enableForward,
        Action<HttpResponseMessage>? handlerResponse = null,
        Action<HttpResponseMessage>? handlerResponseByIsNotSuccessStatusCode = null,
        Serializable.JsonImplType jsonImplType = DefJsonImplType,
        SJsonSerializerOptions? s_options = null,
        JsonTypeInfo<T>? jsonTypeInfo = null,
        CancellationToken cancellationToken = default) where T : notnull
    {
        HttpRequestMessage? request = null;
        bool requestIsSend = false;
        HttpResponseMessage? response = null;
        bool notDisposeResponse = false;
        try
        {
            request = requestFactory();
            requestUri ??= request.RequestUri?.ToString();

            if (!isCheckHttpUrl && !String2.IsHttpUrl(requestUri)) return default;

            //if (enableForward && IsAllowUrl(requestUri))
            //{
            //    try
            //    {
            //        requestIsSend = true;
            //        response = await Csc.Forward(request,
            //            HttpCompletionOption.ResponseHeadersRead,
            //            cancellationToken);
            //    }
            //    catch (Exception e)
            //    {
            //        logger.LogWarning(e, "CloudService Forward Fail, requestUri: {0}", requestUri);
            //        response = null;
            //    }
            //}

            if (response == null)
            {
                if (requestIsSend)
                {
                    request.Dispose();
                    request = requestFactory();
                }
                response = await client.SendAsync(request,
                  HttpCompletionOption.ResponseHeadersRead,
                  cancellationToken).ConfigureAwait(false);
            }

            handlerResponse?.Invoke(response);

            var rspContentClrType = typeof(T);
            if (rspContentClrType == typeof(HttpStatusCode))
            {
                return ConvertibleHelper.Convert<T, HttpStatusCode>(response.StatusCode);
            }

            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    if (rspContentClrType == typeof(string))
                    {
                        return (T)(object)await response.Content.ReadAsStringAsync(cancellationToken);
                    }
                    else if (rspContentClrType == typeof(byte[]))
                    {
                        return (T)(object)await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    }
                    else if (rspContentClrType == typeof(Stream))
                    {
                        notDisposeResponse = true;
                        return (T)(object)await response.Content.ReadAsStreamAsync(cancellationToken);
                    }
                    var mime = response.Content.Headers.ContentType?.MediaType ?? accept;
                    switch (mime)
                    {
                        case MediaTypeNames.JSON:
                            switch (jsonImplType)
                            {
                                case Serializable.JsonImplType.NewtonsoftJson:
                                    return await DeserializeAsync<T>(isJsonOrXml: true,
                                        jsonSerializer,
                                        response,
                                        cancellationToken: cancellationToken);
                                case Serializable.JsonImplType.SystemTextJson:
                                    return await DeserializeUtf8JsonAsync(response,
                                        s_options, jsonTypeInfo, cancellationToken);
                            }
                            break;
                        case MediaTypeNames.XML:
                        case MediaTypeNames.XML_APP:
                            return await DeserializeAsync<T>(isJsonOrXml: false,
                                null,
                                response,
                                cancellationToken: cancellationToken);
                    }
                }
            }
            else
            {
                handlerResponseByIsNotSuccessStatusCode?.Invoke(response);
            }
        }
        catch (Exception e)
        {
            var knownType = e.GetKnownType();
            if (knownType == ExceptionKnownType.Unknown)
            {
                logger.LogError(e, "SendAsync fail, requestUri: {requestUri}", requestUri);
            }
        }
        finally
        {
            request?.Dispose();
            if (!notDisposeResponse)
            {
                response?.Dispose();
            }
        }
        return default;
    }

    /// <summary>
    /// 以异步操作发送 HTTP 请求
    /// </summary>
    /// <typeparam name="T">Response Body 类型</typeparam>
    /// <param name="client"></param>
    /// <param name="logger"></param>
    /// <param name="jsonSerializer">使用 Newtonsoft.Json 可选的自定义序列化</param>
    /// <param name="requestUri">请求 Url</param>
    /// <param name="requestFactory"></param>
    /// <param name="accept">请求头：accept</param>
    /// <param name="handlerResponse"></param>
    /// <param name="handlerResponseByIsNotSuccessStatusCode"></param>
    /// <param name="jsonImplType">使用的 Json 序列化实现，默认使用 <see cref="DefJsonImplType"/></param>
    /// <param name="s_options">使用 System.Text.Json 可选的自定义序列化</param>
    /// <param name="jsonTypeInfo">使用 System.Text.Json 源生成需要传递的参数</param>
    /// <param name="cancellationToken">取消操作的取消标记</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.XmlSerializer(Type)")]
    public static Task<T?> SendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this HttpClient client,
        ILogger logger,
        JsonSerializer? jsonSerializer,
        string? requestUri,
        Func<HttpRequestMessage> requestFactory,
        string? accept,
        //bool enableForward,
        Action<HttpResponseMessage>? handlerResponse = null,
        Action<HttpResponseMessage>? handlerResponseByIsNotSuccessStatusCode = null,
        Serializable.JsonImplType jsonImplType = DefJsonImplType,
        SJsonSerializerOptions? s_options = null,
        JsonTypeInfo<T>? jsonTypeInfo = null,
        CancellationToken cancellationToken = default) where T : notnull
    {
        return client.SendAsync<T>(
            logger,
            jsonSerializer,
            isCheckHttpUrl: false,
            requestUri,
            requestFactory,
            accept,
            //enableForward,
            handlerResponse,
            handlerResponseByIsNotSuccessStatusCode,
            jsonImplType,
            s_options,
            jsonTypeInfo,
            cancellationToken);
    }

    /// <summary>
    /// 以异步操作发送 HTTP GET 请求
    /// </summary>
    /// <typeparam name="T">Response Body 类型</typeparam>
    /// <param name="client"></param>
    /// <param name="logger"></param>
    /// <param name="requestUri">请求 Url</param>
    /// <param name="accept">请求头：accept</param>
    /// <param name="cookie">请求头：cookie，可使用 <see cref="CookieContainer.GetCookieHeader(Uri)"/> 获取请求头字符串</param>
    /// <param name="userAgent">请求头：userAgent</param>
    /// <param name="jsonSerializer">使用 Newtonsoft.Json 可选的自定义序列化</param>
    /// <param name="jsonImplType">使用的 Json 序列化实现，默认使用 <see cref="DefJsonImplType"/></param>
    /// <param name="s_options">使用 System.Text.Json 可选的自定义序列化</param>
    /// <param name="jsonTypeInfo">使用 System.Text.Json 源生成需要传递的参数</param>
    /// <param name="cancellationToken">取消操作的取消标记</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.XmlSerializer(Type)")]
    public static Task<T?> GetAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this HttpClient client,
        ILogger logger,
        string requestUri,
        string accept = MediaTypeNames.JSON,
        string? cookie = null,
        string? userAgent = null,
        JsonSerializer? jsonSerializer = null,
        Serializable.JsonImplType jsonImplType = DefJsonImplType,
        SJsonSerializerOptions? s_options = null,
        JsonTypeInfo<T>? jsonTypeInfo = null,
        CancellationToken cancellationToken = default) where T : notnull
    {
        if (!String2.IsHttpUrl(requestUri))
            return Task.FromResult(default(T?));
        return client.SendAsync<T>(logger, jsonSerializer, isCheckHttpUrl: true, requestUri, () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (cookie != null)
            {
                request.Headers.Add("Cookie", cookie);
            }
            request.Headers.Accept.ParseAdd(accept);
            if (userAgent != null)
                request.Headers.UserAgent.ParseAdd(userAgent);
            return request;
        }, accept/*, true*/, jsonImplType: jsonImplType,
        s_options: s_options,
        jsonTypeInfo: jsonTypeInfo,
        cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 以异步操作发送 HTTP 请求
    /// </summary>
    /// <typeparam name="T">Response Body 类型</typeparam>
    /// <param name="client"></param>
    /// <param name="logger"></param>
    /// <param name="requestUri">请求 Url</param>
    /// <param name="method">请求方法</param>
    /// <param name="accept">请求头：accept</param>
    /// <param name="cookie">请求头：cookie，可使用 <see cref="CookieContainer.GetCookieHeader(Uri)"/> 获取请求头字符串</param>
    /// <param name="userAgent">请求头：userAgent</param>
    /// <param name="content">请求内容</param>
    /// <param name="jsonSerializer">使用 Newtonsoft.Json 可选的自定义序列化</param>
    /// <param name="jsonImplType">使用的 Json 序列化实现，默认使用 <see cref="DefJsonImplType"/></param>
    /// <param name="s_options">使用 System.Text.Json 可选的自定义序列化</param>
    /// <param name="jsonTypeInfo">使用 System.Text.Json 源生成需要传递的参数</param>
    /// <param name="cancellationToken">取消操作的取消标记</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.XmlSerializer(Type)")]
    public static Task<T?> SendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this HttpClient client,
        ILogger logger,
        string requestUri,
        HttpMethod method,
        string accept = MediaTypeNames.JSON,
        string? cookie = null,
        string? userAgent = null,
        Func<HttpContent>? content = null,
        JsonSerializer? jsonSerializer = null,
        Serializable.JsonImplType jsonImplType = DefJsonImplType,
        SJsonSerializerOptions? s_options = null,
        JsonTypeInfo<T>? jsonTypeInfo = null,
        CancellationToken cancellationToken = default) where T : notnull
    {
        if (!String2.IsHttpUrl(requestUri))
            return Task.FromResult(default(T?));
        return client.SendAsync<T>(logger, jsonSerializer, isCheckHttpUrl: true, requestUri, () =>
        {
            var request = new HttpRequestMessage(method, requestUri);
            if (cookie != null)
            {
                request.Headers.Add("Cookie", cookie);
            }
            request.Headers.Accept.ParseAdd(accept);
            if (userAgent != null)
                request.Headers.UserAgent.ParseAdd(userAgent);
            if (content != null)
                request.Content = content();
            return request;
        }, accept/*, true*/, jsonImplType: jsonImplType,
        s_options: s_options,
        jsonTypeInfo: jsonTypeInfo,
        cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 将对象序列化为 <see cref="ByteArrayContent"/>
    /// <para>
    /// <list type="bullet">
    /// <item>使用 ByteArrayContent 而不是 StreamContent</item>
    /// <item>因为请求 HttpRequestMessage 如果已经 Send 过一次就不能再次 Send</item>
    /// <item>HttpRequestMessage 需要能重新建立，包含 HttpContent</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Request Body 类型</typeparam>
    /// <param name="_"></param>
    /// <param name="value">Request Body</param>
    /// <param name="s_options">使用 System.Text.Json 可选的自定义序列化</param>
    /// <param name="jsonTypeInfo">使用 System.Text.Json 源生成需要传递的参数</param>
    /// <param name="action">可选的对 <see cref="ByteArrayContent"/> 需要的附加操作</param>
    /// <param name="cancellationToken">取消操作的取消标记</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Func<ByteArrayContent>> GetJsonContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this HttpClient _,
        T? value,
        SJsonSerializerOptions? s_options = null,
        JsonTypeInfo<T>? jsonTypeInfo = null,
        Action<ByteArrayContent>? action = null,
        CancellationToken cancellationToken = default) where T : notnull
    {
        byte[] bytes;
        using (var stream = new MemoryStream())
        {
            if (jsonTypeInfo != null)
            {
                await SJsonSerializer.SerializeAsync(stream, value!, jsonTypeInfo,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await SJsonSerializer.SerializeAsync(stream, value!, s_options,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            bytes = stream.ToArray();
        }
        return () =>
        {
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.JSON);
            action?.Invoke(content);
            return content;
        };
    }
}
