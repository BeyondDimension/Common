namespace System.Text.Json.Serialization;

/// <summary>
/// 将 <see cref="string"/> 值转换为 <see cref="decimal"/>
/// </summary>
public sealed class StringToDecimalConverter : JsonConverter<decimal>
{
    /// <inheritdoc/>
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, SystemTextJsonSerializerOptions options)
    {
        var valueString = Encoding.UTF8.GetString(reader.ValueSpan);
        var result = decimal.TryParse(valueString, out var valueDecimal) ? valueDecimal : 0m;
        return result;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, decimal value, SystemTextJsonSerializerOptions options)
    {
        var valueString = value.ToString();
        writer.WriteStringValue(valueString);
    }
}
