#if DEBUG
using DeviceTypeEnum = BD.Common8.Essentials.Enums.DeviceType;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Runtime.Devices;

#pragma warning disable SA1600 // Elements should be documented

[Obsolete("use BD.Common8.Essentials.Enums.DeviceType")]
public static partial class DeviceType
{
    public const DeviceTypeEnum Unknown = DeviceTypeEnum.Unknown;
    public const DeviceTypeEnum Physical = DeviceTypeEnum.Physical;
    public const DeviceTypeEnum Virtual = DeviceTypeEnum.Virtual;
}
#endif