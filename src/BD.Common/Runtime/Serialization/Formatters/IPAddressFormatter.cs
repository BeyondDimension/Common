using MessagePack;
using MessagePack.Formatters;

namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="IPAddress"/> 的序列化与反序列化实现
/// </summary>
public sealed class IPAddressFormatter : IMessagePackFormatter<IPAddress?>
{
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
}
