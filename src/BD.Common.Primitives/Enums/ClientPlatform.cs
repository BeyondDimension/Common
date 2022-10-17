namespace BD.Common.Enums;

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
}
