namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供设备信息的辅助方法
/// </summary>
public static class DeviceInfo2
{
    /// <summary>
    /// 获取设备型号
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Model() => IDeviceInfoPlatformService.Interface?.Model ?? string.Empty;

    /// <summary>
    /// 获取设备制造商
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Manufacturer() => IDeviceInfoPlatformService.Interface?.Manufacturer ?? string.Empty;

    /// <summary>
    /// 获取设备名称
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Name() => IDeviceInfoPlatformService.Interface?.Name ?? string.Empty;

    /// <summary>
    /// 获取设备版本信息
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string VersionString() => IDeviceInfoPlatformService.Interface?.VersionString ?? string.Empty;

    /// <summary>
    /// 获取设备平台
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 Platform() => IDeviceInfoPlatformService.Interface?.Platform ?? default;

    /// <summary>
    /// 获取设备种类
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeviceIdiom Idiom()
    {
        var i = IDeviceInfoPlatformService.Interface;
        if (i != null)
        {
            var value = i.Idiom;
            if (value != DeviceIdiom.Unknown)
                return value;
        }
        return DeviceIdiom.Unknown;
    }

    /// <summary>
    /// 获取设备类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeviceType DeviceType()
    {
        var i = IDeviceInfoPlatformService.Interface;
        if (i != null)
            return i.DeviceType;
        return default;
    }
}