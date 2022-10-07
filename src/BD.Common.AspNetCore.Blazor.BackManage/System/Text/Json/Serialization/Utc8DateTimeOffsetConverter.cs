// ReSharper disable once CheckNamespace
namespace System.Text.Json.Serialization;

public sealed class Utc8DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    static readonly TimeSpan utc8 = TimeSpan.FromHours(8);

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateTimeOffset.Parse(value!).ToOffset(utc8);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) => value.ToOffset(utc8).ToString("O");
}