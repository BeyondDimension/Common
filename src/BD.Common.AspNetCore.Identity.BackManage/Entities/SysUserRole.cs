namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色用户关系
/// </summary>
[Table("BM_UserRoles")]
public class SysUserRole : ITenant
{
    /// <summary>
    /// 租户 Id
    /// </summary>
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    [Comment("用户 Id")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色 Id
    /// </summary>
    [Comment("角色 Id")]
    public Guid RoleId { get; set; }
}