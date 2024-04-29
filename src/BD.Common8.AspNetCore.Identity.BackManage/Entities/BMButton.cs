namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统按钮
/// </summary>
[Table("BM_Buttons")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public sealed class BMButton : TenantBaseEntity, INEWSEQUENTIALID, IDisable
{
    /// <summary>
    /// 按钮名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Name)]
    [Comment("按钮名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 按钮样式
    /// </summary>
    [Comment("按钮样式")]
    public SysButtonStyle Style { get; set; }

    /// <summary>
    /// 按钮类型
    /// </summary>
    [Comment("按钮类型")]
    public SysButtonType Type { get; set; }

    /// <inheritdoc/>
    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <inheritdoc cref="BMMenu"/>
    public List<BMMenu>? Menus { get; set; }

    /// <inheritdoc cref="BMMenuButton"/>
    public List<BMMenuButton>? MenuButtons { get; set; }

    /// <inheritdoc cref="OperatorBaseEntity{TPrimaryKey}.EntityTypeConfiguration{TEntity}"/>
    public sealed class EntityTypeConfiguration : EntityTypeConfiguration<BMButton>
    {
        /// <inheritdoc/>
        public sealed override void Configure(EntityTypeBuilder<BMButton> builder)
        {
            base.Configure(builder);
        }
    }
}