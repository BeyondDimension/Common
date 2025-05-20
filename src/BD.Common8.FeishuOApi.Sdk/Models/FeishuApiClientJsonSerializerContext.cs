using BD.Common8.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BD.Common8.FeishuOApi.Sdk.Models;

[JsonSerializable(typeof(SendMessage_RequestBody))]
[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    PropertyNameCaseInsensitive = true)]
sealed partial class FeishuApiClientJsonSerializerContext : JsonSerializerContext
{
    static FeishuApiClientJsonSerializerContext()
    {
        // https://github.com/dotnet/runtime/issues/94135
        s_defaultOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 不转义字符！！！
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
        };
        Default = new FeishuApiClientJsonSerializerContext(new JsonSerializerOptions(s_defaultOptions));
    }
}