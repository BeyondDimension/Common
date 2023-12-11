namespace System.Text.Json.Serialization;

/// <summary>
/// 对类型 T 使用 MemoryPack Base64Bytes 的序列化与反序列化实现
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MemoryPackToJsonConverter<T> : JsonConverter<T> where T : notnull
{
    /// <inheritdoc cref="IMemoryPackFormatter{T}"/>
    protected abstract IMemoryPackFormatter<T?> MemoryPackFormatter { get; }

    /// <inheritdoc/>
    public sealed override T? Read(ref Utf8JsonReader reader, Type typeToConvert, SystemTextJsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.None:
            case JsonTokenType.Null:
                return default;
        }
        var base64Bytes = reader.GetBytesFromBase64();
        using var memoryPackReaderOptionalState = MemoryPackReaderOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var memoryPackReader = new MemoryPackReader(base64Bytes, memoryPackReaderOptionalState);
        IMemoryPackFormatter<T?> f = MemoryPackFormatter;
        T? value = default;
        f.Deserialize(ref memoryPackReader, ref value);
        return value;
    }

    /// <inheritdoc/>
    public sealed override void Write(Utf8JsonWriter writer, T? value, SystemTextJsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        using var memoryPackWriterOptionalState = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var memoryPackWriter = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref arrayBufferWriter, memoryPackWriterOptionalState);
        IMemoryPackFormatter<T?> f = MemoryPackFormatter;
        f.Serialize(ref memoryPackWriter, ref value);
        writer.WriteBase64StringValue(arrayBufferWriter.WrittenSpan.ToArray());
        arrayBufferWriter.Clear();
    }
}
