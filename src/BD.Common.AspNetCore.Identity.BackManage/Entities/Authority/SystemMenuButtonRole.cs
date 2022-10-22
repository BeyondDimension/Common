namespace BD.Common.Entities;

/// <summary>
/// 角色菜单按钮关系
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public class SystemMenuButtonRole :
    Entity<Guid>,
    ITenant
{

    public Guid RoleId { get; set; }

    public Guid TenantId { get; set; }

    public Guid MenuId { get; set; }

    public Guid ButtonId { get; set; }

    /// <summary>
    /// 调用ApiKey 后台鉴权
    /// </summary>
    [MaxLength(200)]
    public string? ApiKey { get; set; }
}
