using BD.SteamPointShop.Entities.Abstractions;

namespace BD.Common.Entities;

/// <summary>
/// 系统组织架构实体类
/// </summary>
[Table("BM_Organizations")]
public class SysOrganization : TenantBaseEntityV2, INEWSEQUENTIALID, IOrder
{
    /// <summary>
    /// 组织架构名称
    /// </summary>
    [Comment("组织架构名称")]
    [MaxLength(100)]
    public string OrganizationName { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    [Comment("状态")]
    public SysOrganizationStatus Status { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    [Comment("父级Id")]
    public Guid ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
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
}