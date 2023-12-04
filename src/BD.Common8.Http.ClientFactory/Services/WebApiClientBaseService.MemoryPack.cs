namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <inheritdoc cref="SerializableExtensions.GetMemoryPackContent{T}(Serializable.IService, T, MediaTypeHeaderValue?)"/>
    protected virtual HttpContent? GetMemoryPackContent<T>(T inputValue, MediaTypeHeaderValue? mediaType = null)
    {
        return SerializableExtensions.GetMemoryPackContent<T>(this, inputValue, mediaType);
    }

    /// <inheritdoc cref="SerializableExtensions.ReadFromMemoryPackAsync{T}(Serializable.IService, HttpContent, CancellationToken)"/>
    protected virtual Task<T?> ReadFromMemoryPackAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
    {
        return SerializableExtensions.ReadFromMemoryPackAsync<T>(this, content, cancellationToken);
    }
}
