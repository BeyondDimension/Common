namespace BD.Common8.Primitives.District.Enums;

/// <summary>
/// 行政区域等级
/// </summary>
public enum DistrictLevel : byte
{
    省或直辖市或特别行政区 = 2,

    市_不包括直辖市 = 3,

    区县_县级市 = 4,
}