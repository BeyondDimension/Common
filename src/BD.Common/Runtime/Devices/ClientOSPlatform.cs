namespace System.Runtime.Devices;

/// <summary>
/// 客户端系统平台
/// </summary>
public enum ClientOSPlatform : byte
{
    /// <summary>
    /// Universal Windows Platform / 通用 Windows 平台
    /// </summary>
    UWP = 1,

    /// <summary>
    /// Windows 桌面桥应用
    /// <para>桌面桥应用是使用桌面桥转换为通用 Windows 平台 (UWP) 应用的 Windows 桌面应用程序。 转换后，将以面向 Windows 10 桌面版的 UWP 应用包（.appx 或 .appxbundle）的形式打包、维护和部署 Windows 桌面应用程序。</para>
    /// </summary>
    WindowsDesktopBridge,

    /// <summary>
    /// 桌面 Win32 应用
    /// </summary>
    Windows,

    /// <summary>
    /// 适用于 Android™ 的 Windows 子系统
    /// </summary>
    WSA,

    /// <summary>
    /// 未知设备类型的 Android
    /// </summary>
    AndroidUnknown,

    /// <summary>
    /// https://www.apple.com.cn/ipados
    /// </summary>
    iPadOS,

    /// <summary>
    /// https://developer.apple.com/cn/ios
    /// </summary>
    iOS,

    /// <summary>
    /// https://www.apple.com.cn/macos
    /// </summary>
    macOS,

    /// <summary>
    /// https://developer.apple.com/cn/tvos
    /// </summary>
    tvOS,

    /// <summary>
    /// https://www.apple.com.cn/watchos
    /// </summary>
    watchOS,

    /// <summary>
    /// GNU/Linux
    /// </summary>
    Linux,

    /// <summary>
    /// Android 手机
    /// </summary>
    AndroidPhone,

    /// <summary>
    /// Android 平板
    /// </summary>
    AndroidTablet,

    /// <summary>
    /// Android 桌面端
    /// </summary>
    AndroidDesktop,

    /// <summary>
    /// https://developer.android.google.cn/training/tv
    /// </summary>
    AndroidTV,

    /// <summary>
    /// https://developer.android.google.cn/training/wearables
    /// </summary>
    AndroidWatch,

    /// <summary>
    /// Android 模拟器
    /// </summary>
    AndroidVirtual,

    /// <summary>
    /// https://github.com/chromeos
    /// </summary>
    ChromeOS,

    /// <summary>
    /// Windows UI 库 (WinUI) 3
    /// </summary>
    WinUI,
}

public static class ClientOSPlatformEnumExtensions
{
    const string WindowsDesktopBridge = "Windows Desktop Bridge";
    const string Android = "Android";

    public static string ToDisplayName(this ClientOSPlatform value) => value == default ? string.Empty :
       (value.IsAndroid() ? Android :
           value switch
           {
               ClientOSPlatform.WindowsDesktopBridge => WindowsDesktopBridge,
               _ => value.ToString(),
           });

    public static ClientOSPlatform Parse(string value)
    {
        if (Enum.TryParse<ClientOSPlatform>(value, true, out var valueEnum)) return valueEnum;
        if (string.Equals(value, WindowsDesktopBridge, StringComparison.OrdinalIgnoreCase))
            return ClientOSPlatform.WindowsDesktopBridge;
        return default;
    }

    public static bool IsAndroid(this ClientOSPlatform value) => value switch
    {
        ClientOSPlatform.WSA or
        ClientOSPlatform.AndroidUnknown or
        ClientOSPlatform.AndroidPhone or
        ClientOSPlatform.AndroidTablet or
        ClientOSPlatform.AndroidDesktop or
        ClientOSPlatform.AndroidTV or
        ClientOSPlatform.AndroidWatch or
        ClientOSPlatform.AndroidVirtual or
        ClientOSPlatform.ChromeOS => true,
        _ => false,
    };
}

[Obsolete("use ClientOSPlatform replace OSNames.Value", true)]
public static class OSNames
{
    [Obsolete("use ClientOSPlatform replace OSNames.Value", true)]
    public enum Value : byte
    {

    }

    [Obsolete("use ClientOSPlatform replace OSNames.Value", true)]
    public static string ToDisplayName(this Value value)
    {
        throw new NotImplementedException();
    }

    [Obsolete("use ClientOSPlatform replace OSNames.Value", true)]
    public static Value Parse(string value)
    {
        throw new NotImplementedException();
    }

    [Obsolete("use ClientOSPlatform replace OSNames.Value", true)]
    public static bool IsAndroid(this Value value)
    {
        throw new NotImplementedException();
    }
}