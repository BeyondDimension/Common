namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = newtonsoftJsonSerializer;

    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    protected NewtonsoftJsonSerializer NewtonsoftJsonSerializer => newtonsoftJsonSerializer ??= new();

    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="encoding"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    [Obsolete("推荐使用 GetSJsonContent，即 System.Text.Json")]
    protected virtual HttpContent? GetNJsonContent<T>(T inputValue, Encoding? encoding = null, MediaTypeHeaderValue? mediaType = null)
    {
        if (inputValue == null)
            return null;
        try
        {
            encoding ??= DefaultEncoding;
            var stream = new MemoryStream(); // 使用内存流，避免 byte[] 块，与字符串 utf16 开销
            using var streamWriter = new StreamWriter(stream, encoding, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(streamWriter);
            NewtonsoftJsonSerializer.Serialize(jsonWriter, inputValue, typeof(T));
            var content = new StreamContent(stream);
            content.Headers.ContentType = mediaType ?? new MediaTypeHeaderValue(MediaTypeNames.JSON, encoding.WebName);
            return content;
        }
        catch (Exception ex)
        {
            OnSerializerError(ex, isSerializeOrDeserialize: true, typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="NewtonsoftJsonObject"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("推荐使用 ReadFromSJsonAsync，即 System.Text.Json")]
    protected virtual async Task<T?> ReadFromNJsonAsync<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        try
        {
            encoding ??= DefaultEncoding;
            using var contentStream = await content.ReadAsStreamAsync(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
            using var contentStreamReader = new StreamReader(contentStream, encoding);
            using var contentJsonTextReader = new JsonTextReader(contentStreamReader);
            var result = NewtonsoftJsonSerializer.Deserialize<T>(contentJsonTextReader);
            return result;
        }
        catch (Exception ex)
        {
            OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="NewtonsoftJsonObject"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("无特殊情况下应使用 Async 异步的函数版本")]
    protected virtual T? ReadFromNJson<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        try
        {
            encoding ??= DefaultEncoding;
            using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
            using var contentStreamReader = new StreamReader(contentStream, encoding);
            using var contentJsonTextReader = new JsonTextReader(contentStreamReader);
            var result = NewtonsoftJsonSerializer.Deserialize<T>(contentJsonTextReader);
            return result;
        }
        catch (Exception ex)
        {
            OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
            return default;
        }
    }
}
