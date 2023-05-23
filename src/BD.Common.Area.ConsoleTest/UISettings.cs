#pragma warning disable IDE0005 // Using 指令是不需要的。
global using BD.Common.Settings;
global using BD.Common.Settings.Abstractions;
#pragma warning restore IDE0005 // Using 指令是不需要的。

namespace BD.Common.UnitTest;

public partial interface IUISettings
{

}

public sealed partial class UISettings_
{

}

[JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyProperties = true)]
[JsonSerializable(typeof(UISettings_))]
partial class UISettingsContext : JsonSerializerContext
{

}

[SettingsV4Generation]
public static partial class UISettings
{
    const string DefaultThemeAccent = "#FF0078D7";

    /// <summary>
    /// 主题强调色（16 进制 RGB 字符串）
    /// </summary>
    public static SettingsProperty<string, UISettings_> ThemeAccent { get; }
        = new(DefaultThemeAccent);

    const bool DefaultUseSystemThemeAccent = true;

    /// <summary>
    /// 从系统中获取主题强调色
    /// </summary>
    public static SettingsStructProperty<bool, UISettings_> UseSystemThemeAccent { get; }
        = new(DefaultUseSystemThemeAccent);

    /// <summary>
    /// 语言
    /// </summary>
    public static SettingsProperty<string, UISettings_> Language { get; } = new();
}
