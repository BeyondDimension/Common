namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IDeviceInfoPlatformService
{
    static IDeviceInfoPlatformService? Interface => Ioc.Get_Nullable<IDeviceInfoPlatformService>();

    string Model { get; }

    string Manufacturer { get; }

    string Name { get; }

    string VersionString { get; }

    DeviceType DeviceType { get; }

    bool IsChromeOS { get; }

    DeviceIdiom Idiom { get; }

    DevicePlatform2 Platform { get; }
}
