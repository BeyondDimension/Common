namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// <see cref="SysMenu"/> 模型类
/// </summary>
public sealed class SysMenuModel : KeyModel<Guid>
{
    /// <inheritdoc cref="SysMenu.Key"/>
    public string Key { get; set; } = "";

    /// <inheritdoc cref="SysMenu.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysMenu.Url"/>
    public string Url { get; set; } = "";

    /// <inheritdoc cref="SysMenu.IconUrl"/>
    public string? IconUrl { get; set; } = "";

    /// <inheritdoc cref="SysMenu.Order"/>
    public long Order { get; set; }

    /// <inheritdoc cref="SysMenuModel"/>
    public SysMenuModel[]? Children { get; set; }

    /// <inheritdoc cref="SysButtonModel"/>
    public SysButtonModel[]? Buttons { get; set; }

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
/// <see cref="SysMenu"/> 模型类
/// </summary>
public sealed class SysMenuModel2 : KeyModel<Guid>
{
    /// <inheritdoc cref="SysMenu.ParentId"/>
    public Guid? ParentId { get; set; }

    /// <inheritdoc cref="SysMenu.Name"/>
    public string? Name { get; set; }

    /// <inheritdoc cref="SysMenu.IconUrl"/>
    public string? IconUrl { get; set; }

    /// <inheritdoc cref="SysMenu.Key"/>
    public string? Key { get; set; }

    /// <inheritdoc cref="SysMenu.Order"/>
    public long Order { get; set; }

    /// <inheritdoc cref="SysMenu.Remarks"/>
    public string? Remarks { get; set; }

    /// <inheritdoc cref="SysMenu.Url"/>
    public string? Url { get; set; }

    /// <summary>
    /// 表达式用于将 <see cref="SysMenu"/> 对象转换为 <see cref="SysMenuModel2"/> 对象
    /// </summary>
    public static readonly Expression<Func<SysMenu, SysMenuModel2>> Expression = item => new()
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