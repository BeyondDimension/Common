namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 权限控制相关实体类 - 菜单按钮关系
/// </summary>
[Table("BM_MenuButtons")]
public sealed class BMMenuButton : ITenant, IOrder
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

    /// <summary>
    /// 排序
    /// </summary>
    [Comment("排序")]
    public long Order { get; set; }

    /// <inheritdoc cref="BMButton"/>
    public BMButton? Button { get; set; }

    /// <inheritdoc cref="BMMenu"/>
    public BMMenu? Menu { get; set; }
}