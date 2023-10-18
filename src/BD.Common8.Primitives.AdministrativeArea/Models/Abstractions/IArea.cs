namespace BD.Common8.Primitives.AdministrativeArea.Models.Abstractions;

public partial interface IArea
{
    /// <summary>
    /// 地区 Id
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// 地区名的最大长度
    /// </summary>
    const int MaxLength_AreaName = 90;

    /// <summary>
    /// 地区名
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// 地区等级
    /// </summary>
    AreaLevel Level { get; set; }

    /// <summary>
    /// 上一级地区 Id
    /// </summary>
    int? Up { get; set; }

    /// <summary>
    /// 短名称
    /// </summary>
    string? ShortName { get; set; }

    /// <inheritdoc cref="object.ToString"/>
    protected static string ToString(IArea area)
    {
        if (!string.IsNullOrWhiteSpace(area.ShortName))
            return area.ShortName;
        if (!string.IsNullOrWhiteSpace(area.Name))
            return area.Name;
        return area.Id.ToString();
    }
}
