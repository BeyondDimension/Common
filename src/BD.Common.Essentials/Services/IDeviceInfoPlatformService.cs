using EPlatform = System.Runtime.Devices.Platform;

namespace BD.Common.Services;

interface IDeviceInfoPlatformService
{
    static IDeviceInfoPlatformService? Interface => Ioc.Get_Nullable<IDeviceInfoPlatformService>();

    string Model { get; }

    string Manufacturer { get; }

    string Name { get; }

    string VersionString { get; }

    DeviceType DeviceType { get; }

    bool IsChromeOS { get; }

    bool IsUWP { get; }

    bool IsWinUI { get; }

    DeviceIdiom Idiom { get; }

    static EPlatform Platform
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return EPlatform.Windows;
            }
            else if (OperatingSystem.IsAndroid())
            {
                return EPlatform.Android;
            }
            else if (
                OperatingSystem.IsIOS() ||
                OperatingSystem.IsMacOS() ||
                OperatingSystem.IsTvOS() ||
                OperatingSystem.IsWatchOS())
            {
                return EPlatform.Apple;
            }
            else if (OperatingSystem.IsLinux())
            {
                return EPlatform.Linux;
            }
            return EPlatform.Unknown;
        }
    }
}
