namespace BD.Common.Entities;

/// <summary>
/// 组织架构
/// </summary>
public class OrganizationalStructure : TenantEntity<Guid>, IOrder
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.NickName)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父 ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public long Order { get; set; }
}

