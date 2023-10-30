namespace BD.Common8.Settings.Abstractions;

partial interface ISettings
{
    /// <summary>
    /// 设置项类型缓存接口
    /// </summary>
    interface ITypeCache
    {
        /// <inheritdoc cref="ISettings.Name"/>
        string Name { get; }

        /// <inheritdoc cref="System.Text.Json.Serialization.JsonSerializerContext"/>
        JsonSerializerContext JsonSerializerContext { get; }

        /// <inheritdoc cref="System.Text.Json.Serialization.Metadata.JsonTypeInfo"/>
        JsonTypeInfo JsonTypeInfo { get; }

        /// <summary>
        /// 创建设置项实例
        /// </summary>
        /// <returns></returns>
        ISettings CreateInstance();

        /// <summary>
        /// 当前设置项是否在保存中，避免并发写
        /// </summary>
        bool IsSaveing { get; set; }

        /// <inheritdoc cref="ISettings.TrySave(Type, object, bool)"/>
        void TrySave(object optionsMonitor, bool notRead = false);

        /// <inheritdoc cref="ISettings.TrySave(Type, object, bool)"/>
        void TrySaveByIocGet(bool notRead = false);

        /// <summary>
        /// 将实例序列化写入 UTF8 Json 流
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="utf8Json"></param>
        void Save(object? settings, Stream utf8Json);
    }

    /// <summary>
    /// 所有已加载的设置项类型
    /// </summary>
    static readonly ConcurrentDictionary<Type, ITypeCache> settingsTypeCaches = [];

    /// <summary>
    /// 设置项文件夹名
    /// </summary>
    protected const string DirName = "Settings";

    /// <summary>
    /// 根据设置项名称获取文件保存的路径，默认为 /AppData/Settings/{settingsName}.json
    /// </summary>
    /// <param name="settingsName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string GetFilePath(string settingsName)
    {
        var settingsFilePath = Path.Combine(
            IOPath.AppDataDirectory,
            DirName,
            settingsName + FileEx.JSON);

        return settingsFilePath;
    }

    /// <summary>
    /// 获取默认的 Json 序列化选项
    /// </summary>
    /// <returns></returns>
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
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        o.Converters.Add(new JsonStringEnumConverter());
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        return o;
    }

    /// <summary>
    /// 设置项类型缓存模型
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    internal record class TypeCache<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings> : ITypeCache where TSettings : class, ISettings<TSettings>, new()
    {
        /// <inheritdoc/>
        public string Name => TSettings.Name;

        /// <inheritdoc/>
        public JsonSerializerContext JsonSerializerContext => TSettings.JsonSerializerContext;

        /// <inheritdoc/>
        public JsonTypeInfo JsonTypeInfo => TSettings.JsonTypeInfo;

        /// <inheritdoc/>
        ISettings ITypeCache.CreateInstance() => new TSettings();

        /// <inheritdoc/>
        public bool IsSaveing { get; set; }

        /// <inheritdoc/>
        public override string? ToString() => $"TypeCache<{Name}>";

        /// <inheritdoc/>
        void ITypeCache.TrySave(object optionsMonitor, bool notRead)
        {
            if (optionsMonitor is IOptionsMonitor<TSettings> optionsMonitor_)
            {
                TrySave(optionsMonitor_, notRead);
            }
        }

        /// <inheritdoc cref="ISettings.TrySave(Type, object, bool)"/>
        public void TrySave(IOptionsMonitor<TSettings> optionsMonitor, bool notRead)
        {
            var settings = optionsMonitor.CurrentValue;
            var settingsName = TSettings.Name;
            var settingsFilePath = GetFilePath(settingsName);

            lock (settingsName)
            {
                IsSaveing = true;
                try
                {
                    if (!notRead) // 通过读取配置文件与内存中的配置进行比较
                        try
                        {
                            if (File.Exists(settingsFilePath))
                            {
                                SystemTextJsonObject? jobj;
                                var options = GetDefaultOptions();
                                using var readStream = new FileStream(
                                    settingsFilePath,
                                    FileMode.Open,
                                    FileAccess.Read,
                                    FileShare.ReadWrite | FileShare.Delete);
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
                                jobj = SystemTextJsonSerializer.Deserialize<SystemTextJsonObject>(readStream, options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
                                if (jobj != null)
                                {
                                    var jnode = jobj[settingsName];
                                    if (jnode != null)
                                    {
                                        options = GetDefaultOptions();
                                        options.TypeInfoResolver = JsonTypeInfoResolver.Instance;
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
                                        var settingsByRead = SystemTextJsonSerializer.Deserialize<TSettings>(jnode, options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.

                                        // 使用 MemoryPack 序列化两者比较相等
                                        var right = MemoryPackSerializer.Serialize(settingsByRead);
                                        var left = MemoryPackSerializer.Serialize(settings);
                                        if (left.SequenceEqual(right))
                                            return;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

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

                    Policy.Handle<Exception>().Retry(3).Execute(() =>
                    {
                        using var writeStream = OpenOrCreate(settingsFilePath);
                        if (writeStream == null)
                        {
                            using var memoryStream = new MemoryStream();
                            Save(settings, memoryStream);
                            var bytes = memoryStream.ToByteArray();
                            File.WriteAllBytes(settingsFilePath, bytes);
                        }
                        else
                        {
                            Save(settings, writeStream);
                        }
                    });
                }
                finally
                {
                    IsSaveing = false;
                }
            }
        }

        /// <inheritdoc/>
        public void TrySaveByIocGet(bool notRead)
        {
            var options = Ioc.Get_Nullable<IOptionsMonitor<TSettings>>();
            if (options != null)
            {
                TrySave(options, notRead);
            }
        }

        /// <inheritdoc/>
        public void Save(object? settings, Stream utf8Json)
        {
            utf8Json.Position = 0;
            utf8Json.Write("{\""u8);
            utf8Json.Write(Encoding.UTF8.GetBytes(TSettings.Name));
            utf8Json.Write("\":"u8);
            var jsonSerializerContext = TSettings.JsonSerializerContext;
            SystemTextJsonSerializer.Serialize(utf8Json, settings, typeof(TSettings), jsonSerializerContext);
            utf8Json.Write("}"u8);
            utf8Json.SetLength(utf8Json.Position);
            utf8Json.Flush();
        }
    }
}