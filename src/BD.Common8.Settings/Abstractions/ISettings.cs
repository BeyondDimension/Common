namespace BD.Common8.Settings.Abstractions;

/// <summary>
/// 设置项接口
/// </summary>
public partial interface ISettings
{
    /// <summary>
    /// 尝试将 <see cref="IOptionsMonitor{TOptions}"/> 保存到文件
    /// </summary>
    /// <param name="settingsType"></param>
    /// <param name="optionsMonitor"></param>
    /// <param name="notRead"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void TrySave(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type settingsType,
        object optionsMonitor,
        bool notRead = false)
    {
        if (settingsTypeCaches.TryGetValue(settingsType, out var settingsTypeCache))
        {
            settingsTypeCache.TrySave(optionsMonitor, notRead);
        }
    }

    /// <summary>
    /// 设置项的名称
    /// </summary>
    static abstract string Name { get; }

    /// <inheritdoc cref="System.Text.Json.Serialization.JsonSerializerContext"/>
    static abstract JsonSerializerContext JsonSerializerContext { get; }

    /// <inheritdoc cref="System.Text.Json.Serialization.Metadata.JsonTypeInfo"/>
    static abstract JsonTypeInfo JsonTypeInfo { get; }

    /// <summary>
    /// 设置项文件夹是否存在，且不存在时创建文件夹
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool DirectoryExists()
    {
        var settingsDirPath = Path.Combine(IOPath.AppDataDirectory, DirName);
        if (!Directory.Exists(settingsDirPath))
        {
            Directory.CreateDirectory(settingsDirPath);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 可自定义异常消息处理
    /// </summary>
    static Action<Exception, string>? ExceptionHandler { private get; set; }

    /// <summary>
    /// 保存设置项到文件
    /// </summary>
    /// <param name="settingsType"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SaveSettings(Type settingsType)
    {
        try
        {
            if (settingsTypeCaches.TryGetValue(settingsType, out var settingsTypeCache))
            {
                settingsTypeCache.TrySaveByIocGet();
            }
        }
        catch (Exception ex)
        {
            ExceptionHandler?.Invoke(ex, nameof(SaveSettings));
        }
    }

    /// <summary>
    /// 保存所有的设置项到文件
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async Task SaveAllSettingsAsync()
    {
        try
        {
            await Parallel.ForEachAsync(settingsTypeCaches, async (item, _) => await Task.Run(() =>
            {
                SaveSettings(item.Key);
            }, _).ConfigureAwait(false)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ExceptionHandler?.Invoke(ex, nameof(SaveAllSettingsAsync));
        }
    }

    /// <summary>
    /// 使用默认配置 Build Configuration
    /// </summary>
    /// <returns></returns>
    static IConfigurationRoot DefaultBuild()
    {
        ConfigurationBuilder builder = new();

        foreach (var settingsTypeCache in settingsTypeCaches)
        {
            var settings = settingsTypeCache.Value.CreateInstance();
            var memoryStream = new MemoryStream();
            settingsTypeCache.Value.Save(settings, memoryStream);
            memoryStream.Position = 0;
            builder.AddJsonStream(memoryStream);
        }

        return builder.Build();
    }

    /// <inheritdoc cref="IJsonTypeInfoResolver"/>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    sealed class JsonTypeInfoResolver : IJsonTypeInfoResolver
    {
        static readonly Lazy<JsonTypeInfoResolver> instance = new(() => new());

        public static IJsonTypeInfoResolver Instance => instance.Value;

        JsonTypeInfoResolver() { }

        JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type settingsType, SystemTextJsonSerializerOptions options)
        {
            if (settingsTypeCaches.TryGetValue(settingsType, out var settingsTypeCache))
            {
                return settingsTypeCache.JsonTypeInfo;
            }
            return null;
        }
    }
}

/// <summary>
/// 设置项泛型接口
/// </summary>
/// <typeparam name="TSettings"></typeparam>
public interface ISettings<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings> : ISettings where TSettings : class, ISettings<TSettings>, new()
{
    /// <inheritdoc cref="ISettings.Name"/>
    static new string Name => TSettings.Name;

    /// <inheritdoc cref="JsonTypeInfo{T}"/>
    static new abstract JsonTypeInfo<TSettings> JsonTypeInfo { get; }

    /// <summary>
    /// 从 UTF8 Json 流中反序列化实例
    /// </summary>
    /// <param name="utf8Json"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TSettings Deserialize(Stream utf8Json)
        => SystemTextJsonSerializer.Deserialize(utf8Json, TSettings.JsonTypeInfo) ?? new();

    /// <summary>
    /// 加载当前设置项类型到 Ioc 中
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="directoryExists"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Action<IConfiguration, IServiceCollection>? Load(
        ConfigurationBuilder builder,
        bool directoryExists)
    {
        var settingsType = typeof(TSettings);
        if (settingsTypeCaches.ContainsKey(settingsType))
            return null; // 避免重复加载

        TypeCache<TSettings> settingsTypeCache = new();
        settingsTypeCaches.TryAdd(settingsType, settingsTypeCache);

        var settingsName = TSettings.Name;
        var settingsFilePath = GetFilePath(settingsName);

        var writeFile = false;
        if (directoryExists)
        {
            if (!File.Exists(settingsFilePath))
            {
                writeFile = true;
            }
        }
        else
        {
            writeFile = true;
        }

        if (writeFile)
        {
            using var fs = File.Create(settingsFilePath);
            var settings = new TSettings();
            settingsTypeCache.Save(settings, fs);
        }

        builder.AddJsonFile(settingsFilePath, optional: true, reloadOnChange: true);

#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        return (configuration, services) =>
            services.Configure<TSettings>(
                configuration.GetRequiredSection(settingsName));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    }
}