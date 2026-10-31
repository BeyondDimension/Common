namespace System.Runtime.Serialization.Formatters;

public sealed class PortableDateTimeOffsetFormatter : MemoryPackFormatter<DateTimeOffset>
{
    public static readonly PortableDateTimeOffsetFormatter Default = new();

    // https://github.com/dotnet/runtime/blob/release/9.0/src/libraries/System.Private.CoreLib/src/System/DateTimeOffset.cs#L65
    // https://github.com/dotnet/runtime/blob/release/10.0/src/libraries/System.Private.CoreLib/src/System/DateTimeOffset.cs#L67

    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref DateTimeOffset value)
    {
        writer.WriteUnmanaged(value.Ticks);
        writer.WriteUnmanaged((short)value.Offset.TotalMinutes);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref DateTimeOffset value)
    {
        var ticks = reader.ReadUnmanaged<long>();
        var offsetMinutes = reader.ReadUnmanaged<short>();
        value = new DateTimeOffset(ticks, TimeSpan.FromMinutes(offsetMinutes));
    }
}

public sealed class PortableDateTimeOffsetFormatterAttribute : MemoryPackCustomFormatterAttribute<PortableDateTimeOffsetFormatter, DateTimeOffset>
{
    public override PortableDateTimeOffsetFormatter GetFormatter() => PortableDateTimeOffsetFormatter.Default;
}