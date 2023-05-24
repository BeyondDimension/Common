using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BD.Common.SourceGenerator.Models;

public sealed class AppSettings
{
    public static AppSettings Instance { get; internal set; } = null!;

    /// <summary>
    /// 命名空间
    /// </summary>
    public string Namespace { get; set; } = "";

    public EntityMetadata[] Entities { get; set; } = Array.Empty<EntityMetadata>();
}

[JsonSerializable(typeof(AppSettings))]
internal partial class AppSettingsContext : JsonSerializerContext
{
    static AppSettingsContext? instance;

    static JsonSerializerOptions GetDefaultOptions()
    {
        var o = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = true,
            IncludeFields = false,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        return o;
    }

    public static AppSettingsContext Instance
        => instance ??= new AppSettingsContext(GetDefaultOptions());
}