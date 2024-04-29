namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统菜单
/// </summary>
[Table("BM_Menus")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public sealed class BMMenu : TenantBaseEntity, INEWSEQUENTIALID, IOrder, IRemarks, IDisable
{
    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.MenuKey)]
    [Comment("按钮多语言名称")]
    public string Key { get; set; } = "";

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.MenuName)]
    [Comment("菜单名称")]
    public string Name { get; set; } = "";

    /// <summary>
    /// 父菜单 Id
    /// </summary>
    [Comment("父菜单 Id")]
    public Guid? ParentId { get; set; }

    /// <inheritdoc cref="BMMenu"/>
    public BMMenu? Parent { get; set; }

    /// <summary>
    /// 菜单 Url
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Url)]
    [Comment("菜单 Url")]
    public string Url { get; set; } = "";

    /// <summary>
    /// 图标 Url，或 IconKey，前端去根据开头是否为 http:// or https:// 自己识别
    /// </summary>
    [MaxLength(MaxLengths.Url)]
    [Comment("图标 Url")]
    public string? IconUrl { get; set; }

    /// <inheritdoc/>
    [Comment("排序")]
    public long Order { get; set; }

    /// <inheritdoc/>
    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <summary>
    /// 打开方式
    /// </summary>
    [Comment("打开方式")]
    public SysMenuOpenMethod OpenMethod { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Comment("备注")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 子级系统菜单
    /// </summary>
    public List<BMMenu>? Children { get; set; }

    /// <inheritdoc cref="BMButton"/>
    public List<BMButton>? Buttons { get; set; }

    /// <inheritdoc cref="BMMenuButton"/>
    public List<BMMenuButton>? MenuButtons { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }

    /// <inheritdoc cref="OperatorBaseEntity{TPrimaryKey}.EntityTypeConfiguration{TEntity}"/>
    public sealed class EntityTypeConfiguration : EntityTypeConfiguration<BMMenu>
    {
        /// <inheritdoc/>
        public sealed override void Configure(EntityTypeBuilder<BMMenu> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.Children)
                   .WithOne(x => x.Parent)
                   .HasForeignKey(x => x.ParentId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Buttons)
                   .WithMany(x => x.Menus)
                   .UsingEntity<BMMenuButton>(
                       x => x.HasOne(y => y.Button)
                             .WithMany(y => y.MenuButtons)
                             .HasForeignKey(y => y.ButtonId)
                             .OnDelete(DeleteBehavior.Cascade),
                       x => x.HasOne(y => y.Menu)
                             .WithMany(y => y.MenuButtons)
                             .HasForeignKey(y => y.MenuId)
                             .OnDelete(DeleteBehavior.Cascade),
                       x => x.HasKey(y => new { y.MenuId, y.ButtonId, y.TenantId })
                   );
        }
    }
}