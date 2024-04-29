namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色菜单按钮关系
/// </summary>
[Table("BM_MenuButtonRoles")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public sealed class BMMenuButtonRole : ITenant
{
    /// <inheritdoc/>
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 角色 Id
    /// </summary>
    [Comment("角色 Id")]
    public Guid RoleId { get; set; }

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
    /// 控制器名称
    /// </summary>
    [Required]
    [MaxLength(128)] // 此处硬编码
    [Comment("控制器名称")]
    public string ControllerName { get; set; } = "";

    /// <inheritdoc cref="IEntityTypeConfiguration{SysMenuButtonRole}"/>
    public sealed class EntityTypeConfiguration : IEntityTypeConfiguration<BMMenuButtonRole>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<BMMenuButtonRole> builder)
        {
            builder.HasKey(x => new { x.ButtonId, x.RoleId, x.MenuId, x.TenantId, x.ControllerName, });
        }
    }
}