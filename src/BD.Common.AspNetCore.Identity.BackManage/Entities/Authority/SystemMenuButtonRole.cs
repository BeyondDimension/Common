namespace BD.Common.Entities;

/// <summary>
/// 角色菜单按钮关系
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public class SystemMenuButtonRole<TPrimaryKey> :
    Entity<TPrimaryKey>,
    ITenant
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{

    public Guid RoleId { get; set; }

    public Guid TenantId { get; set; }

    public Guid MenuId { get; set; }

    public Guid ButtonId { get; set; }
}
