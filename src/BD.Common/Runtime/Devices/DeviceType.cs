namespace System.Runtime.Devices;

/// <summary>
/// 设备类型
/// <para>https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.maui.devices.devicetype</para>
/// <para>https://learn.microsoft.com/zh-cn/dotnet/api/xamarin.essentials.devicetype</para>
/// </summary>
public enum DeviceType
{
    /// <summary>
    /// 一个未知的设备类型
    /// </summary>
    Unknown,

    /// <summary>
    /// 该设备是一个物理设备，如 iPhone、Android 平板电脑或 Windows 桌面
    /// </summary>
    Physical,

    /// <summary>
    /// 该设备是虚拟的，如 iOS 模拟器、Android 模拟器或 Windows 模拟器
    /// </summary>
    Virtual,
}

/// <summary>
/// Enum 扩展 <see cref="DeviceType"/>
/// </summary>
public static partial class DeviceTypeExtensions
{
    /// <summary>
    /// 值是否在定义的范围中，排除 <see cref="DeviceType.Unknown"/>
    /// </summary>
    /// <param name="deviceType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined(this DeviceType deviceType)
        => deviceType != DeviceType.Unknown &&
            Enum.IsDefined(deviceType);
}