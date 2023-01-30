namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="IPAddress"/> 的序列化与反序列化实现
/// </summary>
public sealed class IPAddressFormatter :
    IMessagePackFormatter<IPAddress?>,
    IMemoryPackFormatter<IPAddress?>
{
    public static readonly IPAddressFormatter Default = new();

    public void Serialize(ref MessagePackWriter writer, IPAddress? value, MessagePackSerializerOptions options)
    {
        writer.Write(value?.ToString());
    }

    public IPAddress? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        options.Security.DepthStep(ref reader);

        var value = reader.ReadString();

        reader.Depth--;
        return IPAddress2.ParseNullable(value);
    }

    void IMemoryPackFormatter<IPAddress?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref IPAddress? value)
    {
        var str = value?.ToString();
        writer.WriteString(str);
    }

    void IMemoryPackFormatter<IPAddress?>.Deserialize(ref MemoryPackReader reader, scoped ref IPAddress? value)
    {
        var str = reader.ReadString();
        value = IPAddress2.ParseNullable(str);
    }
}

public sealed class IPAddressFormatterAttribute : MemoryPackCustomFormatterAttribute<IPAddressFormatter, IPAddress?>
{
    public sealed override IPAddressFormatter GetFormatter() => IPAddressFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<IPAddress?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref IPAddress? value)
        {
            IMemoryPackFormatter<IPAddress?> f = IPAddressFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref IPAddress? value)
        {
            IMemoryPackFormatter<IPAddress?> f = IPAddressFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}