namespace BD.Common.Entities;

public class MenuButton : ITenant
{
    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }

    public Guid MenuId { get; set; }

    public Guid ButtonId { get; set; }
}
