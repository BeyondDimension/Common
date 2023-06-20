#if __HAVE_S_JSON__
#else
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
#endif

namespace BD.Common.Repositories.SourceGenerator.Models;

/// <summary>
/// 源生成配置模型
/// </summary>
public sealed record class GeneratorConfig(
    ConcurrentDictionary<string, string> Translates,
    ImmutableHashSet<string> AttributeTypeFullNames,
    string? ApiBaseUrlBackManageLocal,
    string? ApiBaseUrlBackManageDevelopment,
    Dictionary<string, string[]> SourcePath)
{
    static readonly Dictionary<string, string> DefTranslates = new()
    {
        { "名称", "Name" },
        { "文件类型", "FileType" },
        { "邮箱", "Email" },
        { "文件大小", "FileSize" },
        { "文件路径", "FilePath" },
        { "文件后缀名", "FileExtension" },
        { "文件名", "FileName" },
    };

    static readonly Lazy<byte[]> _GetApiBaseUrlBackManageLocal = new(() =>
    {
        var value = Instance.ApiBaseUrlBackManageLocal;
        if (string.IsNullOrWhiteSpace(value))
            return "https://localhost:7129"u8.ToArray();
        return Encoding.UTF8.GetBytes(value!.TrimEnd("/"));
    });

    public static byte[] GetApiBaseUrlBackManageLocal() => _GetApiBaseUrlBackManageLocal.Value;

    static readonly Lazy<byte[]> _GetApiBaseUrlBackManageDevelopment = new(() =>
    {
        var value = Instance.ApiBaseUrlBackManageDevelopment;
        if (string.IsNullOrWhiteSpace(value))
            return ""u8.ToArray();
        return Encoding.UTF8.GetBytes(value!.TrimEnd("/"));
    });

    public static byte[] GetApiBaseUrlBackManageDevelopment() => _GetApiBaseUrlBackManageDevelopment.Value;

    /// <summary>
    /// 配置文件名
    /// </summary>
    const string fileName = "GeneratorConfig.Repositories.json";

    /// <summary>
    /// 配置文件路径
    /// </summary>
    internal static readonly Lazy<string> FilePath = new(() =>
    {
        var projPath = ProjPathHelper.GetProjPath(null);
        var filePath = Path.Combine(projPath, "src", fileName);
        return filePath;
    });

    public static ImmutableDictionary<string, string> AttrTypeFullNames { get; private set; } = null!;

    static readonly Lazy<GeneratorConfig> instance = new(() =>
    {
        using var stream = new FileStream(
            FilePath.Value,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);

        #region Deserialize

#if __HAVE_S_JSON__
        var generatorConfig = JsonSerializer.Deserialize(stream, GeneratorConfigContext.Instance.GeneratorConfig);
#else
        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        using var jsonTextReader = new JsonTextReader(streamReader);
        var generatorConfig = JsonSerializer.CreateDefault().Deserialize<GeneratorConfig>(jsonTextReader);
#endif
        if (generatorConfig == null)
            throw new ArgumentNullException(nameof(generatorConfig));

        #endregion

        #region Translates

        foreach (var item in DefTranslates)
        {
            generatorConfig.Translates.TryAdd(item.Key, item.Value);
        }

        #endregion

        #region AttributeTypeFullNames

        var attrTypeFullNames = typeof(TypeFullNames).
            GetFields(BindingFlags.Public | BindingFlags.Static).
            ToDictionary(static x => x.GetValue(null).ToString(), static x => x.Name);

        var attrTypeFullNamesCfg = generatorConfig.AttributeTypeFullNames;
        if (attrTypeFullNamesCfg != null)
            foreach (var key_ in attrTypeFullNamesCfg)
            {
                if (string.IsNullOrWhiteSpace(key_)) continue;
                var key = key_;
                try
                {
                    if (!key.EndsWith("Attribute"))
                        key = $"{key}Attribute";
                    var indexD = key.LastIndexOf('.');
                    if (indexD == -1) continue;
                    var value = key.Substring(indexD, key.Length - "Attribute".Length);
                    if (!attrTypeFullNames.ContainsKey(key))
                        attrTypeFullNames.Add(key, value);
                }
                catch
                {
                    continue;
                }
            }
        AttrTypeFullNames = attrTypeFullNames.ToImmutableDictionary();

        #endregion

        counter = generatorConfig.GetCounter();
        return generatorConfig;
    });

    /// <summary>
    /// 获取当前配置
    /// </summary>
    public static GeneratorConfig Instance => instance.Value;

    /// <summary>
    /// 翻译
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Translate(string input)
    {
        if (!input.HasOther()) return input; // 仅英文字母与数字组合的名称不需要翻译
        var instance = Instance;
        if (instance.Translates.TryGetValue(input, out var result))
        {
            if (!string.IsNullOrEmpty(result)) return result;
        }
        else
        {
            instance.Translates.TryAdd(input, string.Empty);
        }
        return input;
    }

    static readonly object @lock = new();

    /// <summary>
    /// 保存当前配置到文件
    /// </summary>
    public static void Save(bool force = false)
    {
        var instance = Instance;
        if (!force && counter == instance.GetCounter()) return; // 通过计数器判断是否有变更需要保存
        lock (@lock)
        {
            using var stream = new FileStream(
                FilePath.Value,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.ReadWrite | FileShare.Delete);
#if __HAVE_S_JSON__
            JsonSerializer.Serialize(stream, instance, GeneratorConfigContext.Instance.GeneratorConfig);
#else
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            using var jsonTextWriter = new JsonTextWriter(streamWriter);
            JsonSerializer.CreateDefault(
                new JsonSerializerSettings() { Formatting = Formatting.Indented })
                .Serialize(jsonTextWriter, instance);
#endif
            stream.Flush();
            stream.SetLength(stream.Position);
        }
    }

    static int counter;

    int GetCounter()
    {
        return Translates.Count;
    }
}

#if __HAVE_S_JSON__
[JsonSerializable(typeof(GeneratorConfig))]
internal partial class GeneratorConfigContext : JsonSerializerContext
{
    static JsonSerializerOptions DefaultOptions { get; } = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        IncludeFields = false,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    static GeneratorConfigContext? defaultContext;

    /// <summary>
    /// The default <see cref="JsonSerializerContext"/> associated with a default <see cref="JsonSerializerOptions"/> instance.
    /// </summary>
    public static GeneratorConfigContext Instance => defaultContext ??= new GeneratorConfigContext(new JsonSerializerOptions(DefaultOptions));
}
#endif