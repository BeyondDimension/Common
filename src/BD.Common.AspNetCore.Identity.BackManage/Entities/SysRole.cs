namespace BD.Common.Entities;

/// <summary>
/// 系统角色(权限)实体类
/// </summary>
[Table("BM_Roles")]
public class SysRole : TenantBaseEntityV2, INEWSEQUENTIALID, IOrder
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Comment("角色名称")]
    public string Name { get; set; } = "";

    /// <summary>
    /// 规范化名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Comment("规范化名称")]
    public string NormalizedName { get; set; } = "";

    /// <summary>
    /// 描述
    /// </summary>
    [Comment("描述")]
    public string? Describe { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    [Comment("用户 Id")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Comment("排序")]
    public long Order { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Comment("状态")]
    public SysRoleStatus Status { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }
}