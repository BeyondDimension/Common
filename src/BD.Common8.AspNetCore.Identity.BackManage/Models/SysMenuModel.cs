namespace BD.Common8.AspNetCore.Models;

#pragma warning disable SA1600 // Elements should be documented

/// <inheritdoc cref="SysMenu"/>
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

    public SysMenuModel[]? Children { get; set; }

    public SysButtonModel[]? Buttons { get; set; }

    public bool HasBtnRole(SysButtonType type) => Buttons?.Any(x => x.Type == type) ?? false;

    //public Dictionary<SysButtonType, bool> GetBtnRoles()
    //    => Enum.GetValues<SysButtonType>().ToDictionary(x => x, HasBtnRole);
}

/// <inheritdoc cref="SysMenu"/>
public sealed class SysMenuModel2 : KeyModel<Guid>
{
    public Guid? ParentId { get; set; }

    public string? Name { get; set; }

    public string? IconUrl { get; set; }

    public string? Key { get; set; }

    public long Order { get; set; }

    public string? Remarks { get; set; }

    public string? Url { get; set; }

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