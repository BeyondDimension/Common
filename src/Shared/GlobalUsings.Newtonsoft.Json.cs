// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using Newtonsoft.Json;
global using Newtonsoft.Json.Converters;
global using Newtonsoft.Json.Serialization;
global using Newtonsoft.Json.Linq;
// TODO: using NewtonsoftJsonXXX 缩减为 NJsonXXX
global using NewtonsoftJsonConverter = Newtonsoft.Json.JsonConverter;
global using NewtonsoftJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;
global using NewtonsoftJsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
global using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonSerializer;
global using NewtonsoftJsonFormatting = Newtonsoft.Json.Formatting;
global using NewtonsoftJsonConvert = Newtonsoft.Json.JsonConvert;
global using NewtonsoftJsonSerializerSettings = Newtonsoft.Json.JsonSerializerSettings;
global using NewtonsoftJsonConstructor = Newtonsoft.Json.JsonConstructorAttribute;
global using NewtonsoftJsonObject = Newtonsoft.Json.Linq.JObject;
global using NewtonsoftJsonPropertyClass = Newtonsoft.Json.Serialization.JsonProperty;
