namespace BD.Common.Entities;

/// <summary>
/// 系统组织架构实体类
/// </summary>
[Table("BM_Organizations")]
public class SysOrganization : TenantBaseEntityV2, INEWSEQUENTIALID, IOrder, IDisable
{
    /// <summary>
    /// 组织架构名称
    /// </summary>
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
    public virtual SysOrganization? ParentSysOrganization { get; set; }

    /// <summary>
    /// 子级系统组织架构
    /// </summary>
    public virtual List<SysOrganization>? ChildrenSysOrganization { get; set; }

    public class EntityTypeConfiguration : EntityTypeConfiguration<SysOrganization>
    {
        public override void Configure(EntityTypeBuilder<SysOrganization> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.ChildrenSysOrganization)
                   .WithOne(x => x.ParentSysOrganization)
                   .HasForeignKey(x => x.ParentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}