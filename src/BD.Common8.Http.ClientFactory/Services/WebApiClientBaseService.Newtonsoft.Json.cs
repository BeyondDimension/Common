namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    protected NewtonsoftJsonSerializer? newtonsoftJsonSerializer = newtonsoftJsonSerializer;

    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    protected NewtonsoftJsonSerializer NewtonsoftJsonSerializer => newtonsoftJsonSerializer ??= new();

    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/>），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    [Obsolete("推荐使用 GetSJsonContent，即 System.Text.Json")]
    protected virtual HttpContent? GetNJsonContent<T>(T inputValue, MediaTypeHeaderValue? mediaType = null)
    {
        try
        {
            var stream = new MemoryStream(); // 使用内存流，避免 byte[] 块，与字符串 utf16 开销
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(streamWriter);
            NewtonsoftJsonSerializer.Serialize(jsonWriter, inputValue, typeof(T));
            var content = new StreamContent(stream);
            content.Headers.ContentType = mediaType ?? new MediaTypeHeaderValue("application/json", "utf-8");
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error serializing request model class. (Parameter '{type}')",
                typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/>），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("推荐使用 ReadFromSJsonAsync，即 System.Text.Json")]
    protected virtual async Task<T?> ReadFromNJsonAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = await content.ReadAsStreamAsync(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
            using var streamReader = new StreamReader(stream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(streamReader);
            var value = NewtonsoftJsonSerializer.Deserialize<T>(jsonReader);
            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error reading and deserializing the response content into an instance. (Parameter '{type}')",
                typeof(T));
            return default;
        }
    }
}
