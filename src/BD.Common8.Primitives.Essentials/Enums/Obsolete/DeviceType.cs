#if DEBUG
using DeviceIdiomEnum = BD.Common8.Primitives.Essentials.Enums.DeviceIdiom;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Runtime.Devices;

#pragma warning disable SA1600 // Elements should be documented

[Obsolete("use BD.Common8.Essentials.Enums.DeviceIdiom")]
public static partial class DeviceIdiom
{
    public const DeviceIdiomEnum Unknown = DeviceIdiomEnum.Unknown;
    public const DeviceIdiomEnum Phone = DeviceIdiomEnum.Phone;
    public const DeviceIdiomEnum Tablet = DeviceIdiomEnum.Tablet;
    public const DeviceIdiomEnum Desktop = DeviceIdiomEnum.Desktop;
    public const DeviceIdiomEnum TV = DeviceIdiomEnum.TV;
    public const DeviceIdiomEnum Watch = DeviceIdiomEnum.Watch;
}
#endif