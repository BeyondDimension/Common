namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色用户关系
/// </summary>
[Table(nameof(SysUserRole) + "s")]
public sealed class SysUserRole : ITenant
{
    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }
}
