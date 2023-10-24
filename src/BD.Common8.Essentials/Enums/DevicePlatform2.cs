namespace BD.Common8.Essentials.Enums;

/// <summary>
/// 应用程序正在运行的设备平台。
/// <para>https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.maui.devices.deviceplatform</para>
/// </summary>
public enum DevicePlatform2 : byte
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

    /// <summary>
    /// MacCatalyst 平台
    /// </summary>
    MacCatalyst,

    /// <summary>
    /// Tizen 平台
    /// </summary>
    Tizen,
}

/// <summary>
/// Enum 扩展 <see cref="DevicePlatform2"/>
/// </summary>
public static partial class DevicePlatform2EnumExtensions
{
    /// <summary>
    /// 值是否在定义的范围中，排除 default
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined(this DevicePlatform2 platform)
        => platform != default &&
            Enum.IsDefined(platform);
}