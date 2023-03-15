namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 用户组织架构关系
/// </summary>
[Table("BM_UserOrganizations")]
public class SysUserOrganization : INEWSEQUENTIALID, ITenant
{
    /// <summary>
    /// Id
    /// </summary>
    [Comment("Id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [Comment("租户Id")]
    [Required]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 组织架构Id
    /// </summary>
    [Comment("组织架构Id")]
    [Required]
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [Comment("用户Id")]
    [Required]
    public Guid UserId { get; set; }

    /// <inheritdoc cref="SysOrganization"/>
    public virtual SysOrganization? Organization { get; set; }

    /// <inheritdoc cref="SysUser"/>
    public virtual SysUser? User { get; set; }
}