namespace BD.Common8.AspNetCore.Entities;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 权限控制相关实体类 - 菜单按钮关系
/// </summary>
[Table("BM_MenuButtons")]
public sealed class SysMenuButton : ITenant, IOrder
{
    /// <inheritdoc/>
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 菜单 Id
    /// </summary>
    [Comment("菜单 Id")]
    public Guid MenuId { get; set; }

    /// <summary>
    /// 按钮 Id
    /// </summary>
    [Comment("按钮 Id")]
    public Guid ButtonId { get; set; }

    [Comment("排序")]
    public long Order { get; set; }

    /// <inheritdoc cref="SysButton"/>
    public SysButton? Button { get; set; }

    /// <inheritdoc cref="SysMenu"/>
    public SysMenu? Menu { get; set; }
}