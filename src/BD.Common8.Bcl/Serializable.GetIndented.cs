namespace System;

public static partial class Serializable // GetIndented(格式化缩进)
{
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string ToString(JsonDocument document, JsonWriterOptions options = default)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, options);
        document.WriteTo(writer);
        writer.Flush();
        var bytes = stream.ToArray();
        var result = Encoding.UTF8.GetString(bytes);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetIndentedBySystemTextJson(string json)
    {
        var jsonDoc = JsonDocument.Parse(json);
        var newJsonStr = ToString(jsonDoc, JsonSerializerCompatOptions.Writer.WriteIndented);
        return newJsonStr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetIndentedBySystemTextJson(Stream utf8Json)
    {
        var jsonDoc = JsonDocument.Parse(utf8Json);
        var newJsonStr = ToString(jsonDoc, JsonSerializerCompatOptions.Writer.WriteIndented);
        return newJsonStr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetIndentedBySystemTextJson(ReadOnlyMemory<byte> utf8Json)
    {
        var jsonDoc = JsonDocument.Parse(utf8Json);
        var newJsonStr = ToString(jsonDoc, JsonSerializerCompatOptions.Writer.WriteIndented);
        return newJsonStr;
    }

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetIndentedByNewtonsoftJson(string json)
    {
        var jsonObj = JObject.Parse(json);
        return NewtonsoftJsonConvert.SerializeObject(jsonObj, NewtonsoftJsonFormatting.Indented);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static JObject ParseJObject(Stream utf8Json, JsonLoadSettings? settings = null)
    {
        using JsonReader reader = new JsonTextReader(new StreamReader(utf8Json, Encoding.UTF8));
        JObject o = JObject.Load(reader, settings);

        while (reader.Read())
        {
            // Any content encountered here other than a comment will throw in the reader.
        }

        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetIndentedByNewtonsoftJson(Stream utf8Json)
    {
        var jsonObj = ParseJObject(utf8Json);
        return NewtonsoftJsonConvert.SerializeObject(jsonObj, NewtonsoftJsonFormatting.Indented);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetIndentedByNewtonsoftJson(ReadOnlyMemory<byte> utf8Json)
    {
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        using var utf8JsonStream =
           CommunityToolkit.HighPerformance.ReadOnlyMemoryExtensions.AsStream(utf8Json);
#else
        using var utf8JsonStream = new MemoryStream(utf8Json.ToArray());
#endif
        return GetIndentedByNewtonsoftJson(utf8JsonStream);
    }

    /// <summary>
    /// 将 Json 字符串格式化缩进
    /// </summary>
    /// <param name="json"></param>
    /// <param name="implType"></param>
    /// <returns></returns>
    public static string GetIndented(string? json,
        JsonImplType implType = JsonImplType.SystemTextJson)
    {
        string? jsonIndented = null;
        if (!string.IsNullOrWhiteSpace(json))
        {
            switch (implType)
            {
                case JsonImplType.SystemTextJson:
                    {
                        try
                        {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
                            jsonIndented = GetIndentedBySystemTextJson(json!);
#endif
                        }
                        catch
                        {
                        }
                    }
                    break;
            }
            if (jsonIndented == null)
            {
                try
                {
                    jsonIndented = GetIndentedByNewtonsoftJson(json!);
                }
                catch
                {
                }
            }
        }

        return jsonIndented ?? string.Empty;
    }

    /// <summary>
    /// 将 Json 字符串格式化缩进
    /// </summary>
    /// <param name="utf8Json"></param>
    /// <param name="implType"></param>
    /// <returns></returns>
    public static string GetIndented(Stream? utf8Json,
        JsonImplType implType = JsonImplType.SystemTextJson)
    {
        string? jsonIndented = null;
        if (utf8Json != null)
        {
            switch (implType)
            {
                case JsonImplType.SystemTextJson:
                    {
                        try
                        {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
                            jsonIndented = GetIndentedBySystemTextJson(utf8Json);
#endif
                        }
                        catch
                        {
                        }
                    }
                    break;
            }
            if (jsonIndented == null)
            {
                try
                {
                    jsonIndented = GetIndentedByNewtonsoftJson(utf8Json);
                }
                catch
                {
                }
            }
        }

        return jsonIndented ?? string.Empty;
    }

    /// <summary>
    /// 将 Json 字符串格式化缩进
    /// </summary>
    /// <param name="utf8Json"></param>
    /// <param name="implType"></param>
    /// <returns></returns>
    public static string GetIndented(ReadOnlyMemory<byte> utf8Json,
        JsonImplType implType = JsonImplType.SystemTextJson)
    {
        string? jsonIndented = null;
        switch (implType)
        {
            case JsonImplType.SystemTextJson:
                {
                    try
                    {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
                        jsonIndented = GetIndentedBySystemTextJson(utf8Json);
#endif
                    }
                    catch
                    {
                    }
                }
                break;
        }
        if (jsonIndented == null)
        {
            try
            {
                jsonIndented = GetIndentedByNewtonsoftJson(utf8Json);
            }
            catch
            {
            }
        }

        return jsonIndented ?? string.Empty;
    }
}