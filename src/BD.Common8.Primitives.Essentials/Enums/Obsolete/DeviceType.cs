#if DEBUG
using DeviceIdiomEnum = BD.Common8.Enums.DeviceIdiom;

namespace System.Runtime.Devices;

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