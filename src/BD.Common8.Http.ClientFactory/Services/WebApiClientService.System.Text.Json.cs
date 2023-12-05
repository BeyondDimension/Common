namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContentWrapper<TResponseBody> GetSJsonContent<TResponseBody, TRequestBody>(TRequestBody inputValue, JsonTypeInfo<TRequestBody>? jsonTypeInfo = null, MediaTypeHeaderValue? mediaType = null)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        if (inputValue == null)
            return default;
        try
        {
            JsonContent content;
            jsonTypeInfo ??= JsonSerializerContext?.GetTypeInfo(typeof(TRequestBody)) is JsonTypeInfo<TRequestBody> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonSerializerContext) throw new ArgumentNullException(nameof(jsonTypeInfo));
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                content = JsonContent.Create(inputValue, mediaType);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            }
            else
            {
                content = JsonContent.Create(inputValue, jsonTypeInfo, mediaType);
            }
            return content;
        }
        catch (Exception ex)
        {
            return OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize: true, typeof(TRequestBody));
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<TResponseBody?> ReadFromSJsonAsync<TResponseBody>(HttpContent content, JsonTypeInfo<TResponseBody>? jsonTypeInfo, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        try
        {
            TResponseBody? result;
            jsonTypeInfo ??= JsonSerializerContext?.GetTypeInfo(typeof(TResponseBody)) is JsonTypeInfo<TResponseBody> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonSerializerContext)
                {
                    var modelType = typeof(TResponseBody);
                    if (modelType != typeof(SystemTextJsonObject))
                    {
                        throw new ArgumentNullException(nameof(jsonTypeInfo));
                    }
                }
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                result = await content.ReadFromJsonAsync<TResponseBody>(cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            }
            else
            {
                result = await content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken);
            }
            return result;
        }
        catch (Exception ex)
        {
            return OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize: false, typeof(TResponseBody));
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(Obsolete_UseAsync)]
    protected virtual TResponseBody? ReadFromSJson<TResponseBody>(HttpContent content, JsonTypeInfo<TResponseBody>? jsonTypeInfo, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        try
        {
            TResponseBody? result;
            jsonTypeInfo ??= JsonSerializerContext?.GetTypeInfo(typeof(TResponseBody)) is JsonTypeInfo<TResponseBody> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonSerializerContext)
                {
                    var modelType = typeof(TResponseBody);
                    if (modelType != typeof(SystemTextJsonObject))
                    {
                        throw new ArgumentNullException(nameof(jsonTypeInfo));
                    }
                }
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                result = SystemTextJsonSerializer.Deserialize<TResponseBody>(contentStream);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            }
            else
            {
                using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                result = SystemTextJsonSerializer.Deserialize(contentStream, jsonTypeInfo);
            }
            return result;
        }
        catch (Exception ex)
        {
            return OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize: false, typeof(TResponseBody));
        }
    }
}
