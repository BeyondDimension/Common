namespace System.Text.Json.Serialization;

/// <summary>
/// 指针 Json 转换器，使用 <see cref="string"/> 避免精度问题
/// </summary>
public sealed class IntPtrConverter : JsonConverter<nint>
{
    /// <inheritdoc/>
    public override nint Read(ref Utf8JsonReader reader, Type typeToConvert, SystemTextJsonSerializerOptions options)
    {
        var valueString = Encoding.UTF8.GetString(reader.ValueSpan);
        if (long.TryParse(valueString, out var valueInt64))
        {
            var result = new nint(valueInt64);
            return result;
        }
        return default;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, nint value, SystemTextJsonSerializerOptions options)
    {
        var valueInt64 = value.ToInt64();
        var valueString = valueInt64.ToString();
        writer.WriteStringValue(valueString);
    }
}

/// <summary>
/// 指针 Json 转换器，使用 <see cref="string"/> 避免精度问题
/// </summary>
public sealed class UIntPtrConverter : JsonConverter<nuint>
{
    /// <inheritdoc/>
    public override nuint Read(ref Utf8JsonReader reader, Type typeToConvert, SystemTextJsonSerializerOptions options)
    {
        var valueString = Encoding.UTF8.GetString(reader.ValueSpan);
        if (ulong.TryParse(valueString, out var valueUInt64))
        {
            var result = new nuint(valueUInt64);
            return result;
        }
        return default;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, nuint value, SystemTextJsonSerializerOptions options)
    {
        var valueUInt64 = value.ToUInt64();
        var valueString = valueUInt64.ToString();
        writer.WriteStringValue(valueString);
    }
}
