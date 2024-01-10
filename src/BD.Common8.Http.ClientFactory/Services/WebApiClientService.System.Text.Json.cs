namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ）
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContentWrapper<TResponseBody> GetSJsonContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(
        TRequestBody inputValue,
        MediaTypeHeaderValue? mediaType = null)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        if (inputValue == null)
            return default;
        JsonContent content;
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        // JsonSerializerOptions 应使用 源生成 的 类型解析器
        content = JsonContent.Create(inputValue, mediaType, UseJsonSerializerOptions);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return content;
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<TResponseBody?> ReadFromSJsonAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        HttpContent content,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        var modelType = typeof(TResponseBody);
        TResponseBody? result;
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        // JsonSerializerOptions 应使用 源生成 的 类型解析器
        result = await content.ReadFromJsonAsync<TResponseBody>(UseJsonSerializerOptions, cancellationToken);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成异步迭代器（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual IAsyncEnumerable<TResponseBody?> ReadFromSJsonAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        HttpContent content,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        IAsyncEnumerable<TResponseBody?> result;
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        // JsonSerializerOptions 应使用 源生成 的 类型解析器
        result = content.ReadFromJsonAsAsyncEnumerable<TResponseBody>(UseJsonSerializerOptions, cancellationToken);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(Obsolete_UseAsync)]
    protected virtual TResponseBody? ReadFromSJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        HttpContent content,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        TResponseBody? result;
        using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        // JsonSerializerOptions 应使用 源生成 的 类型解析器
        result = SystemTextJsonSerializer.Deserialize<TResponseBody>(contentStream, UseJsonSerializerOptions);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }
}
