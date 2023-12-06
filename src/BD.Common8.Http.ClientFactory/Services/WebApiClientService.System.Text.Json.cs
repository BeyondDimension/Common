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
    /// 根据 Http 内容获取编码
    /// <para>https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Net.Http.Json/src/System/Net/Http/Json/JsonHelpers.cs#L32</para>
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    protected static Encoding? GetEncoding(HttpContent content)
    {
        Encoding? encoding = null;

        if (content.Headers.ContentType?.CharSet is string charset)
        {
            try
            {
                // Remove at most a single set of quotes.
                if (charset.Length > 2 && charset[0] == '\"' && charset[^1] == '\"')
                {
                    encoding = Encoding.GetEncoding(charset[1..^1]);
                }
                else
                {
                    encoding = Encoding.GetEncoding(charset);
                }
            }
            catch
            {
                return null;
            }
        }

        return encoding;
    }

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Net.Http.Json/src/System/Net/Http/Json/HttpContentJsonExtensions.netcoreapp.cs#L13
    /// </summary>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Task<Stream> ReadHttpContentStreamAsync(HttpContent content, CancellationToken cancellationToken)
    {
        return content.ReadAsStreamAsync(cancellationToken);
    }

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Net.Http.Json/src/System/Net/Http/Json/HttpContentJsonExtensions.netcoreapp.cs#L18
    /// </summary>
    /// <param name="contentStream"></param>
    /// <param name="sourceEncoding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Stream GetTranscodingStream(Stream contentStream, Encoding sourceEncoding)
    {
        return Encoding.CreateTranscodingStream(contentStream, innerStreamEncoding: sourceEncoding, outerStreamEncoding: Encoding.UTF8);
    }

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Net.Http.Json/src/System/Net/Http/Json/HttpContentJsonExtensions.cs#L144
    /// </summary>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async ValueTask<Stream> GetContentStreamAsync(HttpContent content, CancellationToken cancellationToken)
    {
        Stream contentStream = await ReadHttpContentStreamAsync(content, cancellationToken).ConfigureAwait(false);

        // Wrap content stream into a transcoding stream that buffers the data transcoded from the sourceEncoding to utf-8.
        if (GetEncoding(content) is Encoding sourceEncoding && sourceEncoding != Encoding.UTF8)
        {
            contentStream = GetTranscodingStream(contentStream, sourceEncoding);
        }

        return contentStream;
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
        var modelType = typeof(TResponseBody);
        try
        {
            TResponseBody? result;
            jsonTypeInfo ??= JsonSerializerContext?.GetTypeInfo(typeof(TResponseBody)) is JsonTypeInfo<TResponseBody> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonSerializerContext)
                {
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
            return OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize: false, modelType);
        }
    }

    /// <summary>
    /// 空的异步迭代器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    readonly struct EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        public readonly T Current => default!;

        public readonly ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public readonly IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return this;
        }

        public readonly ValueTask<bool> MoveNextAsync()
        {
            return new(false);
        }
    }

    /// <summary>
    /// 将迭代器转换为异步迭代器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    protected static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> values)
    {
        await ValueTask.CompletedTask;
        foreach (var item in values)
        {
            yield return item;
        }
    }

    /// <summary>
    /// 将数组转换为异步迭代器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    protected static IAsyncEnumerable<T> ToAsyncEnumerable<T>(params T[] values) => ToAsyncEnumerable(values.AsEnumerable());

    /// <summary>
    /// 将响应内容读取并反序列化成异步迭代器（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual IAsyncEnumerable<TResponseBody?> ReadFromSJsonAsAsyncEnumerable<TResponseBody>(HttpContent content, JsonTypeInfo<TResponseBody>? jsonTypeInfo, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        var modelType = typeof(TResponseBody);
        try
        {
            IAsyncEnumerable<TResponseBody?> result;
            jsonTypeInfo ??= JsonSerializerContext?.GetTypeInfo(typeof(TResponseBody)) is JsonTypeInfo<TResponseBody> jsonTypeInfo_ ? jsonTypeInfo_ : null;
            if (jsonTypeInfo == null)
            {
                if (RequiredJsonSerializerContext)
                {
                    if (modelType != typeof(SystemTextJsonObject))
                    {
                        throw new ArgumentNullException(nameof(jsonTypeInfo));
                    }
                }
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                result = content.ReadFromJsonAsAsyncEnumerable<TResponseBody>(cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            }
            else
            {
                result = content.ReadFromJsonAsAsyncEnumerable(jsonTypeInfo, cancellationToken);
            }
            return result;
        }
        catch (Exception ex)
        {
            var resultItem = OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize: false, modelType);
            if (resultItem == null)
            {
                return default(EmptyAsyncEnumerable<TResponseBody>);
            }
            else
            {
                return ToAsyncEnumerable(resultItem);
            }
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
