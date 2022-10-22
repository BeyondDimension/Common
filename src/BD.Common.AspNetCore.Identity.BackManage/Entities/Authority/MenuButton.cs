namespace BD.Common.Entities;

public class MenuButton : Entity<Guid>, IOrder, ITenant
{
    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }

    public Guid MenuId { get; set; }

    public Guid ButtonId { get; set; }

    public long Order { get; set; }
}
