using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BD.Common.Settings.Models;

public sealed class AppSettings
{
    public static AppSettings Instance { get; internal set; } = null!;

    /// <summary>
    /// 命名空间
    /// </summary>
    public string Namespace { get; set; } = "";

    /// <summary>
    /// 可选的生成 using 语句
    /// </summary>
    public string Usings { get; set; } = "";

    /// <summary>
    /// 需要识别为字典的类型
    /// </summary>
    public string[] DictionaryTypes { get; set; } = new[]
    {
        "ConcurrentDictionary<",
        "Dictionary<",
    };

    /// <summary>
    /// 需要识别为集合的类型
    /// </summary>
    public string[] CollectionTypes { get; set; } = new[]
    {
        "IList<",
        "List<",
        "HashSet<",
    };

    public Dictionary<string, SettingsMetadata> Settings { get; set; } = new();
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