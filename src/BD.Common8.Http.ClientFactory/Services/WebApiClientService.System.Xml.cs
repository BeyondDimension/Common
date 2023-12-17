namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 XML
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="encoding"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    protected virtual HttpContentWrapper<TResponseBody> GetXmlContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(TRequestBody inputValue, Encoding? encoding = null, MediaTypeHeaderValue? mediaType = null)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 XML
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    protected async Task<TResponseBody?> ReadFromXmlAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        encoding ??= DefaultEncoding;
        using var contentStream = await content.ReadAsStreamAsync(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
        using var contentStreamReader = new StreamReader(contentStream, encoding);
        var result = Serializable.DXml<TResponseBody>(contentStreamReader);
        return result;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 XML
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    [Obsolete(Obsolete_UseAsync)]
    protected virtual TResponseBody? ReadFromXml<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        encoding ??= DefaultEncoding;
        using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
        using var contentStreamReader = new StreamReader(contentStream, encoding);
        var result = Serializable.DXml<TResponseBody>(contentStreamReader);
        return result;
    }
}