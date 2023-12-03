namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="encoding"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual HttpContent? GetXmlContent<T>(T inputValue, Encoding? encoding = null, MediaTypeHeaderValue? mediaType = null)
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

    static readonly ConcurrentDictionary<Type, XmlSerializer> xmlSerializers = new(); // 缓存实例，避免多次创建

    /// <summary>
    /// 根据类型获取 <see cref="XmlSerializer"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public static XmlSerializer GetXmlSerializer(Type type)
    {
        if (!xmlSerializers.TryGetValue(type, out var value))
        {
            value = new XmlSerializer(type);
            xmlSerializers.TryAdd(type, value);
        }
        return value;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    protected virtual async Task<T?> ReadFromXmlAsync<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        try
        {
            encoding ??= DefaultEncoding;
            var xmlSerializer = GetXmlSerializer(typeof(T));
            using var contentStream = await content.ReadAsStreamAsync(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
            using var contentStreamReader = new StreamReader(contentStream, encoding);
            var result = xmlSerializer.Deserialize(contentStreamReader);
            return (T?)result;
        }
        catch (Exception ex)
        {
            OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    [Obsolete("无特殊情况下应使用 Async 异步的函数版本")]
    protected virtual T? ReadFromXml<T>(HttpContent content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        try
        {
            encoding ??= DefaultEncoding;
            var xmlSerializer = GetXmlSerializer(typeof(T));
            using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
            using var contentStreamReader = new StreamReader(contentStream, encoding);
            var result = xmlSerializer.Deserialize(contentStreamReader);
            return (T?)result;
        }
        catch (Exception ex)
        {
            OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
            return default;
        }
    }
}