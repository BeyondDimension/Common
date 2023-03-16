namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 用户组织架构关系
/// </summary>
[Table("BM_UserOrganizations")]
public class SysUserOrganization : ITenant
{
    /// <summary>
    /// 租户 Id
    /// </summary>
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 组织架构 Id
    /// </summary>
    [Comment("组织架构 Id")]
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    [Comment("用户 Id")]
    public Guid UserId { get; set; }

    /// <inheritdoc cref="SysOrganization"/>
    public virtual SysOrganization? Organization { get; set; }

    /// <inheritdoc cref="SysUser"/>
    public virtual SysUser? User { get; set; }
}