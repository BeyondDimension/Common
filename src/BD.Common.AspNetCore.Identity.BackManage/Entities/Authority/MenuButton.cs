namespace BD.Common.Entities;

public class MenuButton : Entity<Guid>, IOrder
{

    /// <summary>
    /// 租户
    /// </summary>
    public Guid RoleId { get; set; }

    public Guid MenuId { get; set; }

    public Guid BottonId { get; set; }

    public long Order { get; set; }
}
