namespace BD.Common8.AspNetCore.Repositories.Abstractions;

public interface ISysRoleRepository
{
    /// <summary>
    /// 查询角色表格
    /// </summary>
    /// <param name="name"></param>
    /// <param name="current"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<PagedModel<SysRoleModel>> QueryAsync(
        string? name,
        int current = IPagedModel.DefaultCurrent,
        int pageSize = IPagedModel.DefaultPageSize);

    /// <summary>
    /// 下拉列表获取
    /// </summary>
    /// <param name="takeCount">要获取的最大数量限制，默认值：<see cref="SelectItemModel.Count"/></param>
    /// <returns></returns>
    Task<SelectItemModel<Guid>[]> GetSelectAsync(int takeCount = SelectItemModel.Count);

    /// <summary>
    /// 传入权限查询拥有的菜单Id
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<Guid[]> GetRoleMenus(Guid roleId, Guid? tenantId);
}
