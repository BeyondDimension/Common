using MemoryPack;
using MessagePack;
using MessagePack.Formatters;
using System.Runtime.CompilerServices;
using Color = System.Drawing.Color;

namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="Color"/> 的序列化与反序列化实现
/// </summary>
public sealed class ColorFormatter :
    IMessagePackFormatter<Color>,
    //IMessagePackFormatter<SplatColor>,
    IMemoryPackFormatter<Color>,
    //IMemoryPackFormatter<SplatColor>,
    IMessagePackFormatter<Color?>,
    //IMessagePackFormatter<SplatColor?>,
    IMemoryPackFormatter<Color?>//,
                                //IMemoryPackFormatter<SplatColor?>
{
    /// <summary>
    /// 默认的 <see cref="ColorFormatter"/> 实例
    /// </summary>
    public static readonly ColorFormatter Default = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static uint ToUInt32(int value) => (uint)(value < 0 ? 0 : value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Color FromArgb(uint colorFromArgbValue)
    {
        if (colorFromArgbValue > int.MaxValue)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(colorFromArgbValue);
            return default;
        }
        else
        {
            return Color.FromArgb((int)colorFromArgbValue);
        }
    }

    /// <summary>
    /// 对 <see cref="Color"/> 类型进行序列化
    /// </summary>
    void IMessagePackFormatter<Color>.Serialize(ref MessagePackWriter writer, Color value, MessagePackSerializerOptions options)
    {
        MessagePackSerializer.Serialize(ref writer, ToUInt32(value.ToArgb()), options);
    }

    /// <summary>
    /// 对 <see cref="Color"/> 类型进行反序列化
    /// </summary>
    Color IMessagePackFormatter<Color>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
        return FromArgb(argb);
    }

    /// <summary>
    /// 对可空 <see cref="Color"/> 类型进行序列化
    /// </summary>
    void IMessagePackFormatter<Color?>.Serialize(ref MessagePackWriter writer, Color? value, MessagePackSerializerOptions options)
    {
        if (value.HasValue)
        {
            MessagePackSerializer.Serialize(ref writer, ToUInt32(value.Value.ToArgb()), options);
        }
        else
        {
            writer.WriteNil();
        }
    }

    /// <summary>
    /// 对可空 <see cref="Color"/> 类型进行反序列化
    /// </summary>
    Color? IMessagePackFormatter<Color?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }
        else
        {
            var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
            return FromArgb(argb);
        }
    }

    //void IMessagePackFormatter<SplatColor>.Serialize(ref MessagePackWriter writer, SplatColor value, MessagePackSerializerOptions options)
    //{
    //    MessagePackSerializer.Serialize(ref writer, value.ToArgb(), options);
    //}

    //SplatColor IMessagePackFormatter<SplatColor>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    //{
    //    var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
    //    return SplatColor.FromArgb(argb);
    //}

    //void IMessagePackFormatter<SplatColor?>.Serialize(ref MessagePackWriter writer, SplatColor? value, MessagePackSerializerOptions options)
    //{
    //    if (value.HasValue)
    //    {
    //        MessagePackSerializer.Serialize(ref writer, value.Value.ToArgb(), options);
    //    }
    //    else
    //    {
    //        writer.WriteNil();
    //    }
    //}

    //SplatColor? IMessagePackFormatter<SplatColor?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    //{
    //    if (reader.TryReadNil())
    //    {
    //        return default;
    //    }
    //    else
    //    {
    //        var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
    //        return SplatColor.FromArgb(argb);
    //    }
    //}

    /// <summary>
    /// 将 <see cref="Color"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/>
    /// </summary>
    /// <typeparam name="TBufferWriter"></typeparam>
    void IMemoryPackFormatter<Color>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color value)
    {
        writer.WriteVarInt(ToUInt32(value.ToArgb()));
    }

    /// <summary>
    /// 将 <see cref="MemoryPackReader"/> 中的数据反序列化为 <see cref="Color"/> 对象
    /// </summary>
    void IMemoryPackFormatter<Color>.Deserialize(ref MemoryPackReader reader, scoped ref Color value)
    {
        var argb = reader.ReadVarIntUInt32();
        value = FromArgb(argb);
    }

    //void IMemoryPackFormatter<SplatColor>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor value)
    //{
    //    writer.WriteVarInt(value.ToArgb());
    //}

    //void IMemoryPackFormatter<SplatColor>.Deserialize(ref MemoryPackReader reader, scoped ref SplatColor value)
    //{
    //    var argb = reader.ReadVarIntUInt32();
    //    value = SplatColor.FromArgb(argb);
    //}

    /// <summary>
    /// 将可空 <see cref="Color"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/>
    /// </summary>
    /// <typeparam name="TBufferWriter"></typeparam>
    void IMemoryPackFormatter<Color?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color? value)
    {
        if (value.HasValue)
        {
            writer.WriteVarInt(ToUInt32(value.Value.ToArgb()));
        }
        else
        {
            writer.WriteNullObjectHeader();
        }
    }

    /// <summary>
    /// 将 <see cref="MemoryPackReader"/> 中的数据反序列化为可空 <see cref="Color"/> 对象
    /// </summary>
    void IMemoryPackFormatter<Color?>.Deserialize(ref MemoryPackReader reader, scoped ref Color? value)
    {
        if (reader.PeekIsNull())
        {
            value = default;
        }
        else
        {
            var argb = reader.ReadVarIntUInt32();
            value = FromArgb(argb);
        }
    }

    //void IMemoryPackFormatter<SplatColor?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor? value)
    //{
    //    if (value.HasValue)
    //    {
    //        writer.WriteVarInt(value.Value.ToArgb());
    //    }
    //    else
    //    {
    //        writer.WriteNullObjectHeader();
    //    }
    //}

    //void IMemoryPackFormatter<SplatColor?>.Deserialize(ref MemoryPackReader reader, scoped ref SplatColor? value)
    //{
    //    if (reader.PeekIsNull())
    //    {
    //        value = default;
    //    }
    //    else
    //    {
    //        var argb = reader.ReadVarIntUInt32();
    //        value = SplatColor.FromArgb(argb);
    //    }
    //}
}

/// <summary>
/// 颜色格式化程序特性
/// </summary>
public sealed class ColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, Color>
{
    /// <summary>
    /// 获取 <see cref="ColorFormatter.Default"/> 实例
    /// </summary>
    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

    /// <summary>
    /// <see cref="Color"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<Color>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将 <paramref name="value"/> 对象序列化到 <paramref name="writer"/> 中
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color value)
        {
            IMemoryPackFormatter<Color> f = ColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <paramref name="reader"/> 中反序列化 <paramref name="value"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Color value)
        {
            IMemoryPackFormatter<Color> f = ColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

//public sealed class SplatColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, SplatColor>
//{
//    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

//    public sealed class Formatter : MemoryPackFormatter<SplatColor>
//    {
//        public static readonly Formatter Default = new();

//        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor value)
//        {
//            IMemoryPackFormatter<SplatColor> f = ColorFormatter.Default;
//            f.Serialize(ref writer, ref value);
//        }

//        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor value)
//        {
//            IMemoryPackFormatter<SplatColor> f = ColorFormatter.Default;
//            f.Deserialize(ref reader, ref value);
//        }
//    }
//}

/// <summary>
/// 可为空的颜色格式化程序特性
/// </summary>
public sealed class NullableColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, Color?>
{
    /// <summary>
    /// 获取 <see cref="ColorFormatter.Default"/> 实例
    /// </summary>
    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

    /// <summary>
    /// 可为空的 <see cref="Color"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<Color?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <paramref name="value"/> 对象序列化到 <paramref name="writer"/> 中
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color? value)
        {
            IMemoryPackFormatter<Color?> f = ColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <paramref name="reader"/> 中反序列化可空 <paramref name="value"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Color? value)
        {
            IMemoryPackFormatter<Color?> f = ColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

//public sealed class NullableSplatColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, SplatColor?>
//{
//    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

//    public sealed class Formatter : MemoryPackFormatter<SplatColor?>
//    {
//        public static readonly Formatter Default = new();

//        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor? value)
//        {
//            IMemoryPackFormatter<SplatColor?> f = ColorFormatter.Default;
//            f.Serialize(ref writer, ref value);
//        }

//        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor? value)
//        {
//            IMemoryPackFormatter<SplatColor?> f = ColorFormatter.Default;
//            f.Deserialize(ref reader, ref value);
//        }
//    }
//}