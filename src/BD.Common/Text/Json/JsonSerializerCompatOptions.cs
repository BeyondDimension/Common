namespace System.Text.Json;

public static class JsonSerializerCompatOptions
{
    static JsonSerializerOptions? _Default;

    public static JsonSerializerOptions Default => _Default ??= new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    static JsonSerializerOptions? _WriteIndented;

    public static JsonSerializerOptions WriteIndented => _WriteIndented ??= new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };
}
