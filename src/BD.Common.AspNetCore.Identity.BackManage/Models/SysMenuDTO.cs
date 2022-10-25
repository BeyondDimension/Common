namespace BD.Common.Models;

/// <inheritdoc cref="SysMenu"/>
public sealed class SysMenuDTO : KeyModel<Guid>
{
    /// <inheritdoc cref="SysMenu.Key"/>
    public string Key { get; set; } = "";

    /// <inheritdoc cref="SysMenu.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysMenu.Path"/>
    public string Path { get; set; } = "";

    /// <inheritdoc cref="SysMenu.Icon"/>
    public string Icon { get; set; } = "";

    /// <inheritdoc cref="SysMenu.Order"/>
    public long Order { get; set; }

    public SysMenuDTO[]? Children { get; set; }

    public SysButtonDTO[]? Buttons { get; set; }

    public bool HasBtnRole(SysButtonType type) => Buttons?.Any(x => x.Type == type) ?? false;

    //public Dictionary<SysButtonType, bool> GetBtnRoles()
    //    => Enum.GetValues<SysButtonType>().ToDictionary(x => x, HasBtnRole);
}
