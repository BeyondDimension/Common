namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 系统组织架构实体类
/// </summary>
[Table("BM_Organizations")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public sealed class SysOrganization : TenantBaseEntityV2, INEWSEQUENTIALID, IOrder, IDisable
{
    /// <summary>
    /// 组织架构名称
    /// </summary>
    [Required]
    [Comment("组织架构名称")]
    [MaxLength(MaxLengths.Name)]
    public string OrganizationName { get; set; } = string.Empty;

    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <summary>
    /// 父级 Id
    /// </summary>
    [Comment("父级 Id")]
    public Guid ParentId { get; set; }

    [Comment("排序")]
    public long Order { get; set; }

    /// <summary>
    /// 父级系统组织架构
    /// </summary>
    public SysOrganization? Parent { get; set; }

    /// <summary>
    /// 子级系统组织架构
    /// </summary>
    public List<SysOrganization>? Children { get; set; }

    public List<SysUserOrganization>? UserOrganizations { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }

    public sealed class EntityTypeConfiguration : EntityTypeConfiguration<SysOrganization>
    {
        public override void Configure(EntityTypeBuilder<SysOrganization> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.Children)
                   .WithOne(x => x.Parent)
                   .HasForeignKey(x => x.ParentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}