#if DEBUG
using DeviceTypeEnum = BD.Common8.Enums.DeviceType;

namespace System.Runtime.Devices;

[Obsolete("use BD.Common8.Essentials.Enums.DeviceType")]
public static partial class DeviceType
{
    public const DeviceTypeEnum Unknown = DeviceTypeEnum.Unknown;
    public const DeviceTypeEnum Physical = DeviceTypeEnum.Physical;
    public const DeviceTypeEnum Virtual = DeviceTypeEnum.Virtual;
}
#endif