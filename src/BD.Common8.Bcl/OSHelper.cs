namespace System;

#if ANDROID
using Build = Android.OS.Build;
using JavaSystem = Java.Lang.JavaSystem;
#endif

/// <summary>
/// 操作系统相关助手类
/// </summary>
public abstract partial class OSHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OSHelper"/> class.
    /// </summary>
    protected OSHelper() => throw new NotSupportedException();

#if WINDOWS
    /// <summary>
    /// 通过判断进程含有包标识返回 <see cref="bool"/>，从 Windows 10 版本 2004 开始，可以通过生成稀疏包并将其注册到应用，向未打包到 MSIX 包中的桌面应用授予包标识符。
    /// <para>https://docs.microsoft.com/zh-cn/windows/apps/desktop/modernize/grant-identity-to-nonpackaged-apps</para>
    /// </summary>
    public static bool IsRunningAsUwp { get; protected set; }
#endif

    /// <summary>
    /// 指示当前应用程序是否发布到应用商店中。
    /// </summary>
    public static bool IsPublishToStore { get; protected set; }

#if ANDROID
    static readonly Lazy<bool> _IsRunningOnWSA = new(() =>
    {
        string? model = Build.Model, manufacturer = Build.Manufacturer,
            brand = Build.Brand, device = Build.Device,
            product = Build.Product, hardware = Build.Hardware;
        if (model != "Subsystem for Android(TM)") return false;
        if (manufacturer != "Microsoft Corporation") return false;
        if (brand != "Windows") return false;
        if (device == null || !device.Contains("windows")) return false;
        if (product == null || !product.Contains("windows")) return false;
        if (hardware == null || !hardware.Contains("windows")) return false;
        var osVer = JavaSystem.GetProperty("os.version");
        if (osVer == null || !osVer.Contains("windows-subsystem-for-android")) return false;
        return true;
    });
#endif

    /// <summary>
    /// 指示当前应用程序是否正在 Windows Subsystem for Android™️ 上运行。
    /// </summary>
    [SupportedOSPlatformGuard("android")]
    public static bool IsRunningOnWSA() =>
#if ANDROID
        _IsRunningOnWSA.Value;
#else
        false;
#endif
}
