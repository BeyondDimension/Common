// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
global using System.Text.Encodings.Web;
global using System.Text.Unicode;
global using SystemTextJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
global using SystemTextJsonProperty = System.Text.Json.Serialization.JsonPropertyNameAttribute;
global using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
global using SystemTextJsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
global using SystemTextJsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;
global using SystemTextJsonConstructor = System.Text.Json.Serialization.JsonConstructorAttribute;
