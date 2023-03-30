namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统菜单
/// </summary>
[Table("BM_Menus")]
public sealed class SysMenu : TenantBaseEntityV2, INEWSEQUENTIALID, IOrder, IRemarks, IDisable
{
    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Name)]
    [Comment("按钮多语言名称")]
    public string Key { get; set; } = "";

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Name)]
    [Comment("菜单名称")]
    public string Name { get; set; } = "";

    /// <summary>
    /// 父菜单 Id
    /// </summary>
    [Comment("父菜单 Id")]
    public Guid? ParentId { get; set; }

    public SysMenu? Parent { get; set; }

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

    [Comment("排序")]
    public long Order { get; set; }

    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <summary>
    /// 打开方式
    /// </summary>
    [Comment("打开方式")]
    public SysMenuOpenMethod OpenMethod { get; set; }

    [Comment("备注")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 子级系统菜单
    /// </summary>
    public List<SysMenu>? Children { get; set; }

    /// <inheritdoc cref="SysButton"/>
    public List<SysButton>? Buttons { get; set; }

    /// <inheritdoc cref="SysMenuButton"/>
    public List<SysMenuButton>? MenuButtons { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }

    public sealed class EntityTypeConfiguration : EntityTypeConfiguration<SysMenu>
    {
        public sealed override void Configure(EntityTypeBuilder<SysMenu> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.Children)
                   .WithOne(x => x.Parent)
                   .HasForeignKey(x => x.ParentId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Buttons)
                   .WithMany(x => x.Menus)
                   .UsingEntity<SysMenuButton>(
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