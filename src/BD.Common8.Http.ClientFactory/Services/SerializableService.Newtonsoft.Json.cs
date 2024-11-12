#if !NETFRAMEWORK && !PROJ_SETUP
namespace BD.Common8.Http.ClientFactory.Services;

partial class SerializableService // Newtonsoft.Json
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="encoding"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    [Obsolete(Obsolete_GetNJsonContent)]
    protected virtual HttpContentWrapper<TResponseBody> GetNJsonContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(TRequestBody inputValue, Encoding? encoding = null, MediaTypeHeaderValue? mediaType = null)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        if (inputValue == null)
            return default;
        encoding ??= DefaultEncoding;
        var stream = new MemoryStream(); // 使用内存流，避免 byte[] 块，与字符串 utf16 开销
        using var streamWriter = new StreamWriter(stream, encoding, leaveOpen: true);
        using var jsonWriter = new JsonTextWriter(streamWriter);
        NewtonsoftJsonSerializer.Serialize(jsonWriter, inputValue, typeof(TRequestBody));
        var content = new StreamContent(stream);
        content.Headers.ContentType = mediaType ?? new MediaTypeHeaderValue(MediaTypeNames.JSON, encoding.WebName);
        return content;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="NewtonsoftJsonObject"/></para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(Obsolete_ReadFromNJsonAsync)]
    protected virtual async Task<TResponseBody?> ReadFromNJsonAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        encoding ??= DefaultEncoding;
        using var contentStream = await content.ReadAsStreamAsync(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
        using var contentStreamReader = new StreamReader(contentStream, encoding);
        using var contentJsonTextReader = new JsonTextReader(contentStreamReader);
        var result = NewtonsoftJsonSerializer.Deserialize<TResponseBody>(contentJsonTextReader);
        return result;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
    /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="NewtonsoftJsonObject"/></para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(Obsolete_UseAsync)]
    protected virtual TResponseBody? ReadFromNJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        encoding ??= DefaultEncoding;
        using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
        using var contentStreamReader = new StreamReader(contentStream, encoding);
        using var contentJsonTextReader = new JsonTextReader(contentStreamReader);
        var result = NewtonsoftJsonSerializer.Deserialize<TResponseBody>(contentJsonTextReader);
        return result;
    }
}
#endif