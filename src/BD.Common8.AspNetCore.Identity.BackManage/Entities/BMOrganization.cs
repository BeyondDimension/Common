namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 系统组织实体类
/// </summary>
[Table("BM_Organizations")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public sealed class BMOrganization : TenantBaseEntity, INEWSEQUENTIALID, IOrder, IDisable
{
    /// <summary>
    /// 组织架构名称
    /// </summary>
    [Required]
    [Comment("组织架构名称")]
    [MaxLength(MaxLengths.Name)]
    public string OrganizationName { get; set; } = string.Empty;

    /// <inheritdoc/>
    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <summary>
    /// 父级 Id
    /// </summary>
    [Comment("父级 Id")]
    public Guid ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Comment("排序")]
    public long Order { get; set; }

    /// <inheritdoc cref="BMOrganization"/>
    public BMOrganization? Parent { get; set; }

    /// <inheritdoc cref="BMOrganization"/>
    public List<BMOrganization>? Children { get; set; }

    /// <inheritdoc cref="BMUserOrganization"/>
    public List<BMUserOrganization>? UserOrganizations { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }

    /// <inheritdoc cref="OperatorBaseEntity{TPrimaryKey}.EntityTypeConfiguration{TEntity}"/>
    public sealed class EntityTypeConfiguration : EntityTypeConfiguration<BMOrganization>
    {
        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<BMOrganization> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.Children)
                   .WithOne(x => x.Parent)
                   .HasForeignKey(x => x.ParentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}