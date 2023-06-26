using BD.Common.Settings.Models;

const string fileName = "settings_v4_app.json";
try
{
    if (!File.Exists(fileName))
    {
        Console.WriteLine($"file notfound, name: {fileName}");
        return;
    }
    AppSettings? appSettings = null;
    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
    {
        appSettings = JsonSerializer.Deserialize(fs, AppSettingsContext.Instance.AppSettings);
    }
    ArgumentNullException.ThrowIfNull(appSettings);
    AppSettings.Instance = appSettings;

    static FileStream GetFileStream(string path)
    {
        var dirPath = Path.GetDirectoryName(path);
        if (dirPath != null && !Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
    }

    foreach (var item in appSettings.Settings)
    {
        if (!string.IsNullOrWhiteSpace(item.Value.ClassFilePath))
        {
            var classFilePath = Path.Combine(AppContext.BaseDirectory, item.Value.ClassFilePath, $"{item.Key}.cs");
            using var stream = GetFileStream(classFilePath);
            SourceGenerator_Class(item.Key, stream, appSettings.Namespace, item.Value, appSettings.Usings);
            stream.Flush();
            stream.SetLength(stream.Position);
            Console.WriteLine($"已写入文件：{classFilePath}");
        }
        if (!string.IsNullOrWhiteSpace(item.Value.InterfaceFilePath))
        {
            var interfaceFilePath = Path.Combine(AppContext.BaseDirectory, item.Value.InterfaceFilePath, $"I{item.Key}.cs");
            using var stream = GetFileStream(interfaceFilePath);
            SourceGenerator_Interface(item.Key, stream, appSettings.Namespace, item.Value, appSettings.Usings);
            stream.Flush();
            stream.SetLength(stream.Position);
            Console.WriteLine($"已写入文件：{interfaceFilePath}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
finally
{
    Console.ReadLine();
}

//SourceGenerator_Interface(new MemoryStream(), appSettings.Namespace, appSettings.Settings.First());

static void SourceGenerator_Class(string name, Stream s, string @namespace, SettingsMetadata metadata, string usings = "")
{
    var @namespace_ = Encoding.UTF8.GetBytes(@namespace);
    var classNameTrim = Encoding.UTF8.GetBytes(name);
    s.Write("""
        #nullable enable
        #pragma warning disable IDE0079 // 请删除不必要的忽略
        #pragma warning disable SA1634 // File header should show copyright
        #pragma warning disable CS8601 // 引用类型赋值可能为 null。
        #pragma warning disable CS0108 // 成员隐藏继承的成员；缺少关键字 new
        //------------------------------------------------------------------------------
        // <auto-generated>
        //     此代码由包 BD.Common.Settings.V4.SourceGenerator.Tools 源生成。
        //
        //     对此文件的更改可能会导致不正确的行为，并且如果
        //     重新生成代码，这些更改将会丢失。
        // </auto-generated>
        //------------------------------------------------------------------------------


        """u8);
    if (!string.IsNullOrWhiteSpace(usings))
        s.Write(usings);
    if (!string.IsNullOrWhiteSpace(metadata.Usings))
        s.Write(metadata.Usings);
    s.Write("using static "u8);
    s.Write(@namespace_);
    s.Write(".Abstractions.I"u8);
    s.Write(classNameTrim);
    s.Write(";"u8);
    s.Write("""


        // ReSharper disable once CheckNamespace
        namespace 
        """u8);
    s.Write(@namespace_);
    s.Write(";\r\n\r\n[JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyProperties = true)]\r\n[JsonSerializable(typeof("u8);
    s.Write(classNameTrim);
    s.Write("_))]\r\ninternal partial class "u8);
    s.Write(classNameTrim);
    s.Write("Context : JsonSerializerContext\r\n{\r\n    static "u8);
    s.Write(classNameTrim);
    s.Write("Context? instance;\r\n\r\n    public static "u8);
    s.Write(classNameTrim);
    s.Write("Context Instance\r\n        => instance ??= new "u8);
    s.Write(classNameTrim);
    s.Write("Context(ISettings.GetDefaultOptions());\r\n}"u8);

    s.Write("\r\n\r\n[MPObj, MP2Obj(SerializeLayout.Explicit)]\r\n"u8);
    s.Write("public sealed partial class "u8);
    s.Write(classNameTrim);
    s.Write("_ : I"u8);
    s.Write(classNameTrim);
    s.Write(", ISettings, ISettings<"u8);
    s.Write(classNameTrim);
    s.Write("_>\r\n{\r\n    public const string Name = nameof("u8);
    s.Write(classNameTrim);
    s.Write(");\r\n\r\n    static string ISettings.Name => Name;\r\n\r\n    static JsonSerializerContext ISettings.JsonSerializerContext\r\n"u8);
    s.Write("        => "u8);
    s.Write(classNameTrim);
    s.Write("Context.Instance;\r\n\r\n    static JsonTypeInfo ISettings.JsonTypeInfo\r\n        => "u8);
    s.Write(classNameTrim);
    s.Write("Context.Instance."u8);
    s.Write(classNameTrim);
    s.Write("_;\r\n\r\n    static JsonTypeInfo<"u8);
    s.Write(classNameTrim);
    s.Write("_> ISettings<"u8);
    s.Write(classNameTrim);
    s.Write("_>.JsonTypeInfo\r\n        => "u8);
    s.Write(classNameTrim);
    s.Write("Context.Instance."u8);
    s.Write(classNameTrim);
    s.Write("_;\r\n\r\n"u8);

    int order = 0;

    foreach (var item in metadata.Properties)
    {
        if (item.IsRegionOrEndregion.HasValue)
        {
            if (item.IsRegionOrEndregion.Value)
            {
                s.Write("    #region"u8);
            }
            else
            {
                s.Write("    #endregion"u8);
            }
            if (!string.IsNullOrWhiteSpace(item.Summary))
            {
                s.Write(" "u8);
                s.Write(item.Summary);
            }
            s.Write("\r\n\r\n"u8);
        }
        else if (!string.IsNullOrWhiteSpace(item.Sharp))
        {
            //s.Write("#"u8);
            //s.Write(item.Sharp);
            //s.Write("\r\n\r\n"u8);
        }
        else
        {
            var order_ = Encoding.UTF8.GetBytes(order.ToString());
            s.Write("    /// <summary>\r\n    /// "u8);
            s.Write(item.Summary);
            s.Write("\r\n    /// </summary>\r\n    [MPKey("u8);
            s.Write(order_);
            s.Write("), MP2Key("u8);
            s.Write(order_);
            s.Write("), JsonPropertyOrder("u8);
            s.Write(order_);
            s.Write(")]\r\n"u8);
            order++;
            s.Write("    public "u8);
            s.Write(item.TypeName);
            s.Write("? "u8);
            s.Write(item.PropertyName);
            s.Write(" { get; set; }\r\n\r\n"u8);
        }
    }

    s.Write("}\r\n\r\npublic static partial class "u8);
    s.Write(classNameTrim);
    s.Write("\r\n{\r\n"u8);

    foreach (var item in metadata.Properties)
    {
        if (item.IsRegionOrEndregion.HasValue)
        {
            if (item.IsRegionOrEndregion.Value)
            {
                s.Write("    #region"u8);
            }
            else
            {
                s.Write("    #endregion"u8);
            }
            if (!string.IsNullOrWhiteSpace(item.Summary))
            {
                s.Write(" "u8);
                s.Write(item.Summary);
            }
            s.Write("\r\n\r\n"u8);
        }
        else if (!string.IsNullOrWhiteSpace(item.Sharp))
        {
            s.Write("#"u8);
            s.Write(item.Sharp);
            s.Write("\r\n\r\n"u8);
        }
        else
        {
            s.Write("    /// <summary>\r\n    /// "u8);
            s.Write(item.Summary);
            s.Write("\r\n    /// </summary>\r\n"u8);
            s.Write("    public static "u8);

            if (item.IsDictionary(out var key, out var value))
            {
                var key_ = Encoding.UTF8.GetBytes(key);
                var value_ = Encoding.UTF8.GetBytes(value);
                var propertyName = Encoding.UTF8.GetBytes(item.PropertyName);
                s.Write("SettingsProperty<"u8);
                s.Write(key_);
                s.Write(", "u8);
                s.Write(value_);
                s.Write(", "u8);
                s.Write(item.TypeName);
                s.Write(", "u8);
                s.Write(classNameTrim);
                s.Write("_> "u8);
                s.Write(propertyName);
                s.Write(" { get; }\r\n"u8);
                s.Write("        = new(Default"u8);
                s.Write(propertyName);
                s.Write(");"u8);
                s.Write("\r\n\r\n"u8);
            }
            else if (item.IsCollection(out value))
            {
                var value_ = Encoding.UTF8.GetBytes(value);
                var propertyName = Encoding.UTF8.GetBytes(item.PropertyName);
                s.Write("SettingsProperty<"u8);
                s.Write(value_);
                s.Write(", "u8);
                s.Write(item.TypeName);
                s.Write(", "u8);
                s.Write(classNameTrim);
                s.Write("_> "u8);
                s.Write(propertyName);
                s.Write(" { get; }\r\n"u8);
                s.Write("        = new(Default"u8);
                s.Write(propertyName);
                s.Write(");"u8);
                s.Write("\r\n\r\n"u8);
            }
            else if (item.GetIsValueType())
            {
                var propertyName = Encoding.UTF8.GetBytes(item.PropertyName);
                s.Write("SettingsStructProperty<"u8);
                s.Write(item.TypeName);
                s.Write(", "u8);
                s.Write(classNameTrim);
                s.Write("_> "u8);
                s.Write(propertyName);
                s.Write(" { get; }\r\n"u8);
                s.Write("        = new(Default"u8);
                s.Write(propertyName);
                s.Write(");"u8);
                s.Write("\r\n\r\n"u8);
            }
            else
            {
                var propertyName = Encoding.UTF8.GetBytes(item.PropertyName);
                s.Write("SettingsProperty<"u8);
                s.Write(item.TypeName);
                s.Write(", "u8);
                s.Write(classNameTrim);
                s.Write("_> "u8);
                s.Write(propertyName);
                s.Write(" { get; }\r\n"u8);
                s.Write("        = new(Default"u8);
                s.Write(propertyName);
                s.Write(");"u8);
                s.Write("\r\n\r\n"u8);
            }
        }
    }

    s.Write("}\r\n"u8);

#if DEBUG
    var str = Encoding.UTF8.GetString(s.ToByteArray());
    Console.WriteLine(str);
#endif
}

static void SourceGenerator_Interface(string name, Stream s, string @namespace, SettingsMetadata metadata, string usings = "")
{
    s.Write("""
        #nullable enable
        #pragma warning disable IDE0079 // 请删除不必要的忽略
        #pragma warning disable SA1634 // File header should show copyright
        #pragma warning disable CS8601 // 引用类型赋值可能为 null。
        #pragma warning disable CS0108 // 成员隐藏继承的成员；缺少关键字 new
        //------------------------------------------------------------------------------
        // <auto-generated>
        //     此代码由包 BD.Common.Settings.V4.SourceGenerator.Tools 源生成。
        //
        //     对此文件的更改可能会导致不正确的行为，并且如果
        //     重新生成代码，这些更改将会丢失。
        // </auto-generated>
        //------------------------------------------------------------------------------


        """u8);
    if (!string.IsNullOrWhiteSpace(usings))
        s.Write(usings);
    if (!string.IsNullOrWhiteSpace(metadata.Usings))
        s.Write(metadata.Usings);
    s.Write("""
        // ReSharper disable once CheckNamespace
        namespace 
        """u8);
    s.Write(@namespace);
    var classNameTrim = Encoding.UTF8.GetBytes(name);
    s.Write(".Abstractions;\r\n\r\npublic partial interface I"u8);
    s.Write(classNameTrim);
    s.Write("\r\n{\r\n    static I"u8);
    s.Write(classNameTrim);
    s.Write("? Instance\r\n        => Ioc.Get_Nullable<IOptionsMonitor<I"u8);
    s.Write(classNameTrim);
    s.Write(">>()?.CurrentValue;\r\n\r\n"u8);

    foreach (var item in metadata.Properties)
    {
        if (item.IsRegionOrEndregion.HasValue)
        {
            if (item.IsRegionOrEndregion.Value)
            {
                s.Write("    #region"u8);
            }
            else
            {
                s.Write("    #endregion"u8);
            }
            if (!string.IsNullOrWhiteSpace(item.Summary))
            {
                s.Write(" "u8);
                s.Write(item.Summary);
            }
            s.Write("\r\n\r\n"u8);
        }
        else if (!string.IsNullOrWhiteSpace(item.Sharp))
        {
            s.Write("#"u8);
            s.Write(item.Sharp);
            s.Write("\r\n\r\n"u8);
        }
        else
        {
            var summary = Encoding.UTF8.GetBytes(item.Summary);
            var typeName = Encoding.UTF8.GetBytes(item.TypeName);
            var propertyName = Encoding.UTF8.GetBytes(item.PropertyName);

            s.Write("    /// <summary>\r\n    /// "u8);
            s.Write(summary);
            s.Write("\r\n    /// </summary>\r\n"u8);
            s.Write("    "u8);
            s.Write(typeName);
            s.Write("? "u8);
            s.Write(propertyName);
            s.Write(" { get; set; }\r\n\r\n"u8);

            s.Write("    /// <summary>\r\n    /// "u8);
            s.Write(summary);
            s.Write("的默认值");
            s.Write("\r\n    /// </summary>\r\n"u8);
            if (item.DefaultValueIsConst)
            {
                s.Write("    const "u8);
            }
            else
            {
                s.Write("    static readonly "u8);
            }
            s.Write(typeName);
            if ((item.DefaultValueIsNullable.HasValue && item.DefaultValueIsNullable.Value)
                || (!item.GetIsValueType() && item.DefaultValue == "null"))
            {
                s.Write("?"u8);
            }
            s.Write(" Default"u8);
            s.Write(propertyName);
            s.Write(" = "u8);
            s.Write(item.DefaultValue);
            s.Write(";\r\n\r\n"u8);
        }
    }

    s.Write("}\r\n"u8);

#if DEBUG
    var str = Encoding.UTF8.GetString(s.ToByteArray());
    Console.WriteLine(str);
#endif
}

//AppSettings appSettings2 = AppSettings.Instance = new()
//{
//    Namespace = "BD.WTTS.Settings",
//    Settings = new()
//    {
//        { "UISettings", new("", "", new SP[] {
//                new SP(Summary: "主题", IsRegionOrEndregion: true),
//                new SP("AppTheme", "Theme", "AppTheme.FollowingSystem", true, "主题", IsValueType: true),
//                new SP("string", "ThemeAccent", "\"#FF0078D7\"", true, "主题强调色（16 进制 RGB 字符串）"),
//                new SP("bool", "UseSystemThemeAccent", "true", true, "从系统中获取主题强调色"),
//                new SP(IsRegionOrEndregion: false),
//                new SP("string", "Language", "null", true, "语言"),
//                new SP("HashSet<MessageBox.DontPromptType>", "MessageBoxDontPrompts", "null", true, "不再提示的消息框"),
//                new SP("bool", "IsShowAdvertisement", "true", true, "是否显示广告"),
//                new SP("ConcurrentDictionary<string, SizePosition>", "WindowSizePositions", "null", true, "不再提示的消息框"),
//                new SP("string", "FontName", "null", true, "字体"),
//                new SP("int", "GameListGridSize", "150", true, "库存游戏网格布局大小"),
//                new SP("bool", "Fillet", "false", true, "圆角"),
//                new SP(Summary: "WindowBackground 窗口背景", IsRegionOrEndregion: true),
//                new SP("double", "WindowBackgroundOpacity", ".8", true, "窗口背景不透明度"),
//                new SP("WindowBackgroundMaterial", "WindowBackgroundMaterial", """

//                OperatingSystem2.IsWindows11AtLeast() ? Enums.WindowBackgroundMaterial.Mica
//                : Enums.WindowBackgroundMaterial.AcrylicBlur
//                """, false, "窗口背景材质", IsValueType: true),
//                new SP("bool", "WindowBackgroundDynamic", "false", true, "动态桌面背景"),
//                new SP("string", "WindowBackgroundCustomImagePath", "\"/UI/Assets/back.png\"", true, "自定义背景图像路径"),
//                new SP("double", "WindowBackgroundCustomImageOpacity", ".8", true, "自定义背景图像不透明度"),
//                new SP("XamlMediaStretch", "WindowBackgroundCustomImageStretch", "XamlMediaStretch.UniformToFill", true, "自定义背景图像缩放方式", IsValueType: true),
//                new SP(IsRegionOrEndregion: false),
//            })
//        },
//    },
//};

//var a = JsonSerializer.Serialize<AppSettings>(appSettings2, AppSettingsContext.Instance.AppSettings);