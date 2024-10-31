namespace BD.Common8.FeishuOApi.Sdk.Models;

[JsonSerializable(typeof(SendMessage_RequestBody))]
[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    PropertyNameCaseInsensitive = true)]
sealed partial class FeishuApiClientJsonSerializerContext : SystemTextJsonSerializerContext
{
    static readonly Lazy<FeishuApiClientJsonSerializerContext> _Instance = new(() =>
    {
        SystemTextJsonSerializerOptions o = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 不转义字符！！！
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
        };
        return new FeishuApiClientJsonSerializerContext(o);
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    public static FeishuApiClientJsonSerializerContext Instance => _Instance.Value;
}