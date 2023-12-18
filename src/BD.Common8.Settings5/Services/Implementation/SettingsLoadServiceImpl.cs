using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace BD.Common8.Settings5.Services.Implementation;

/// <summary>
/// <see cref="ISettingsLoadService"/> 的实现
/// </summary>
public sealed class SettingsLoadServiceImpl : ISettingsLoadService
{
    /// <summary>
    /// 设置项文件存放文件夹名
    /// </summary>
    const string DirName = "Settings";

    readonly bool isReadOnly;
    readonly SystemTextJsonSerializerOptions options;
    readonly HashSet<Type> SettingsModelTypes = [];
    readonly ConcurrentDictionary<Type, (string Name, string FilePath)> CacheSettingsFilePaths = [];
    readonly ConcurrentDictionary<Type, object> optionsMonitors = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsLoadServiceImpl"/> class.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="isReadOnly"></param>
    public SettingsLoadServiceImpl(SystemTextJsonSerializerOptions options, bool isReadOnly = false)
    {
        this.isReadOnly = isReadOnly;
        this.options = options;
        Current = this;
    }

    static SettingsLoadServiceImpl? _Current;

    /// <summary>
    /// 获取当前单例服务
    /// </summary>
    public static SettingsLoadServiceImpl Current { get => _Current.ThrowIsNull(); private set => _Current = value; }

    /// <summary>
    /// 根据设置模型类型获取文件名不包含扩展名
    /// </summary>
    /// <param name="settingsModelType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string GetSettingsFileNameWithoutExtensionByType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type settingsModelType)
        => settingsModelType.Name.TrimEnd("Model");

    /// <summary>
    /// 根据文件名不包含扩展名获取模型类型
    /// </summary>
    /// <param name="settingsFileNameWithoutExtension"></param>
    /// <returns></returns>
    public Type? GetSettingsModelType(string settingsFileNameWithoutExtension)
    {
        foreach (var type in SettingsModelTypes)
        {
#pragma warning disable IL2072 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            var value = GetSettingsFileNameWithoutExtensionByType(type);
#pragma warning restore IL2072 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            if (value == settingsFileNameWithoutExtension)
                return type;
        }
        return null;
    }

    /// <summary>
    /// 获取设置项保存的文件路径
    /// </summary>
    /// <param name="settingsModelType"></param>
    /// <param name="settingsFileNameWithoutExtension"></param>
    /// <param name="settingsFileDirectory"></param>
    /// <returns></returns>
    string GetSettingsFilePath([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type settingsModelType, string settingsFileNameWithoutExtension, string? settingsFileDirectory = null)
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
    /// <param name="options"></param>
    static void Save(object settingsModel, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type settingsModelType, string settingsFileNameWithoutExtension, Stream utf8Json, SystemTextJsonSerializerOptions options)
    {
        utf8Json.Position = 0;
        utf8Json.Write("{\""u8);
        utf8Json.Write(Encoding.UTF8.GetBytes(settingsFileNameWithoutExtension));
        utf8Json.Write("\":"u8);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        SystemTextJsonSerializer.Serialize(utf8Json, settingsModel, settingsModelType, options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        utf8Json.Write("}"u8);
        utf8Json.SetLength(utf8Json.Position);
        utf8Json.Flush();
    }

    static TSettingsModel? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(string settingsFilePath, string settingsFileNameWithoutExtension, SystemTextJsonSerializerOptions options)
    {
        SystemTextJsonObject? jobj;
        using var readStream = new FileStream(
            settingsFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        jobj = SystemTextJsonSerializer.Deserialize<SystemTextJsonObject>(readStream, options);
        if (jobj != null)
        {
            var jnode = jobj[settingsFileNameWithoutExtension];
            if (jnode != null)
            {
                var settingsByRead = jnode.Deserialize<TSettingsModel>(options);
                return settingsByRead;
            }
        }
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return default;
    }

    /// <inheritdoc/>
    public (bool isInvalid, Exception? ex, string settingsFileNameWithoutExtension) Load<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(
        out Action<IServiceCollection>? configureServices,
        out IOptionsMonitor<TSettingsModel>? options,
        string? settingsFileDirectory = null) where TSettingsModel : class, new()
    {
        options = default;
        configureServices = default;

        var settingsModelType = typeof(TSettingsModel);
        var settingsFileNameWithoutExtension = GetSettingsFileNameWithoutExtensionByType(settingsModelType);
        if (!SettingsModelTypes.Add(settingsModelType))
            return (false, null, settingsFileNameWithoutExtension); // 已经添加过，不需要重复添加

        TSettingsModel? settingsModel = null;

        var settingsFilePath = GetSettingsFilePath(settingsModelType, settingsFileNameWithoutExtension, settingsFileDirectory);

        bool writeFile = false;
        if (!isReadOnly)
        {
            if (!File.Exists(settingsFilePath))
            {
                writeFile = true;
            }
        }

        var isInvalid = false;
        Exception? exception = null;
        if (!isReadOnly && writeFile)
        {
            bool fileCreateMark = false;
            try
            {
            fileCreateStart: try
                {
                    using var fs = File.Create(settingsFilePath);
                    settingsModel = new();
                    Save(settingsModel, settingsModelType, settingsFileNameWithoutExtension, fs, this.options);
                }
                catch (DirectoryNotFoundException)
                {
                    if (!fileCreateMark)
                    {
                        var settingsFileDir = Path.GetDirectoryName(settingsFilePath);
                        if (settingsFileDir != null)
                            IOPath.DirCreateByNotExists(settingsFileDir);
                        fileCreateMark = true;
                        goto fileCreateStart;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                settingsModel = new();
                isInvalid = true;
            }
        }
        else
        {
            try
            {
                settingsModel = Deserialize<TSettingsModel>(settingsFilePath, settingsFileNameWithoutExtension, this.options) ?? new();
            }
            catch (Exception ex)
            {
                exception = ex;
                settingsModel = new();
                isInvalid = true;

                if (!isReadOnly)
                {
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
            }
        }

        var monitor = new OptionsMonitor<TSettingsModel>(settingsFilePath, settingsFileNameWithoutExtension, settingsModel, this);
        optionsMonitors.TryAdd(typeof(IOptions<TSettingsModel>), monitor);
        optionsMonitors.TryAdd(typeof(IOptionsMonitor<TSettingsModel>), monitor);
        configureServices = s =>
        {
            s.AddSingleton(_ => monitor);
            s.AddSingleton<IOptions<TSettingsModel>>(_ => monitor);
            s.AddSingleton<IOptionsMonitor<TSettingsModel>>(_ => monitor);
        };
        options = monitor;
        return (isInvalid, exception, settingsFileNameWithoutExtension);
    }

    /// <inheritdoc/>
    public T Get<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() where T : notnull
    {
        if (optionsMonitors.TryGetValue(typeof(T), out var value))
        {
            return (T)value;
        }
        return Ioc.Get<T>();
    }

    /// <inheritdoc/>
    public void Save<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(TSettingsModel settingsModel, bool force = true) where TSettingsModel : class, new()
    {
        var monitor = ISettingsLoadService.Current.Get<OptionsMonitor<TSettingsModel>>();
        if (force)
        {
            monitor.SettingsModel = settingsModel;
            monitor.Save();
        }
        else
            monitor.UpdateSettingsModel(settingsModel, save: true);
    }

    sealed class OptionsMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel> : IOptionsMonitor<TSettingsModel>, IOptions<TSettingsModel> where TSettingsModel : class, new()
    {
        readonly string settingsFileName;
        readonly string settingsFilePath;
        readonly PhysicalFileProvider fileProvider;
        readonly string settingsFileNameWithoutExtension;
        readonly SettingsLoadServiceImpl settingsLoadService;

        bool isSaveing;
        TSettingsModel? beforeSavingSettingsModel = null;

        internal OptionsMonitor(string settingsFilePath, string settingsFileNameWithoutExtension, TSettingsModel settingsModel, SettingsLoadServiceImpl settingsLoadService)
        {
            this.settingsFileNameWithoutExtension = settingsFileNameWithoutExtension;
            this.settingsLoadService = settingsLoadService;
            this.settingsFilePath = settingsFilePath;
            var settingsDirPath = Path.GetDirectoryName(settingsFilePath);
            settingsDirPath.ThrowIsNull();
            SettingsModel = settingsModel;
            fileProvider = settingsLoadService.isReadOnly ? null! : new(settingsDirPath);
            settingsFileName = Path.GetFileName(settingsFilePath);
        }

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
            if (settingsLoadService.isReadOnly)
            {
                return;
            }

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
                        SettingsLoadServiceImpl.Save(savingSettingsModel, typeof(TSettingsModel), settingsFileNameWithoutExtension, memoryStream, settingsLoadService.options);
                        var bytes = memoryStream.ToByteArray();
                        File.WriteAllBytes(settingsFilePath, bytes);
                    }
                    else
                        SettingsLoadServiceImpl.Save(savingSettingsModel, typeof(TSettingsModel), settingsFileNameWithoutExtension, writeStream, settingsLoadService.options);
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

        TSettingsModel IOptionsMonitor<TSettingsModel>.CurrentValue => SettingsModel;

        TSettingsModel IOptions<TSettingsModel>.Value => SettingsModel;

        TSettingsModel IOptionsMonitor<TSettingsModel>.Get(string? name) => SettingsModel;

        TSettingsModel? Deserialize()
        {
            try
            {
                var settings = Deserialize<TSettingsModel>(settingsFilePath, settingsFileNameWithoutExtension, settingsLoadService.options);
                return settings;
            }
            catch
            {
                return null;
            }
        }

        IDisposable? IOptionsMonitor<TSettingsModel>.OnChange(Action<TSettingsModel, string?> listener)
        {
            if (settingsLoadService.isReadOnly)
            {
                return null;
            }

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
