namespace BD.Common.Entities;

public class BMUserAuthority
{
    public Guid UserId { get; set; }

    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }

}
