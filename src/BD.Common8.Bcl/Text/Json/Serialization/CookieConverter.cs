namespace System.Text.Json.Serialization;

/// <summary>
/// 对类型 <see cref="Cookie"/> 的序列化与反序列化实现
/// </summary>
public sealed class CookieConverter : MemoryPackToJsonConverter<Cookie>
{
    /// <inheritdoc/>
    protected override IMemoryPackFormatter<Cookie?> MemoryPackFormatter
        => CookieFormatter.Default;
}

/// <summary>
/// 对类型 <see cref="CookieCollection"/> 的序列化与反序列化实现
/// </summary>
public sealed class CookieCollectionConverter : MemoryPackToJsonConverter<CookieCollection>
{
    /// <inheritdoc/>
    protected override IMemoryPackFormatter<CookieCollection?> MemoryPackFormatter
        => CookieFormatter.Default;
}

/// <summary>
/// 对类型 <see cref="CookieContainer"/> 的序列化与反序列化实现
/// </summary>
public sealed class CookieContainerConverter : MemoryPackToJsonConverter<CookieContainer>
{
    /// <inheritdoc/>
    protected override IMemoryPackFormatter<CookieContainer?> MemoryPackFormatter
        => CookieFormatter.Default;
}
