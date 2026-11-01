namespace System.Runtime.Serialization.Formatters;

public sealed class PortableDateTimeOffsetFormatter : MemoryPackFormatter<DateTimeOffset>
{
    public static readonly PortableDateTimeOffsetFormatter Default = new();

    // https://github.com/dotnet/runtime/blob/release/9.0/src/libraries/System.Private.CoreLib/src/System/DateTimeOffset.cs#L65
    // https://github.com/dotnet/runtime/blob/release/10.0/src/libraries/System.Private.CoreLib/src/System/DateTimeOffset.cs#L67

    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref DateTimeOffset value)
    {
        var raw = default(LegacyDateTimeOffsetLayout);
        raw.OffsetMinutes = (short)(int)value.Offset.TotalMinutes;
        raw.UtcTicks = value.UtcTicks;
        writer.WriteUnmanaged(raw);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref DateTimeOffset value)
    {
        var raw = reader.ReadUnmanaged<LegacyDateTimeOffsetLayout>();
        value = new DateTimeOffset(raw.UtcTicks, TimeSpan.Zero).ToOffset(TimeSpan.FromMinutes(raw.OffsetMinutes));
    }
}

[StructLayout(LayoutKind.Explicit, Size = 16)]
internal struct LegacyDateTimeOffsetLayout
{
    [FieldOffset(0)]
    public short OffsetMinutes;

    [FieldOffset(8)]
    public long UtcTicks;
}

public sealed class PortableDateTimeOffsetFormatterAttribute : MemoryPackCustomFormatterAttribute<PortableDateTimeOffsetFormatter, DateTimeOffset>
{
    public override PortableDateTimeOffsetFormatter GetFormatter() => PortableDateTimeOffsetFormatter.Default;
}