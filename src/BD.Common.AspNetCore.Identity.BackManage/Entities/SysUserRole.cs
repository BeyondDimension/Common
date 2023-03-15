namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色用户关系
/// </summary>
[Table("BM_UserRoles")]
public class SysUserRole : ITenant
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [Comment("租户Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [Comment("用户Id")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色Id
    /// </summary>
    [Comment("角色Id")]
    public Guid RoleId { get; set; }
}