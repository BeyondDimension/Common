using Splat;
using System.Drawing;

namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="Color"/>, <see cref="SplatColor"/> 的序列化与反序列化实现
/// </summary>
public sealed class ColorFormatter :
    IMessagePackFormatter<Color>,
    IMessagePackFormatter<SplatColor>,
    IMemoryPackFormatter<Color>,
    IMemoryPackFormatter<SplatColor>,
    IMessagePackFormatter<Color?>,
    IMessagePackFormatter<SplatColor?>,
    IMemoryPackFormatter<Color?>,
    IMemoryPackFormatter<SplatColor?>
{
    public static readonly ColorFormatter Default = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static uint ToUInt32(int value) => (uint)(value < 0 ? 0 : value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Color FromArgb(uint value)
    {
        if (value > int.MaxValue)
        {
            var color = SplatColor.FromArgb(value);
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        else
        {
            return Color.FromArgb((int)value);
        }
    }

    void IMessagePackFormatter<Color>.Serialize(ref MessagePackWriter writer, Color value, MessagePackSerializerOptions options)
    {
        MessagePackSerializer.Serialize(ref writer, ToUInt32(value.ToArgb()), options);
    }

    Color IMessagePackFormatter<Color>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
        return FromArgb(argb);
    }

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

    void IMessagePackFormatter<SplatColor>.Serialize(ref MessagePackWriter writer, SplatColor value, MessagePackSerializerOptions options)
    {
        MessagePackSerializer.Serialize(ref writer, value.ToArgb(), options);
    }

    SplatColor IMessagePackFormatter<SplatColor>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
        return SplatColor.FromArgb(argb);
    }

    void IMessagePackFormatter<SplatColor?>.Serialize(ref MessagePackWriter writer, SplatColor? value, MessagePackSerializerOptions options)
    {
        if (value.HasValue)
        {
            MessagePackSerializer.Serialize(ref writer, value.Value.ToArgb(), options);
        }
        else
        {
            writer.WriteNil();
        }
    }

    SplatColor? IMessagePackFormatter<SplatColor?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }
        else
        {
            var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
            return SplatColor.FromArgb(argb);
        }
    }

    void IMemoryPackFormatter<Color>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color value)
    {
        writer.WriteVarInt(ToUInt32(value.ToArgb()));
    }

    void IMemoryPackFormatter<Color>.Deserialize(ref MemoryPackReader reader, scoped ref Color value)
    {
        var argb = reader.ReadVarIntUInt32();
        value = FromArgb(argb);
    }

    void IMemoryPackFormatter<SplatColor>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor value)
    {
        writer.WriteVarInt(value.ToArgb());
    }

    void IMemoryPackFormatter<SplatColor>.Deserialize(ref MemoryPackReader reader, scoped ref SplatColor value)
    {
        var argb = reader.ReadVarIntUInt32();
        value = SplatColor.FromArgb(argb);
    }

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

    void IMemoryPackFormatter<SplatColor?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor? value)
    {
        if (value.HasValue)
        {
            writer.WriteVarInt(value.Value.ToArgb());
        }
        else
        {
            writer.WriteNullObjectHeader();
        }
    }

    void IMemoryPackFormatter<SplatColor?>.Deserialize(ref MemoryPackReader reader, scoped ref SplatColor? value)
    {
        if (reader.PeekIsNull())
        {
            value = default;
        }
        else
        {
            var argb = reader.ReadVarIntUInt32();
            value = SplatColor.FromArgb(argb);
        }
    }
}

public sealed class ColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, Color>
{
    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<Color>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color value)
        {
            IMemoryPackFormatter<Color> f = ColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Color value)
        {
            IMemoryPackFormatter<Color> f = ColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class SplatColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, SplatColor>
{
    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<SplatColor>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor value)
        {
            IMemoryPackFormatter<SplatColor> f = ColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor value)
        {
            IMemoryPackFormatter<SplatColor> f = ColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class NullableColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, Color?>
{
    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<Color?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color? value)
        {
            IMemoryPackFormatter<Color?> f = ColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Color? value)
        {
            IMemoryPackFormatter<Color?> f = ColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class NullableSplatColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter, SplatColor?>
{
    public sealed override ColorFormatter GetFormatter() => ColorFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<SplatColor?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor? value)
        {
            IMemoryPackFormatter<SplatColor?> f = ColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor? value)
        {
            IMemoryPackFormatter<SplatColor?> f = ColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}