using Splat;
using System.Buffers;
using System.Drawing;
using static System.Runtime.Serialization.Formatters.H;

namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="Color"/>, <see cref="SplatColor"/> 的序列化与反序列化实现
/// </summary>
[Obsolete("use ColorFormatter2", true)]
public sealed class ColorFormatter
{
}

file static class H
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ToUInt32(int value) => (uint)(value < 0 ? 0 : value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color FromArgb(uint value)
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
}

public sealed class ColorFormatter2 : IMessagePackFormatter<Color>, IMemoryPackFormatter<Color>
{
    public static readonly ColorFormatter2 Default = new();

    public void Serialize(ref MessagePackWriter writer, Color value, MessagePackSerializerOptions options)
    {
        MessagePackSerializer.Serialize(ref writer, ToUInt32(value.ToArgb()), options);
    }

    public Color Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
        return FromArgb(argb);
    }

    public void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color value)
        where TBufferWriter : IBufferWriter<byte>
    {
        writer.WriteVarInt(ToUInt32(value.ToArgb()));
    }

    public void Deserialize(ref MemoryPackReader reader, scoped ref Color value)
    {
        var argb = reader.ReadVarIntUInt32();
        value = FromArgb(argb);
    }
}

#pragma warning disable MsgPack009 // Colliding formatters
public sealed class NullableColorFormatter : IMessagePackFormatter<Color?>, IMemoryPackFormatter<Color?>
#pragma warning restore MsgPack009 // Colliding formatters
{
    public static readonly NullableColorFormatter Default = new();

    public void Serialize(ref MessagePackWriter writer, Color? value, MessagePackSerializerOptions options)
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

    public Color? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

    public void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color? value)
        where TBufferWriter : IBufferWriter<byte>
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

    public void Deserialize(ref MemoryPackReader reader, scoped ref Color? value)
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
}

public sealed class SplatColorFormatter : IMessagePackFormatter<SplatColor>, IMemoryPackFormatter<SplatColor>
{
    public static readonly SplatColorFormatter Default = new();

    public void Serialize(ref MessagePackWriter writer, SplatColor value, MessagePackSerializerOptions options)
    {
        MessagePackSerializer.Serialize(ref writer, value.ToArgb(), options);
    }

    public SplatColor Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var argb = MessagePackSerializer.Deserialize<uint>(ref reader, options);
        return SplatColor.FromArgb(argb);
    }

    public void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor value)
        where TBufferWriter : IBufferWriter<byte>
    {
        writer.WriteVarInt(value.ToArgb());
    }

    public void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor value)
    {
        var argb = reader.ReadVarIntUInt32();
        value = SplatColor.FromArgb(argb);
    }
}

#pragma warning disable MsgPack009 // Colliding formatters
public sealed class NullableSplatColorFormatter : IMessagePackFormatter<SplatColor?>, IMemoryPackFormatter<SplatColor?>
#pragma warning restore MsgPack009 // Colliding formatters
{
    public static readonly NullableSplatColorFormatter Default = new();

    public void Serialize(ref MessagePackWriter writer, SplatColor? value, MessagePackSerializerOptions options)
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

    public SplatColor? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

    public void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor? value)
        where TBufferWriter : IBufferWriter<byte>
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

    public void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor? value)
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

public sealed class ColorFormatterAttribute : MemoryPackCustomFormatterAttribute<ColorFormatter2, Color>
{
    public sealed override ColorFormatter2 GetFormatter() => ColorFormatter2.Default;

    public sealed class Formatter : MemoryPackFormatter<Color>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color value)
        {
            IMemoryPackFormatter<Color> f = ColorFormatter2.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Color value)
        {
            IMemoryPackFormatter<Color> f = ColorFormatter2.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class SplatColorFormatterAttribute : MemoryPackCustomFormatterAttribute<SplatColorFormatter, SplatColor>
{
    public sealed override SplatColorFormatter GetFormatter() => SplatColorFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<SplatColor>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor value)
        {
            IMemoryPackFormatter<SplatColor> f = SplatColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor value)
        {
            IMemoryPackFormatter<SplatColor> f = SplatColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class NullableColorFormatterAttribute : MemoryPackCustomFormatterAttribute<NullableColorFormatter, Color?>
{
    public sealed override NullableColorFormatter GetFormatter() => NullableColorFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<Color?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color? value)
        {
            IMemoryPackFormatter<Color?> f = NullableColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Color? value)
        {
            IMemoryPackFormatter<Color?> f = NullableColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class NullableSplatColorFormatterAttribute : MemoryPackCustomFormatterAttribute<NullableSplatColorFormatter, SplatColor?>
{
    public sealed override NullableSplatColorFormatter GetFormatter() => NullableSplatColorFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<SplatColor?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SplatColor? value)
        {
            IMemoryPackFormatter<SplatColor?> f = NullableSplatColorFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref SplatColor? value)
        {
            IMemoryPackFormatter<SplatColor?> f = NullableSplatColorFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}