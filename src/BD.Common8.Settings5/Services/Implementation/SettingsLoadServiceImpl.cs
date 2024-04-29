using Microsoft.Extensions.FileProviders;
#if !(ANDROID || IOS)
using Microsoft.Extensions.Primitives;
#endif

namespace BD.Common8.Settings5.Services.Implementation;

/// <summary>
/// <see cref="ISettingsLoadService"/> 的实现
/// </summary>
public class SettingsLoadServiceImpl : ISettingsLoadService
{
    /// <summary>
    /// 设置项文件存放文件夹名
    /// </summary>
    const string DirName = "Settings";

    /// <summary>
    /// 是否写入文件
    /// </summary>
    protected readonly bool isWriteFile;

    /// <summary>
    /// Json 序列化选项
    /// </summary>
    protected readonly SystemTextJsonSerializerOptions options;

    /// <summary>
    /// 设置模型类型哈希集合
    /// </summary>
    protected readonly HashSet<Type> SettingsModelTypes = [];

    /// <summary>
    /// 缓存设置文件路径字典
    /// </summary>
    protected readonly ConcurrentDictionary<Type, (string Name, string FilePath)> CacheSettingsFilePaths = [];

    /// <summary>
    /// 设置项监视器字典
    /// </summary>
    protected readonly ConcurrentDictionary<Type, object> optionsMonitors = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsLoadServiceImpl"/> class.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="isWriteFile"></param>
    public SettingsLoadServiceImpl(SystemTextJsonSerializerOptions options, bool isWriteFile = true)
    {
        this.isWriteFile = isWriteFile;
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
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
    protected static string GetSettingsFileNameWithoutExtensionByType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type settingsModelType)
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
    protected string GetSettingsFilePath([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type settingsModelType, string settingsFileNameWithoutExtension, string? settingsFileDirectory = null)
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
    protected static void Save(object settingsModel, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type settingsModelType, string settingsFileNameWithoutExtension, Stream utf8Json, SystemTextJsonSerializerOptions options)
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

    /// <summary>
    /// 根据设置文件保存路径反序列化实例
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    /// <param name="settingsFilePath"></param>
    /// <param name="settingsFileNameWithoutExtension"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected static TSettingsModel? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(string settingsFilePath, string settingsFileNameWithoutExtension, SystemTextJsonSerializerOptions options)
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
        if (isWriteFile)
        {
            if (!File.Exists(settingsFilePath))
            {
                writeFile = true;
            }
        }

        var isInvalid = false;
        Exception? exception = null;
        if (isWriteFile && writeFile)
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

                if (isWriteFile)
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

        var monitor = CreateOptionsMonitor(settingsFilePath, settingsFileNameWithoutExtension, settingsModel, this);
        optionsMonitors.TryAdd(typeof(IOptions<TSettingsModel>), monitor);
        optionsMonitors.TryAdd(typeof(IOptionsMonitor<TSettingsModel>), monitor);
        optionsMonitors.TryAdd(typeof(OptionsMonitor<TSettingsModel>), monitor);
        configureServices = s =>
        {
            s.AddSingleton(_ => monitor);
            s.AddSingleton<IOptions<TSettingsModel>>(_ => monitor);
            s.AddSingleton<IOptionsMonitor<TSettingsModel>>(_ => monitor);
        };
        options = monitor;
        return (isInvalid, exception, settingsFileNameWithoutExtension);
    }

    /// <summary>
    /// 创建 <see cref="IOptionsMonitor{TOptions}"/>
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    /// <param name="settingsFilePath"></param>
    /// <param name="settingsFileNameWithoutExtension"></param>
    /// <param name="settingsModel"></param>
    /// <param name="settingsLoadService"></param>
    /// <returns></returns>
    protected virtual OptionsMonitor<TSettingsModel> CreateOptionsMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(
        string settingsFilePath,
        string settingsFileNameWithoutExtension,
        TSettingsModel settingsModel,
        SettingsLoadServiceImpl settingsLoadService)
        where TSettingsModel : class, new()
    {
        OptionsMonitor<TSettingsModel> monitor = new(settingsFilePath, settingsFileNameWithoutExtension, settingsModel, settingsLoadService);
        return monitor;
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

    object? Get([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type typeOptionsMonitor)
    {
        if (optionsMonitors.TryGetValue(typeOptionsMonitor, out var value))
        {
            return value;
        }
        return Ioc.Get(typeOptionsMonitor);
    }

    /// <summary>
    /// 根据类型名与模型值字节数组保存设置项
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <param name="bytes"></param>
    public void Save(string typeFullName, byte[] bytes)
    {
        var settingsModelType = SettingsModelTypes.FirstOrDefault(x => x.FullName == typeFullName);
        if (settingsModelType != null)
        {
#pragma warning disable IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            var typeOptionsMonitor = typeof(IOptionsMonitor<>).MakeGenericType(settingsModelType);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            var monitor = Get(typeOptionsMonitor);
            if (monitor is IInternalOptionsMonitor monitor2)
            {
                monitor2.Save(bytes);
            }
        }
    }

    /// <summary>
    /// 变更设置项
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <param name="bytes"></param>
    public void Change(string typeFullName, byte[] bytes)
    {
        var settingsModelType = SettingsModelTypes.FirstOrDefault(x => x.FullName == typeFullName);
        if (settingsModelType != null)
        {
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            var typeOptionsMonitor = typeof(IOptionsMonitor<>).MakeGenericType(settingsModelType);
#pragma warning restore IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            var monitor = Get(typeOptionsMonitor);
            if (monitor is IInternalOptionsMonitor monitor2)
            {
                monitor2.Change(bytes);
            }
        }
    }

    /// <summary>
    /// 强制保存设置项
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    /// <param name="monitor"></param>
    public void ForceSave<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(
        IOptionsMonitor<TSettingsModel> monitor) where TSettingsModel : class, new()
    {
        if (monitor is OptionsMonitor<TSettingsModel> monitor2)
        {
            monitor2.Save();
        }
    }

    /// <inheritdoc/>
    public void Save<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(
        TSettingsModel settingsModel,
        bool force = true) where TSettingsModel : class, new()
    {
        var monitor = Get<OptionsMonitor<TSettingsModel>>();
        if (force)
        {
            monitor.SettingsModel = settingsModel;
            monitor.Save();
        }
        else
            monitor.UpdateSettingsModel(settingsModel, save: true);
    }

    /// <inheritdoc/>
    public void ForceSave<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>() where TSettingsModel : class, new()
    {
        var monitor = Get<OptionsMonitor<TSettingsModel>>();
        monitor.Save();
    }

    interface IInternalOptionsMonitor
    {
        void Save(byte[] bytes);

        void Change(byte[] bytes);
    }

    /// <summary>
    /// <see cref="IOptionsMonitor{TOptions}"/> 的实现类
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    protected class OptionsMonitor<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel> :
        IOptionsMonitor<TSettingsModel>,
        IOptions<TSettingsModel>,
        IInternalOptionsMonitor
        where TSettingsModel : class, new()
    {
        /// <summary>
        /// 设置项保存文件名
        /// </summary>
        protected readonly string settingsFileName;

        /// <summary>
        /// 设置项保存文件路径
        /// </summary>
        protected readonly string settingsFilePath;
#if !(ANDROID || IOS)
        /// <summary>
        /// 设置项保存文件提供者
        /// </summary>
        protected readonly PhysicalFileProvider fileProvider;
#endif

        /// <summary>
        /// 设置项保存文件名不包含扩展名
        /// </summary>
        protected readonly string settingsFileNameWithoutExtension;

        /// <summary>
        /// 设置项加载服务
        /// </summary>
        protected readonly SettingsLoadServiceImpl settingsLoadService;

        /// <summary>
        /// 是否正在保存中
        /// </summary>
        protected bool isSaveing;

        /// <summary>
        /// 上一个保存中的设置模型项实例
        /// </summary>
        protected TSettingsModel? beforeSavingSettingsModel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsMonitor{TSettingsModel}"/> class.
        /// </summary>
        /// <param name="settingsFilePath"></param>
        /// <param name="settingsFileNameWithoutExtension"></param>
        /// <param name="settingsModel"></param>
        /// <param name="settingsLoadService"></param>
        public OptionsMonitor(string settingsFilePath,
            string settingsFileNameWithoutExtension,
            TSettingsModel settingsModel,
            SettingsLoadServiceImpl settingsLoadService)
        {
            this.settingsFileNameWithoutExtension = settingsFileNameWithoutExtension;
            this.settingsLoadService = settingsLoadService;
            this.settingsFilePath = settingsFilePath;
            var settingsDirPath = Path.GetDirectoryName(settingsFilePath);
            settingsDirPath.ThrowIsNull();
            SettingsModel = settingsModel;
#if !(ANDROID || IOS)
            fileProvider = !settingsLoadService.isWriteFile ? null! : new(settingsDirPath);
#endif
            settingsFileName = Path.GetFileName(settingsFilePath);
        }

        /// <summary>
        /// 当前设置项模型实例
        /// </summary>
        public TSettingsModel SettingsModel { get; set; }

        /// <summary>
        /// 更新设置项模型
        /// </summary>
        /// <param name="value"></param>
        protected virtual void UpdateSettingsModel(TSettingsModel value)
        {
        }

        /// <summary>
        /// 保存设置项
        /// </summary>
        /// <param name="bytes"></param>
        public void Save(byte[] bytes)
        {
            TSettingsModel? settingsModel = null;
            try
            {
                settingsModel = Serializable.DMP2<TSettingsModel>(bytes);
            }
            catch
            {
            }
            if (settingsModel != null)
            {
                SettingsModel = settingsModel;
                Save();
            }
        }

        /// <summary>
        /// 变更设置项
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void Change(byte[] bytes) { }

        /// <summary>
        /// 更新设置模型，会检查值是否相等，相等则跳过赋值与保存
        /// </summary>
        /// <param name="settingsModel"></param>
        /// <param name="save">是否保存到文件，默认值：<see langword="true"/></param>
        public void UpdateSettingsModel(TSettingsModel settingsModel, bool save = true)
        {
            var newSettingsData = Serializable.SMP2(settingsModel);
            var oldSettingsData = Serializable.SMP2(SettingsModel);
            if (newSettingsData.SequenceEqual(oldSettingsData))
                return;

            SettingsModel = settingsModel;

            if (save)
                Save();
        }

        /// <summary>
        /// 打开或创建文件流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static FileStream? OpenOrCreate(string path)
        {
            try
            {
                return new FileStream(
                                path,
                                FileMode.OpenOrCreate,
                                FileAccess.Write,
                                FileShare.ReadWrite | FileShare.Delete);
            }
#pragma warning disable CS0168 // 声明了变量，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量，但从未使用过
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
                return default;
            }
        }

        /// <summary>
        /// 保存重试次数
        /// </summary>
        protected const int saveRetryCount = 3;

        /// <summary>
        /// 保存设置项
        /// </summary>
        public virtual void Save()
        {
            if (!settingsLoadService.isWriteFile)
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
#pragma warning disable CS0168 // 声明了变量，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量，但从未使用过
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
                ChangeListener?.Invoke(savingSettingsModel, settingsFileNameWithoutExtension);
            }
        }

        /// <summary>
        /// 比较两个设置项是否相等
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool Equals(TSettingsModel l, TSettingsModel r)
        {
            var l2 = Serializable.SMP2(l);
            var r2 = Serializable.SMP2(r);
            return l2.SequenceEqual(r2);
        }

        /// <inheritdoc/>
        public virtual TSettingsModel CurrentValue => SettingsModel;

        /// <inheritdoc/>
        public virtual TSettingsModel Value => SettingsModel;

        /// <inheritdoc/>
        public virtual TSettingsModel Get(string? name) => SettingsModel;

        /// <summary>
        /// 根据当前设置文件保存路径反序列化实例
        /// </summary>
        /// <returns></returns>
        protected virtual TSettingsModel? Deserialize()
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

        event Action<TSettingsModel, string?>? ChangeListener;

        /// <summary>
        /// 变更设置项时监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public virtual IDisposable? OnChange(Action<TSettingsModel, string?> listener)
        {
#if ANDROID || IOS
            return null;
#else
            if (fileProvider == null)
            {
                return null;
            }

            ChangeListener += listener;
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
                    UpdateSettingsModel(settingsModel);
                    listener.Invoke(settingsModel, settingsFileNameWithoutExtension);
                }
            }
            var changeToken = ChangeToken.OnChange(() => fileProvider.Watch(settingsFileName), OnChange);
            CompositeDisposable disposables = new();
            disposables.Add(changeToken);
            disposables.Add(Disposable.Create(() =>
            {
                ChangeListener -= listener;
            }));
            return disposables;
#endif
        }
    }
}
