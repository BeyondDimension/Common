namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContentWrapper<TResponseBody> GetMemoryPackContent<TResponseBody, TRequestBody>(TRequestBody inputValue, MediaTypeHeaderValue? mediaType = null)
        where TRequestBody : notnull
        where TResponseBody : notnull
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            return OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize: true, typeof(TRequestBody));
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<TResponseBody?> ReadFromMemoryPackAsync<TResponseBody>(HttpContent content, CancellationToken cancellationToken = default) where TResponseBody : notnull
    {
        try
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            return OnSerializerError<TResponseBody>(ex, isSerializeOrDeserialize: false, typeof(TResponseBody));
        }
    }
}
