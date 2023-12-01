namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="IPAddress"/> 的序列化与反序列化实现
/// </summary>
public sealed class IPAddressFormatter :
    IMessagePackFormatter<IPAddress?>,
    IMemoryPackFormatter<IPAddress?>
{
    /// <summary>
    /// 默认的 <see cref="IPAddressFormatter"/> 实例
    /// </summary>
    public static readonly IPAddressFormatter Default = new();

    /// <summary>
    /// 对可空 <see cref="IPAddress"/> 类型进行序列化
    /// </summary>
    public void Serialize(ref MessagePackWriter writer, IPAddress? value, MessagePackSerializerOptions options)
    {
        writer.Write(value?.ToString());
    }

    /// <summary>
    /// 对可空 <see cref="IPAddress"/> 类型进行反序列化
    /// </summary>
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

    /// <summary>
    /// 将可空 <see cref="IPAddress"/> 对象序列化到 <see cref="MemoryPackWriter{TBufferWriter}"/>
    /// </summary>
    /// <typeparam name="TBufferWriter"></typeparam>
    void IMemoryPackFormatter<IPAddress?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref IPAddress? value)
    {
        var str = value?.ToString();
        writer.WriteString(str);
    }

    /// <summary>
    /// 将 <see cref="MemoryPackReader"/> 中的数据反序列化为可空 <see cref="IPAddress"/> 对象
    /// </summary>
    void IMemoryPackFormatter<IPAddress?>.Deserialize(ref MemoryPackReader reader, scoped ref IPAddress? value)
    {
        var str = reader.ReadString();
        value = IPAddress2.ParseNullable(str);
    }
}

/// <summary>
/// IPAddress 格式化程序特性
/// </summary>
public sealed class IPAddressFormatterAttribute : MemoryPackCustomFormatterAttribute<IPAddressFormatter, IPAddress?>
{
    /// <summary>
    /// 获取 <see cref="IPAddressFormatter.Default"/> 实例
    /// </summary>
    public sealed override IPAddressFormatter GetFormatter() => IPAddressFormatter.Default;

    /// <summary>
    /// 可空 <see cref="IPAddress"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<IPAddress?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="IPAddress"/> 对象序列化到 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref IPAddress? value)
        {
            IMemoryPackFormatter<IPAddress?> f = IPAddressFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="IPAddress"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref IPAddress? value)
        {
            IMemoryPackFormatter<IPAddress?> f = IPAddressFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}