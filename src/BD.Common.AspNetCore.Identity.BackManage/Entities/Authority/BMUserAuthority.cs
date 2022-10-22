namespace BD.Common.Entities;

public class BMUserAuthority : Entity<Guid>, ITenant
{
    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public Guid RoleId { get; set; }

}
