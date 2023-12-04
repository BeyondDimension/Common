namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContent? GetMemoryPackContent<T>(T inputValue, MediaTypeHeaderValue? mediaType = null)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            OnSerializerError(ex, isSerializeOrDeserialize: true, typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual async Task<T?> ReadFromMemoryPackAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
            return default;
        }
    }
}
