namespace BD.Common8.Models.Abstractions;

/// <summary>
/// 行政区域
/// <para>https://lbs.amap.com/api/webservice/guide/api/district</para>
/// </summary>
public partial interface IDistrict
{
    /// <summary>
    /// 行政区域 Id
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// 行政区域名
    /// </summary>
    string? Name { get; set; }

    /// <inheritdoc cref="DistrictLevel"/>
    DistrictLevel Level { get; set; }

    /// <summary>
    /// 上一级行政区域 Id
    /// </summary>
    int? Up { get; set; }

    /// <summary>
    /// 行政区域短名称
    /// </summary>
    string? ShortName { get; set; }

    /// <inheritdoc cref="object.ToString"/>
    protected static string ToString(IDistrict area)
    {
        if (!string.IsNullOrWhiteSpace(area.ShortName))
            return area.ShortName;
        if (!string.IsNullOrWhiteSpace(area.Name))
            return area.Name;
        return area.Id.ToString();
    }
}
