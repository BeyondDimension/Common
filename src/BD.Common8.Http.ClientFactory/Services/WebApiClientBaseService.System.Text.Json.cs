namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContent? GetSJsonContent<T>(T inputValue, JsonTypeInfo<T>? jsonTypeInfo, MediaTypeHeaderValue? mediaType = null)
    {
        if (inputValue == null)
            return null;
        try
        {
            JsonContent content;
            jsonTypeInfo ??= JsonTypeInfo is JsonTypeInfo<T> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonTypeInfo) throw new ArgumentNullException(nameof(jsonTypeInfo));
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
            OnSerializerError(ex, isSerializeOrDeserialize: true, typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 序列化是否必须使用 <see cref="JsonTypeInfo"/>，即源生成的类型信息数据，避免运行时反射
    /// </summary>
    protected virtual bool RequiredJsonTypeInfo => true;

    /// <summary>
    /// 用于序列化的类型信息，由 Json 源生成
    /// </summary>
    protected virtual JsonTypeInfo? JsonTypeInfo { get; }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<T?> ReadFromSJsonAsync<T>(HttpContent content, JsonTypeInfo<T>? jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            T? result;
            jsonTypeInfo ??= JsonTypeInfo is JsonTypeInfo<T> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonTypeInfo)
                {
                    var modelType = typeof(T);
                    if (modelType != typeof(SystemTextJsonObject))
                    {
                        throw new ArgumentNullException(nameof(jsonTypeInfo));
                    }
                }
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                result = await content.ReadFromJsonAsync<T>(cancellationToken);
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
            OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("无特殊情况下应使用 Async 异步的函数版本")]
    protected virtual T? ReadFromSJson<T>(HttpContent content, JsonTypeInfo<T>? jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            T? result;
            jsonTypeInfo ??= JsonTypeInfo is JsonTypeInfo<T> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonTypeInfo)
                {
                    var modelType = typeof(T);
                    if (modelType != typeof(SystemTextJsonObject))
                    {
                        throw new ArgumentNullException(nameof(jsonTypeInfo));
                    }
                }
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                result = SystemTextJsonSerializer.Deserialize<T>(contentStream);
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
            OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
            return default;
        }
    }
}
