namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = newtonsoftJsonSerializer;

    /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
    protected NewtonsoftJsonSerializer NewtonsoftJsonSerializer => newtonsoftJsonSerializer ??= new();

    /// <inheritdoc/>
    NewtonsoftJsonSerializer Serializable.IService.NewtonsoftJsonSerializer => NewtonsoftJsonSerializer;

    /// <inheritdoc cref="SerializableExtensions.GetNJsonContent{T}(Serializable.IService, T, Encoding?, MediaTypeHeaderValue?)"/>
    [Obsolete(SerializableExtensions.Obsolete_GetNJsonContent)]
    protected virtual HttpContent? GetNJsonContent<T>(T inputValue, Encoding? encoding = null, MediaTypeHeaderValue? mediaType = null)
    {
        return SerializableExtensions.GetNJsonContent(this, inputValue, encoding, mediaType);
    }

    /// <inheritdoc cref="SerializableExtensions.ReadFromNJsonAsync{T}(Serializable.IService, HttpContent, Encoding?, CancellationToken)"/>
    [Obsolete(SerializableExtensions.Obsolete_ReadFromNJsonAsync)]
    protected virtual Task<T?> ReadFromNJsonAsync<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        return SerializableExtensions.ReadFromNJsonAsync<T>(this, content, encoding, cancellationToken);
    }

    /// <inheritdoc cref="SerializableExtensions.ReadFromNJson{T}(Serializable.IService, HttpContent, Encoding?, CancellationToken)"/>
    [Obsolete(SerializableExtensions.Obsolete_UseAsync)]
    protected virtual T? ReadFromNJson<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        return SerializableExtensions.ReadFromNJson<T>(this, content, encoding, cancellationToken);
    }
}
