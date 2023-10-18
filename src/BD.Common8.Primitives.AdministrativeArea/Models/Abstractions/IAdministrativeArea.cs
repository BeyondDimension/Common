namespace BD.Common8.Primitives.AdministrativeArea.Models.Abstractions;

/// <summary>
/// 行政区划接口
/// </summary>
public partial interface IAdministrativeArea
{
    /// <summary>
    /// 行政区划 Id
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// 行政区划名的最大长度
    /// </summary>
    const int MaxLength_AreaName = 90;

    /// <summary>
    /// 行政区划名
    /// </summary>
    string? Name { get; set; }

    /// <inheritdoc cref="AdministrativeAreaLevel"/>
    AdministrativeAreaLevel Level { get; set; }

    /// <summary>
    /// 上一级行政区划 Id
    /// </summary>
    int? Up { get; set; }

    /// <summary>
    /// 行政区划短名称
    /// </summary>
    string? ShortName { get; set; }

    /// <inheritdoc cref="object.ToString"/>
    protected static string ToString(IAdministrativeArea area)
    {
        if (!string.IsNullOrWhiteSpace(area.ShortName))
            return area.ShortName;
        if (!string.IsNullOrWhiteSpace(area.Name))
            return area.Name;
        return area.Id.ToString();
    }
}
