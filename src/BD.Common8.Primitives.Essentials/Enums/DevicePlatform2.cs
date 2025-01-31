namespace BD.Common8.Enums;

/// <summary>
/// 应用程序正在运行的设备平台
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
    const string WindowsDesktopBridge = "Windows Desktop Bridge";
    const string Android = "Android";

    /// <summary>
    /// 将 <see cref="DevicePlatform2"/> 转换为显示字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToDisplayName(this DevicePlatform2 value) => value == default ? string.Empty :
       (value.IsAndroid() ? Android :
           value switch
           {
               DevicePlatform2.WindowsDesktopBridge => WindowsDesktopBridge,
               _ => value.ToString(),
           });

    /// <summary>
    /// 值是否在定义的范围中，排除 default
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined(this DevicePlatform2 platform)
        => platform != default &&
#if NET5_0_OR_GREATER
            Enum.IsDefined(platform);
#else
            Enum.IsDefined(typeof(DevicePlatform2), platform);
#endif

    /// <summary>
    /// 将 <see cref="string"/> 转换为 <see cref="DevicePlatform2"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 Parse(string value)
    {
        if (Enum.TryParse<DevicePlatform2>(value, true, out var valueEnum))
            return valueEnum;
        if (string.Equals(value, WindowsDesktopBridge, StringComparison.OrdinalIgnoreCase))
            return DevicePlatform2.WindowsDesktopBridge;
        return default;
    }

    /// <summary>
    /// 判断 <see cref="DevicePlatform2"/> 值是否为 Android
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAndroid(this DevicePlatform2 value) => value switch
    {
        DevicePlatform2.WSA or
        DevicePlatform2.AndroidUnknown or
        DevicePlatform2.AndroidPhone or
        DevicePlatform2.AndroidTablet or
        DevicePlatform2.AndroidDesktop or
        DevicePlatform2.AndroidTV or
        DevicePlatform2.AndroidWatch or
        DevicePlatform2.AndroidVirtual or
        DevicePlatform2.ChromeOS => true,
        _ => false,
    };

    /// <summary>
    /// 根据【设备平台枚举】和【CPU 架构枚举】获取 RID 字符串
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="architecture"></param>
    /// <param name="throw"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRID(this DevicePlatform2 platform, Architecture architecture, bool @throw = true) => platform switch
    {
        DevicePlatform2.UWP or DevicePlatform2.WindowsDesktopBridge or
        DevicePlatform2.Windows or DevicePlatform2.WinUI
            => architecture.GetWindowsRID(@throw),
        DevicePlatform2.Linux
            => architecture.GetLinuxRID(@throw),
        DevicePlatform2.macOS or DevicePlatform2.MacCatalyst
            => architecture.GetOSXRID(@throw),
        _
            => @throw ? throw new ArgumentOutOfRangeException(nameof(platform), platform, null) : "",
    };
}