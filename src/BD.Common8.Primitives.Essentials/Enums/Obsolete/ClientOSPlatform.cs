#if DEBUG
namespace System.Runtime.Devices;

[Obsolete("use BD.Common8.Essentials.Enums.DevicePlatform2")]
public static partial class ClientOSPlatform
{
    public const DevicePlatform2 UWP = DevicePlatform2.UWP;
    public const DevicePlatform2 WindowsDesktopBridge = DevicePlatform2.WindowsDesktopBridge;
    public const DevicePlatform2 Windows = DevicePlatform2.Windows;
    public const DevicePlatform2 WSA = DevicePlatform2.WSA;
    public const DevicePlatform2 AndroidUnknown = DevicePlatform2.AndroidUnknown;
    public const DevicePlatform2 iPadOS = DevicePlatform2.iPadOS;
    public const DevicePlatform2 iOS = DevicePlatform2.iOS;
    public const DevicePlatform2 macOS = DevicePlatform2.macOS;
    public const DevicePlatform2 tvOS = DevicePlatform2.tvOS;
    public const DevicePlatform2 watchOS = DevicePlatform2.watchOS;
    public const DevicePlatform2 Linux = DevicePlatform2.Linux;
    public const DevicePlatform2 AndroidPhone = DevicePlatform2.AndroidPhone;
    public const DevicePlatform2 AndroidTablet = DevicePlatform2.AndroidTablet;
    public const DevicePlatform2 AndroidDesktop = DevicePlatform2.AndroidDesktop;
    public const DevicePlatform2 AndroidTV = DevicePlatform2.AndroidTV;
    public const DevicePlatform2 AndroidWatch = DevicePlatform2.AndroidWatch;
    public const DevicePlatform2 AndroidVirtual = DevicePlatform2.AndroidVirtual;
    public const DevicePlatform2 ChromeOS = DevicePlatform2.ChromeOS;
    public const DevicePlatform2 WinUI = DevicePlatform2.WinUI;
}
#endif