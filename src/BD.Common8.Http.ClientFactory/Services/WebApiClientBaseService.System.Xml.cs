namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <inheritdoc cref="SerializableExtensions.GetXmlContent{T}(Serializable.IService, T, Encoding?, MediaTypeHeaderValue?)"/>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    protected virtual HttpContent? GetXmlContent<T>(T inputValue, Encoding? encoding = null, MediaTypeHeaderValue? mediaType = null)
    {
        return SerializableExtensions.GetXmlContent(this, inputValue, encoding, mediaType);
    }

    /// <inheritdoc cref="SerializableExtensions.ReadFromXmlAsync{T}(Serializable.IService, HttpContent, Encoding?, CancellationToken)"/>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    protected Task<T?> ReadFromXmlAsync<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        return SerializableExtensions.ReadFromXmlAsync<T>(this, content, encoding, cancellationToken);
    }

    /// <inheritdoc cref="SerializableExtensions.ReadFromXml{T}(Serializable.IService, HttpContent, Encoding?, CancellationToken)"/>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    [Obsolete(SerializableExtensions.Obsolete_UseAsync)]
    protected virtual T? ReadFromXml<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        return SerializableExtensions.ReadFromXml<T>(this, content, encoding, cancellationToken);
    }
}