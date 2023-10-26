namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 系统角色(权限)实体类
/// </summary>
[Table("BM_Roles")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public sealed class SysRole : TenantBaseEntityV2, INEWSEQUENTIALID, IOrder, IDisable, IDescribe
{
    /// <summary>
    /// 角色名
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Name)]
    [Comment("角色名")]
    public string Name { get; set; } = "";

    /// <summary>
    /// 规范化角色名
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Name)]
    [Comment("规范化角色名")]
    public string NormalizedName { get; set; } = "";

    [Comment("描述")]
    public string? Describe { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    [Comment("用户 Id")]
    public Guid UserId { get; set; }

    [Comment("排序")]
    public long Order { get; set; }

    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }

    public sealed class EntityTypeConfiguration : EntityTypeConfiguration<SysRole>
    {
        public sealed override void Configure(EntityTypeBuilder<SysRole> builder)
        {
            base.Configure(builder);
        }
    }
}