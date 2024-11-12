#if !NETFRAMEWORK && !PROJ_SETUP
namespace BD.Common8.Http.ClientFactory.Services;

partial class SerializableService // MessagePack
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 MessagePack
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="mediaType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual HttpContentWrapper<TResponseBody> GetMessagePackContent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(TRequestBody inputValue, MediaTypeHeaderValue? mediaType = null, CancellationToken cancellationToken = default)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        var byteArray = Serializable.SMP(inputValue, cancellationToken);
        if (byteArray == null)
            return default;
        mediaType ??= new MediaTypeHeaderValue(MediaTypeNames.MessagePack);
        var httpContent = new ByteArrayContent(byteArray);
        httpContent.Headers.ContentType = mediaType;
        return httpContent;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 MessagePack
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="aes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<TResponseBody?> ReadFromMessagePackAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        HttpContent content,
        Aes? aes = null,
        CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        using var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var stream2 = aes != null ? new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read) : stream;
        try
        {
            var obj = await MessagePackSerializer.DeserializeAsync<TResponseBody>(stream2, options: Serializable.Lz4Options(), cancellationToken: cancellationToken);
            return obj;
        }
        catch
        {
            var typeTResponseBody = typeof(TResponseBody);
            if (typeTResponseBody.IsGenericType &&
                typeTResponseBody.GetGenericTypeDefinition() == typeof(ApiRspImpl<>))
            {
                var obj = await MessagePackSerializer.DeserializeAsync<ApiRspImpl>(stream2, options: Serializable.Lz4Options(), cancellationToken: cancellationToken);
                if (obj == null)
                    return default;
                return (TResponseBody?)ApiRspHelper.Set(typeTResponseBody, obj);
            }
            throw;
        }
    }
}
#endif