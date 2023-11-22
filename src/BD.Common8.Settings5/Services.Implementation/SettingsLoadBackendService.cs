using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using static BD.Common8.Settings5.Services.ISettingsLoadService;

namespace BD.Common8.Settings5.Services.Implementation;

/// <summary>
/// <see cref="ISettingsLoadService"/> 的后端(Backend)实现
/// </summary>
public sealed class SettingsLoadBackendService : ISettingsLoadService
{
    readonly SystemTextJsonSerializerContext settingsSerializerContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsLoadBackendService"/> class.
    /// </summary>
    /// <param name="settingsSerializerContext"></param>
    public SettingsLoadBackendService(SystemTextJsonSerializerContext settingsSerializerContext)
    {
        this.settingsSerializerContext = settingsSerializerContext;
        Current = this;
    }

    readonly HashSet<Type> SettingsModelTypes = [];

    readonly ConcurrentDictionary<Type, (string Name, string FilePath)> CacheSettingsFilePaths = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string GetSettingsFileNameWithoutExtensionByType([DynamicallyAccessedMembers(DAMT_M)] Type settingsModelType)
        => settingsModelType.Name.TrimEnd("Model");

    /// <summary>
    /// 获取设置项保存的文件路径
    /// </summary>
    /// <param name="settingsModelType"></param>
    /// <param name="settingsFileNameWithoutExtension"></param>
    /// <param name="settingsFileDirectory"></param>
    /// <returns></returns>
    string GetSettingsFilePath([DynamicallyAccessedMembers(DAMT_M)] Type settingsModelType, string settingsFileNameWithoutExtension, string? settingsFileDirectory = null)
    {
        if (CacheSettingsFilePaths.TryGetValue(settingsModelType, out var result))
            return result.FilePath;
        else
        {
            var settingsFilePath = Path.Combine(
                settingsFileDirectory ?? IOPath.AppDataDirectory,
                DirName,
                settingsFileNameWithoutExtension + FileEx.JSON);

            CacheSettingsFilePaths.TryAdd(settingsModelType, (settingsFileNameWithoutExtension, settingsFilePath));
            return settingsFilePath;
        }
    }

    /// <summary>
    /// 保存设置项数据到流中
    /// </summary>
    /// <param name="settingsModel"></param>
    /// <param name="settingsModelType"></param>
    /// <param name="settingsFileNameWithoutExtension"></param>
    /// <param name="utf8Json"></param>
    /// <param name="settingsSerializerContext"></param>
    static void Save(object settingsModel, [DynamicallyAccessedMembers(DAMT_M)] Type settingsModelType, string settingsFileNameWithoutExtension, Stream utf8Json, SystemTextJsonSerializerContext settingsSerializerContext)
    {
        utf8Json.Position = 0;
        utf8Json.Write("{\""u8);
        utf8Json.Write(Encoding.UTF8.GetBytes(settingsFileNameWithoutExtension));
        utf8Json.Write("\":"u8);
        SystemTextJsonSerializer.Serialize(utf8Json, settingsModel, settingsModelType, settingsSerializerContext);
        utf8Json.Write("}"u8);
        utf8Json.SetLength(utf8Json.Position);
        utf8Json.Flush();
    }

    [RequiresDynamicCode("JsonStringEnumConverter cannot be statically analyzed and requires runtime code generation. Applications should use the generic JsonStringEnumConverter<TEnum> instead.")]
    static SystemTextJsonSerializerOptions GetDefaultOptions()
    {
        var o = new SystemTextJsonSerializerOptions()
        {
            DefaultIgnoreCondition = SystemTextJsonIgnoreCondition.Never,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = true,
            IncludeFields = false,
            WriteIndented = true,
        };
        o.Converters.Add(new JsonStringEnumConverter());
        return o;
    }

    static TSettingsModel? Deserialize<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(string settingsFilePath, string settingsFileNameWithoutExtension, SystemTextJsonSerializerContext settingsSerializerContext)
    {
        SystemTextJsonObject? jobj;
        var options = GetDefaultOptions();
        using var readStream = new FileStream(
            settingsFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);
        jobj = SystemTextJsonSerializer.Deserialize<SystemTextJsonObject>(readStream, options);
        if (jobj != null)
        {
            var jnode = jobj[settingsFileNameWithoutExtension];
            if (jnode != null)
            {
                options = GetDefaultOptions();
                options.TypeInfoResolver = settingsSerializerContext;
                var settingsByRead = jnode.Deserialize<TSettingsModel>(options);
                return settingsByRead;
            }
        }
        return default;
    }

    /// <inheritdoc/>
    public bool Load<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(
        out Action<IServiceCollection>? configureServices,
        out IOptionsMonitor<TSettingsModel>? options,
        bool settingsFileDirectoryExists,
        string? settingsFileDirectory = null) where TSettingsModel : class, new()
    {
        options = default;
        configureServices = default;

        var settingsModelType = typeof(TSettingsModel);
        if (!SettingsModelTypes.Add(settingsModelType))
            return false; // 已经添加过，不需要重复添加

        TSettingsModel? settingsModel = null;

        var settingsFileNameWithoutExtension = GetSettingsFileNameWithoutExtensionByType(settingsModelType);
        var settingsFilePath = GetSettingsFilePath(settingsModelType, settingsFileNameWithoutExtension, settingsFileDirectory);

        bool writeFile = false;
        if (settingsFileDirectoryExists)
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

        var isInvalid = false;
        if (writeFile)
        {
            using var fs = File.Create(settingsFilePath);
            settingsModel = new();
            Save(settingsModel, settingsModelType, settingsFileNameWithoutExtension, fs, settingsSerializerContext);
        }
        else
            try
            {
                settingsModel = Deserialize<TSettingsModel>(settingsFilePath, settingsFileNameWithoutExtension, settingsSerializerContext) ?? new();
            }
            catch
            {
                settingsModel = new();
                isInvalid = true;

                // 尝试将错误的配置保存为 .json.i.bak 防止启动软件当前配置被覆盖
                var settingsFilePath_i_bak = $"{settingsFilePath}.i.bak";
                try
                {
                    File.Move(settingsFilePath, settingsFilePath_i_bak);
                }
                catch
                {
                }
            }

        var monitor = new OptionsMonitor<TSettingsModel>(settingsFilePath, settingsFileNameWithoutExtension, settingsModel, settingsSerializerContext);
        configureServices = s =>
        {
            s.AddSingleton(_ => monitor);
            s.AddSingleton<IOptions<TSettingsModel>>(_ => monitor);
            s.AddSingleton<IOptionsMonitor<TSettingsModel>>(_ => monitor);
        };
        options = monitor;
        return isInvalid;
    }

    /// <inheritdoc/>
    public void Save<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(TSettingsModel settingsModel, bool force = true) where TSettingsModel : class, new()
    {
        var monitor = Ioc.Get<OptionsMonitor<TSettingsModel>>();
        if (force)
        {
            monitor.SettingsModel = settingsModel;
            monitor.Save();
        }
        else
            monitor.UpdateSettingsModel(settingsModel, save: true);
    }

    sealed class OptionsMonitor<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel> : IOptionsMonitor<TSettingsModel>, IOptions<TSettingsModel> where TSettingsModel : class, new()
    {
        readonly string settingsFileName;
        readonly string settingsFilePath;
        readonly PhysicalFileProvider fileProvider;
        readonly SystemTextJsonSerializerContext settingsSerializerContext;
        readonly string settingsFileNameWithoutExtension;

        bool isSaveing;
        TSettingsModel? beforeSavingSettingsModel = null;

        internal TSettingsModel SettingsModel { get; set; }

        /// <summary>
        /// 更新设置模型，会检查值是否相等，相等则跳过赋值与保存
        /// </summary>
        /// <param name="settingsModel"></param>
        /// <param name="save">是否保存到文件，默认值：<see langword="true"/></param>
        internal void UpdateSettingsModel(TSettingsModel settingsModel, bool save = true)
        {
            var newSettingsData = Serializable.SMP2(settingsModel);
            var oldSettingsData = Serializable.SMP2(SettingsModel);
            if (newSettingsData.SequenceEqual(oldSettingsData))
                return;

            SettingsModel = settingsModel;

            if (save)
                Save();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static FileStream? OpenOrCreate(string path)
        {
            try
            {
                return new FileStream(
                                path,
                                FileMode.OpenOrCreate,
                                FileAccess.Write,
                                FileShare.ReadWrite | FileShare.Delete);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
                return default;
            }
        }

        const int saveRetryCount = 3;

        internal void Save()
        {
            #region 将当前设置项文件转存为备份

            try
            {
                var settingsFilePath2 = $"{settingsFilePath}.bak";
                IOPath.FileTryDelete(settingsFilePath2);
                File.Move(settingsFilePath, settingsFilePath2);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("bak settings file fail, ex: " + ex);
#endif
            }

            #endregion

            isSaveing = true;
            var savingSettingsModel = SettingsModel;
            beforeSavingSettingsModel = savingSettingsModel;
            try
            {
                Policy.Handle<Exception>().Retry(saveRetryCount).Execute(() => // 保存文件且重试几次
                {
                    var dirPath = Path.GetDirectoryName(settingsFilePath);
                    if (dirPath != null)
                        IOPath.DirCreateByNotExists(dirPath);
                    using var writeStream = OpenOrCreate(settingsFilePath);
                    if (writeStream == null)
                    {
                        using var memoryStream = new MemoryStream();
                        SettingsLoadBackendService.Save(savingSettingsModel, typeof(TSettingsModel), settingsFileNameWithoutExtension, memoryStream, settingsSerializerContext);
                        var bytes = memoryStream.ToByteArray();
                        File.WriteAllBytes(settingsFilePath, bytes);
                    }
                    else
                        SettingsLoadBackendService.Save(savingSettingsModel, typeof(TSettingsModel), settingsFileNameWithoutExtension, writeStream, settingsSerializerContext);
                });
            }
            finally
            {
                isSaveing = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Equals(TSettingsModel l, TSettingsModel r)
        {
            var l2 = Serializable.SMP2(l);
            var r2 = Serializable.SMP2(r);
            return l2.SequenceEqual(r2);
        }

        internal OptionsMonitor(string settingsFilePath, string settingsFileNameWithoutExtension, TSettingsModel settingsModel, SystemTextJsonSerializerContext settingsSerializerContext)
        {
            this.settingsFileNameWithoutExtension = settingsFileNameWithoutExtension;
            this.settingsSerializerContext = settingsSerializerContext;
            this.settingsFilePath = settingsFilePath;
            var settingsDirPath = Path.GetDirectoryName(settingsFilePath);
            settingsDirPath.ThrowIsNull();
            SettingsModel = settingsModel;
            fileProvider = new(settingsDirPath);
            settingsFileName = Path.GetFileName(settingsFilePath);
        }

        TSettingsModel IOptionsMonitor<TSettingsModel>.CurrentValue => SettingsModel;

        TSettingsModel IOptions<TSettingsModel>.Value => SettingsModel;

        TSettingsModel IOptionsMonitor<TSettingsModel>.Get(string? name) => SettingsModel;

        TSettingsModel? Deserialize()
        {
            try
            {
                var settings = Deserialize<TSettingsModel>(settingsFilePath, settingsFileNameWithoutExtension, settingsSerializerContext);
                return settings;
            }
            catch
            {
                return null;
            }
        }

        IDisposable? IOptionsMonitor<TSettingsModel>.OnChange(Action<TSettingsModel, string?> listener)
        {
            void OnChange()
            {
                // 需要防止保存时修改文件，触发文件变更，死循环
                if (isSaveing)
                    return;

                var settingsModel = Deserialize();
                if (settingsModel != null)
                {
                    if (beforeSavingSettingsModel != null)
                        if (Equals(beforeSavingSettingsModel, settingsModel))
                            return; // 如果监听到的变动与保存之前记录的值一样，则忽略

                    // 监听到的设置模型实例，如果和 new 一个空的数据一样的，就是默认值则忽略
                    if (Equals(settingsModel, new()))
                        return;

                    SettingsModel = settingsModel;
                    listener.Invoke(settingsModel, settingsFileNameWithoutExtension);
                }
            }
            return ChangeToken.OnChange(() => fileProvider.Watch(settingsFileName), OnChange);
        }
    }
}
