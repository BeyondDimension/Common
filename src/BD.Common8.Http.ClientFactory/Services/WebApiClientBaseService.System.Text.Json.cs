namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <inheritdoc cref="SerializableExtensions.GetSJsonContent{T}(Serializable.IService, T, JsonTypeInfo{T}?, MediaTypeHeaderValue?)"/>
    protected virtual HttpContent? GetSJsonContent<T>(T inputValue, JsonTypeInfo<T>? jsonTypeInfo = null, MediaTypeHeaderValue? mediaType = null)
    {
        return SerializableExtensions.GetSJsonContent(this, inputValue, jsonTypeInfo, mediaType);
    }

    /// <inheritdoc cref="Serializable.IService.RequiredJsonTypeInfo"/>
    protected virtual bool RequiredJsonTypeInfo => true;

    /// <inheritdoc cref="Serializable.IService.RequiredJsonTypeInfo"/>
    bool Serializable.IService.RequiredJsonTypeInfo => RequiredJsonTypeInfo;

    /// <inheritdoc cref="Serializable.IService.JsonTypeInfo"/>
    protected virtual JsonTypeInfo? JsonTypeInfo { get; }

    /// <inheritdoc cref="Serializable.IService.JsonTypeInfo"/>
    JsonTypeInfo? Serializable.IService.JsonTypeInfo => JsonTypeInfo;

    /// <inheritdoc cref="SerializableExtensions.ReadFromSJsonAsync{T}(Serializable.IService, HttpContent, JsonTypeInfo{T}?, CancellationToken)"/>
    protected virtual Task<T?> ReadFromSJsonAsync<T>(HttpContent content, JsonTypeInfo<T>? jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        return SerializableExtensions.ReadFromSJsonAsync(this, content, jsonTypeInfo, cancellationToken);
    }

    /// <inheritdoc cref="SerializableExtensions.ReadFromSJson{T}(Serializable.IService, HttpContent, JsonTypeInfo{T}?, CancellationToken)"/>
    [Obsolete(SerializableExtensions.Obsolete_UseAsync)]
    protected virtual T? ReadFromSJson<T>(HttpContent content, JsonTypeInfo<T>? jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        return SerializableExtensions.ReadFromSJson(this, content, jsonTypeInfo, cancellationToken);
    }
}
