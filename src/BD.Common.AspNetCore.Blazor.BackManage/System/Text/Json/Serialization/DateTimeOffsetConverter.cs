//// ReSharper disable once CheckNamespace
//namespace System.Text.Json.Serialization;

//public sealed class LocalTimeDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
//{
//    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        var value = reader.GetString();
//        return DateTimeOffset.Parse(value!).ToLocalTime();
//    }

//    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) => value.ToLocalTime().ToString("O");
//}