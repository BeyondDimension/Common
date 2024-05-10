namespace BD.Common8.Http.ClientFactory.Services;

partial class SerializableService // MemoryPack
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContentWrapper<TResponseBody> GetMemoryPackContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(TRequestBody inputValue, MediaTypeHeaderValue? mediaType = null)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        var byteArray = Serializable.SMP2(inputValue);
        if (byteArray == null)
            return default;
        mediaType ??= new MediaTypeHeaderValue(MediaTypeNames.MemoryPack);
        var httpContent = new ByteArrayContent(byteArray);
        httpContent.Headers.ContentType = mediaType;
        return httpContent;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="aes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<TResponseBody?> ReadFromMemoryPackAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        HttpContent content,
        Aes? aes = null,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        using var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var stream2 = aes != null ? new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read) : stream;
        try
        {
            var obj = await MemoryPackSerializer.DeserializeAsync<TResponseBody>(stream2, cancellationToken: cancellationToken);
            return obj;
        }
        catch
        {
            var typeTResponseBody = typeof(TResponseBody);
            if (typeTResponseBody.IsGenericType &&
                typeTResponseBody.GetGenericTypeDefinition() == typeof(ApiRspImpl<>))
            {
                var obj = await MemoryPackSerializer.DeserializeAsync<ApiRspImpl>(stream2, cancellationToken: cancellationToken);
                if (obj == null)
                    return default;
                return (TResponseBody?)ApiRspHelper.Set(typeTResponseBody, obj);
            }
            throw;
        }
    }
}
