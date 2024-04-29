namespace BD.Common8.AspNetCore.Models.Menus;

/// <summary>
/// <see cref="BMMenu"/> 模型类
/// </summary>
public sealed class BMMenuModel : KeyModel<Guid>
{
    /// <inheritdoc cref="BMMenu.Key"/>
    public string Key { get; set; } = "";

    /// <inheritdoc cref="BMMenu.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="BMMenu.Url"/>
    public string Url { get; set; } = "";

    /// <inheritdoc cref="BMMenu.IconUrl"/>
    public string? IconUrl { get; set; } = "";

    /// <inheritdoc cref="BMMenu.Order"/>
    public long Order { get; set; }

    /// <inheritdoc cref="BMMenuModel"/>
    public BMMenuModel[]? Children { get; set; }

    /// <inheritdoc cref="BMButtonModel"/>
    public BMButtonModel[]? Buttons { get; set; }

    /// <summary>
    /// 判断 <see cref="Buttons"/> 是否具有指定操作按钮
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool HasBtnRole(SysButtonType type) => Buttons?.Any(x => x.Type == type) ?? false;

    //public Dictionary<SysButtonType, bool> GetBtnRoles()
    //    => Enum.GetValues<SysButtonType>().ToDictionary(x => x, HasBtnRole);
}

/// <summary>
/// <see cref="BMMenu"/> 模型类
/// </summary>
public sealed class BMMenuModel2 : KeyModel<Guid>
{
    /// <inheritdoc cref="BMMenu.ParentId"/>
    public Guid? ParentId { get; set; }

    /// <inheritdoc cref="BMMenu.Name"/>
    public string? Name { get; set; }

    /// <inheritdoc cref="BMMenu.IconUrl"/>
    public string? IconUrl { get; set; }

    /// <inheritdoc cref="BMMenu.Key"/>
    public string? Key { get; set; }

    /// <inheritdoc cref="BMMenu.Order"/>
    public long Order { get; set; }

    /// <inheritdoc cref="BMMenu.Remarks"/>
    public string? Remarks { get; set; }

    /// <inheritdoc cref="BMMenu.Url"/>
    public string? Url { get; set; }

    /// <summary>
    /// 表达式用于将 <see cref="BMMenu"/> 对象转换为 <see cref="BMMenuModel2"/> 对象
    /// </summary>
    public static readonly Expression<Func<BMMenu, BMMenuModel2>> Expression = item => new()
    {
        Id = item.Id,
        Name = item.Name,
        IconUrl = item.IconUrl,
        Key = item.Key,
        Order = item.Order,
        ParentId = item.ParentId,
        Url = item.Url,
        Remarks = item.Remarks,
    };
}