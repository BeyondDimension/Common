namespace BD.Common8.Essentials.Services;

/// <summary>
/// 设备信息平台服务接口
/// </summary>
public interface IDeviceInfoPlatformService
{
    /// <summary>
    /// 获取 <see cref="IDeviceInfoPlatformService"/> 实例
    /// </summary>
    static IDeviceInfoPlatformService? Interface => Ioc.Get_Nullable<IDeviceInfoPlatformService>();

    /// <summary>
    /// 设备型号
    /// </summary>
    string Model { get; }

    /// <summary>
    /// 制造商
    /// </summary>
    string Manufacturer { get; }

    /// <summary>
    /// 设备名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 版本字符串
    /// </summary>
    string VersionString { get; }

    /// <summary>
    /// 设备类型
    /// </summary>
    DeviceType DeviceType { get; }

    /// <summary>
    /// 是否为 Chrome OS
    /// </summary>
    bool IsChromeOS { get; }

    /// <summary>
    /// 设备种类
    /// </summary>
    DeviceIdiom Idiom { get; }

    /// <summary>
    /// 设备平台
    /// </summary>
    DevicePlatform2 Platform { get; }
}
