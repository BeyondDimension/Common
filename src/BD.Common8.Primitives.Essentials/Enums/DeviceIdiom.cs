namespace BD.Common8.Primitives.Essentials.Enums;

/// <summary>
/// 设备种类，例如手机，平板，电视，手表
/// <para>https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.maui.devices.deviceidiom</para>
/// <para>https://learn.microsoft.com/zh-cn/dotnet/api/xamarin.essentials.deviceidiom</para>
/// </summary>
[Flags]
public enum DeviceIdiom
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 1,

    /// <summary>
    /// 手机
    /// </summary>
    Phone = 4,

    /// <summary>
    /// 平板电脑
    /// </summary>
    Tablet = 8,

    /// <summary>
    /// 桌面
    /// </summary>
    Desktop = 16,

    /// <summary>
    /// 电视
    /// </summary>
    TV = 32,

    /// <summary>
    /// 手表
    /// </summary>
    Watch = 64,

    //_ = 128
    //_ = 256
    //_ = 512
    //_ = 1024
    //_ = 2048
    //_ = 4096
    //_ = 8192
    //_ = 16384,
    //_ = 32768,
    //_ = 65536,
    //_ = 131072,
    //_ = 262144,
}

/// <summary>
/// Enum 扩展 <see cref="DeviceIdiom"/>
/// </summary>
public static partial class IdiomEnumExtensions
{
    /// <summary>
    /// 值是否在定义的范围中，排除 <see cref="DeviceIdiom.Unknown"/>
    /// </summary>
    /// <param name="deviceIdiom"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined(this DeviceIdiom deviceIdiom)
        => deviceIdiom != DeviceIdiom.Unknown &&
            Enum.IsDefined(deviceIdiom);
}