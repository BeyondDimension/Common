namespace System.Runtime.Devices;

/// <summary>
/// 客户端平台
/// </summary>
[Flags]
public enum ClientPlatform : long
{
    /// <summary>
    /// Microsoft Windows(Win32) 32 位应用程序(x86)
    /// </summary>
    Win32X86 = 1L,

    /// <summary>
    /// Microsoft Windows(Win32) 64 位应用程序(x86-64/x64/AMD64)
    /// </summary>
    Win32X64 = 2L,

    /// <summary>
    /// Microsoft Windows(Win32) ARM 64 位应用程序(ARM64)
    /// </summary>
    Win32Arm64 = 4L,

    /// <summary>
    /// Apple macOS 64 位应用程序(x86-64/x64/AMD64)
    /// </summary>
    macOSX64 = 8L,

    /// <summary>
    /// Apple macOS ARM 64 位应用程序(M1/M2/ARM64)
    /// </summary>
    macOSArm64 = 16L,

    /// <summary>
    /// Ubuntu / Debian / CentOS 64 位应用程序(x86-64/x64/AMD64)
    /// </summary>
    LinuxX64 = 32L,

    /// <summary>
    /// Ubuntu / Debian / CentOS ARM 64 位应用程序(ARM64)
    /// </summary>
    LinuxArm64 = 128L,

    /// <summary>
    /// Ubuntu / Debian / CentOS ARM 32 位应用程序(ARM)
    /// </summary>
    LinuxArm = 256L,

    /// <summary>
    /// Android 64 位应用程序(x86-64/x64/AMD64/x86_64) for Phone
    /// </summary>
    AndroidPhoneX64 = 512L,

    /// <summary>
    /// Android 32 位应用程序(x86) for Phone
    /// </summary>
    AndroidPhoneX86 = 1024L,

    /// <summary>
    /// Android ARM 64 位应用程序(ARM64/arm64-v8a) for Phone
    /// </summary>
    AndroidPhoneArm64 = 2048L,

    /// <summary>
    /// Android ARM 32 位应用程序(ARM/armeabi-v7a) for Phone
    /// </summary>
    AndroidPhoneArm = 4096L,

    /// <summary>
    /// iOS ARM 64 位应用程序(ARM64/arm64-v8a)
    /// </summary>
    iOSArm64 = 8192L,

    /// <summary>
    /// iPadOS ARM 64 位应用程序(ARM64/arm64-v8a)
    /// </summary>
    iPadOSArm64 = 16384L,

    /// <summary>
    /// watchOS ARM 64 位应用程序(ARM64/arm64-v8a)
    /// </summary>
    watchOSArm64 = 32768L,

    /// <summary>
    /// tvOS ARM 64 位应用程序(ARM64/arm64-v8a)
    /// </summary>
    tvOSArm64 = 65536L,

    /// <summary>
    /// Android 64 位应用程序(x86-64/x64/AMD64/x86_64) for Pad
    /// </summary>
    AndroidPadX64 = 131072L,

    /// <summary>
    /// Android 32 位应用程序(x86) for Pad
    /// </summary>
    AndroidPadX86 = 262144L,

    /// <summary>
    /// Android ARM 64 位应用程序(ARM64/arm64-v8a) for Pad
    /// </summary>
    AndroidPadArm64 = 524288L,

    /// <summary>
    /// Android ARM 32 位应用程序(ARM/armeabi-v7a) for Pad
    /// </summary>
    AndroidPadArm = 1048576L,

    /// <summary>
    /// Android ARM 64 位应用程序(ARM64/arm64-v8a) for Wear
    /// </summary>
    AndroidWearArm64 = 2097152L,

    /// <summary>
    /// Android 64 位应用程序(x86-64/x64/AMD64/x86_64) for TV
    /// </summary>
    AndroidTVX64 = 4194304L,

    /// <summary>
    /// Android 32 位应用程序(x86) for TV
    /// </summary>
    AndroidTVX86 = 8388608L,

    /// <summary>
    /// Android ARM 64 位应用程序(ARM64/arm64-v8a) for TV
    /// </summary>
    AndroidTVArm64 = 16777216L,

    /// <summary>
    /// Android ARM 32 位应用程序(ARM/armeabi-v7a) for TV
    /// </summary>
    AndroidTVArm = 33554432L,

    /// <summary>
    /// Universal Windows Platform 32 位应用程序(x86)
    /// </summary>
    UWPX86 = 67108864L,

    /// <summary>
    /// Universal Windows Platform 64 位应用程序(x86-64/x64/AMD64)
    /// </summary>
    UWPX64 = 134217728L,

    /// <summary>
    /// Universal Windows Platform ARM 64 位应用程序(ARM64)
    /// </summary>
    UWPArm64 = 268435456L,

    /// <summary>
    /// Microsoft Store(Win32) 32 位应用程序(x86)
    /// </summary>
    Win32StoreX86 = 536870912L,

    /// <summary>
    /// Microsoft Store(Win32) 64 位应用程序(x86-64/x64/AMD64)
    /// </summary>
    Win32StoreX64 = 1073741824L,

    /// <summary>
    /// Microsoft Store(Win32) ARM 64 位应用程序(ARM64)
    /// </summary>
    Win32StoreArm64 = 2147483648L,

    /// <summary>
    /// Linux LoongArch 64 位应用程序(LoongArch64)
    /// </summary>
    LinuxLoongArch64 = 4294967296L,
}

public static class ClientPlatformExtensions
{
    #region Platform

    // 未知
    //const ClientPlatform Platform_Unknown =

    // Microsoft Windows(Win32)
    const ClientPlatform Platform_Windows =
        ClientPlatform.Win32X86 |
        ClientPlatform.Win32X64 |
        ClientPlatform.Win32Arm64;

    const ClientPlatform Platform_WindowsStore =
        Platform_Windows |
        ClientPlatform.Win32StoreX86 |
        ClientPlatform.Win32StoreX64 |
        ClientPlatform.Win32StoreArm64;

    // Ubuntu / Debian / CentOS / Tizen
    const ClientPlatform Platform_Linux =
        ClientPlatform.LinuxX64 |
        ClientPlatform.LinuxArm64 |
        ClientPlatform.LinuxArm |
        ClientPlatform.LinuxLoongArch64;

    // Android Phone / Android Pad / WearOS(Android Wear) / Android TV
    const ClientPlatform Platform_Android =
        ClientPlatform.AndroidPhoneX64 |
        ClientPlatform.AndroidPhoneX86 |
        ClientPlatform.AndroidPhoneArm64 |
        ClientPlatform.AndroidPhoneArm |
        ClientPlatform.AndroidPadX64 |
        ClientPlatform.AndroidPadX86 |
        ClientPlatform.AndroidPadArm64 |
        ClientPlatform.AndroidPadArm |
        ClientPlatform.AndroidWearArm64 |
        ClientPlatform.AndroidTVX64 |
        ClientPlatform.AndroidTVX86 |
        ClientPlatform.AndroidTVArm64 |
        ClientPlatform.AndroidTVArm;

    // iOS / iPadOS / watchOS / tvOS / macOS
    const ClientPlatform Platform_Apple =
        ClientPlatform.macOSX64 |
        ClientPlatform.macOSArm64 |
        ClientPlatform.iOSArm64 |
        ClientPlatform.iPadOSArm64 |
        ClientPlatform.watchOSArm64 |
        ClientPlatform.tvOSArm64;

    // Universal Windows Platform
    const ClientPlatform Platform_UWP =
        ClientPlatform.UWPX86 |
        ClientPlatform.UWPX64 |
        ClientPlatform.UWPArm64;

    // Windows UI 库 (WinUI) 3
    //const ClientPlatform Platform_WinUI =

    #endregion Platform

    #region ArchitectureFlags

    // 于 Intel 的 32 位处理器体系结构
    const ClientPlatform ArchitectureFlags_X86 =
        ClientPlatform.Win32X86 |
        ClientPlatform.AndroidPhoneX86 |
        ClientPlatform.AndroidPadX86 |
        ClientPlatform.AndroidTVX86 |
        ClientPlatform.UWPX86;

    const ClientPlatform ArchitectureStoreFlags_X86 =
        ArchitectureFlags_X86 |
        ClientPlatform.Win32StoreX86;

    // 32 位 ARM 处理器体系结构
    const ClientPlatform ArchitectureFlags_Arm =
        ClientPlatform.LinuxArm |
        ClientPlatform.AndroidPhoneArm |
        ClientPlatform.AndroidPadArm |
        ClientPlatform.AndroidTVArm;

    // 基于 Intel 的 64 位处理器体系结构(AMD64)
    const ClientPlatform ArchitectureFlags_X64 =
        ClientPlatform.Win32X64 |
        ClientPlatform.macOSX64 |
        ClientPlatform.LinuxX64 |
        ClientPlatform.AndroidPhoneX64 |
        ClientPlatform.AndroidPadX64 |
        ClientPlatform.AndroidTVX64 |
        ClientPlatform.UWPX64;

    const ClientPlatform ArchitectureStoreFlags_X64 =
        ArchitectureFlags_X64 |
        ClientPlatform.Win32StoreX64;

    // 64 位 ARM 处理器体系结构
    const ClientPlatform ArchitectureFlags_Arm64 =
        ClientPlatform.Win32Arm64 |
        ClientPlatform.macOSArm64 |
        ClientPlatform.LinuxArm64 |
        ClientPlatform.AndroidPhoneArm64 |
        ClientPlatform.iOSArm64 |
        ClientPlatform.iPadOSArm64 |
        ClientPlatform.watchOSArm64 |
        ClientPlatform.tvOSArm64 |
        ClientPlatform.AndroidPadArm64 |
        ClientPlatform.AndroidWearArm64 |
        ClientPlatform.AndroidTVArm64;

    const ClientPlatform ArchitectureStoreFlags_Arm64 =
        ArchitectureFlags_Arm64 |
        ClientPlatform.UWPArm64 |
        ClientPlatform.Win32StoreArm64;

    // const ClientPlatform ArchitectureFlags_Wasm =
    // const ClientPlatform ArchitectureFlags_S390x =

    // Linux LoongArch 64 位应用程序(LoongArch64)
    const ClientPlatform ArchitectureFlags_LoongArch64 =
        ClientPlatform.LinuxLoongArch64;

    // const ClientPlatform ArchitectureFlags_Armv6 =
    // const ClientPlatform ArchitectureFlags_Ppc64le =

    #endregion ArchitectureFlags

    #region DeviceIdiom

    // 未知
    //const ClientPlatform DeviceIdiom_Unknown =

    // 手机
    const ClientPlatform DeviceIdiom_Phone =
        ClientPlatform.AndroidPhoneX64 |
        ClientPlatform.AndroidPhoneX86 |
        ClientPlatform.AndroidPhoneArm64 |
        ClientPlatform.AndroidPhoneArm |
        ClientPlatform.iOSArm64;

    // 平板电脑
    const ClientPlatform DeviceIdiom_Tablet =
        ClientPlatform.iPadOSArm64 |
        ClientPlatform.AndroidPadX64 |
        ClientPlatform.AndroidPadX86 |
        ClientPlatform.AndroidPadArm64 |
        ClientPlatform.AndroidPadArm;

    // 桌面
    const ClientPlatform DeviceIdiom_Desktop =
        ClientPlatform.Win32X86 |
        ClientPlatform.Win32X64 |
        ClientPlatform.Win32Arm64 |
        ClientPlatform.UWPX86 |
        ClientPlatform.UWPX64 |
        ClientPlatform.UWPArm64 |
        ClientPlatform.macOSX64 |
        ClientPlatform.macOSArm64 |
        ClientPlatform.LinuxX64 |
        ClientPlatform.LinuxArm64 |
        ClientPlatform.LinuxArm |
        ClientPlatform.LinuxLoongArch64;

    const ClientPlatform DeviceIdiom_DesktopStore =
        DeviceIdiom_Desktop |
        Platform_WindowsStore;

    // 电视
    const ClientPlatform DeviceIdiom_TV =
        ClientPlatform.tvOSArm64 |
        ClientPlatform.AndroidTVX64 |
        ClientPlatform.AndroidTVX86 |
        ClientPlatform.AndroidTVArm64 |
        ClientPlatform.AndroidTVArm;

    // 手表
    const ClientPlatform DeviceIdiom_Watch =
        ClientPlatform.watchOSArm64 |
        ClientPlatform.AndroidWearArm64;

    #endregion DeviceIdiom

    public static Platform ToPlatform(this ClientPlatform source)
    {
        Platform result = default;
        foreach (var item in Enum2.FlagsSplit(source))
        {
            if (Platform_Windows.HasFlag(item)) result |= Platform.Windows;
            if (Platform_Linux.HasFlag(item)) result |= Platform.Linux;
            if (Platform_Android.HasFlag(item)) result |= Platform.Android;
            if (Platform_Apple.HasFlag(item)) result |= Platform.Apple;
            if (Platform_UWP.HasFlag(item)) result |= Platform.UWP;
        }
        return result != default ? result : Platform.Unknown;
    }

    public static ArchitectureFlags ToArchitectureFlags(this ClientPlatform source)
    {
        ArchitectureFlags result = default;
        foreach (var item in Enum2.FlagsSplit(source))
        {
            if (ArchitectureFlags_X86.HasFlag(item)) result |= ArchitectureFlags.X86;
            if (ArchitectureFlags_Arm.HasFlag(item)) result |= ArchitectureFlags.Arm;
            if (ArchitectureFlags_X64.HasFlag(item)) result |= ArchitectureFlags.X64;
            if (ArchitectureFlags_Arm64.HasFlag(item)) result |= ArchitectureFlags.Arm64;
            if (ArchitectureFlags_LoongArch64.HasFlag(item)) result |= ArchitectureFlags.LoongArch64;
        }
        return result != default ? result : default;
    }

    public static DeviceIdiom ToDeviceIdiom(this ClientPlatform source)
    {
        DeviceIdiom result = default;
        foreach (var item in Enum2.FlagsSplit(source))
        {
            if (DeviceIdiom_Phone.HasFlag(item)) result |= DeviceIdiom.Phone;
            if (DeviceIdiom_Tablet.HasFlag(item)) result |= DeviceIdiom.Tablet;
            if (DeviceIdiom_Desktop.HasFlag(item)) result |= DeviceIdiom.Desktop;
            if (DeviceIdiom_TV.HasFlag(item)) result |= DeviceIdiom.TV;
            if (DeviceIdiom_Watch.HasFlag(item)) result |= DeviceIdiom.Watch;
        }
        return result != default ? result : DeviceIdiom.Unknown;
    }

    public static ClientPlatform ToClientPlatform(this ArchitectureFlags source, bool isStore = true)
    {
        ClientPlatform result = default;
        foreach (var item in Enum2.FlagsSplit(source))
        {
            result |= item switch
            {
                ArchitectureFlags.Arm64 => isStore ? ArchitectureStoreFlags_Arm64 : ArchitectureFlags_Arm64,
                ArchitectureFlags.X86 => isStore ? ArchitectureStoreFlags_X86 : ArchitectureFlags_X86,
                ArchitectureFlags.Arm => ArchitectureFlags_Arm,
                ArchitectureFlags.X64 => isStore ? ArchitectureStoreFlags_X64 : ArchitectureFlags_X64,
                ArchitectureFlags.LoongArch64 => ArchitectureFlags_LoongArch64,
                _ => default,
            };
        }
        return result;
    }

    public static ClientPlatform ToClientPlatform(this Platform source, bool isStore = true)
    {
        ClientPlatform result = default;
        foreach (var item in Enum2.FlagsSplit(source))
        {
            result |= item switch
            {
                Platform.Windows => isStore ? Platform_WindowsStore : Platform_Windows,
                Platform.Linux => Platform_Linux,
                Platform.Android => Platform_Android,
                Platform.Apple => Platform_Apple,
                Platform.UWP => Platform_UWP,
                _ => default
            };
        }
        return result;
    }

    public static ClientPlatform ToClientPlatform(this DeviceIdiom source, bool isStore = true)
    {
        ClientPlatform result = default;
        foreach (var item in Enum2.FlagsSplit(source))
        {
            result |= item switch
            {
                DeviceIdiom.TV => DeviceIdiom_TV,
                DeviceIdiom.Phone => DeviceIdiom_Phone,
                DeviceIdiom.Tablet => DeviceIdiom_Tablet,
                DeviceIdiom.Desktop => isStore ? DeviceIdiom_DesktopStore : DeviceIdiom_Desktop,
                DeviceIdiom.Watch => DeviceIdiom_Watch,
                _ => default
            };
        }
        return result;
    }
}