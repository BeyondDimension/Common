namespace BD.Common8.AspNetCore.Repositories.Abstractions;

using EditModel = BD.Common8.AspNetCore.Models.SysMenuEdit;

public interface ISysMenuRepository
{
    /// <summary>
    /// 查询树结构
    /// </summary>
    /// <param name="takeCount"></param>
    /// <returns></returns>
    Task<SysMenuTreeItem[]> GetTreeAsync();

    /// <summary>
    /// 查询详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<SysMenuModel2?> InfoAsync(Guid id);

    /// <summary>
    /// 新增或修改
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(EditModel model, Guid userId, Guid tenantId);

    #region 菜单权限相关

    /// <summary>
    /// 获取菜单下的按钮
    /// </summary>
    /// <param name="menuId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<Guid[]> GetMenuButtonsAsync(Guid menuId, Guid tenantId);

    /// <summary>
    /// 获取用户菜单
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<SysMenuModel[]?> GetUserMenuAsync(Guid userId, Guid tenantId);

    /// <summary>
    /// 获取角色权限编辑菜单（暂时没有加基础菜单表等无筛选权限）
    /// </summary>
    /// <returns></returns>
    Task<SysMenuModel2[]> GetRoleTreeAsync();

    /// <summary>
    /// 获取系统按钮列表
    /// </summary>
    /// <returns></returns>
    Task<SysButtonModel[]> GetButtonsAsync();

    /// <summary>
    /// 获取角色菜单按钮
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="menuId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<SysButtonModel[]> GetRoleMenuButtonsAsync(
        Guid roleId,
        Guid menuId,
        Guid tenantId);

    /// <summary>
    /// 添加菜单权限
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="menuId"></param>
    /// <param name="tenantId"></param>
    /// <param name="buttons"></param>
    /// <returns></returns>
    Task<bool> AddMenuButtonsAsync(
         Guid userId,
         Guid roleId,
         Guid menuId,
         Guid tenantId,
         SysButtonModel[] buttons);

    /// <summary>
    /// 修改菜单权限
    /// </summary>
    /// <param name="name"></param>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="menuId"></param>
    /// <param name="tenantId"></param>
    /// <param name="buttons"></param>
    /// <returns></returns>
    Task<bool> EditMenuButtonsAsync(
         string name,
         Guid userId,
         Guid roleId,
         Guid menuId,
         Guid tenantId,
         SysButtonModel[] buttons);

    /// <summary>
    /// 删除菜单权限
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="menuId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<bool> DeleteMenuButtonsAsync(
        Guid userId,
        Guid roleId,
        Guid menuId,
        Guid tenantId);

    /// <summary>
    /// 新增修改菜单的按钮
    /// </summary>
    /// <param name="menuId"></param>
    /// <param name="buttons"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<bool> EditMenuButtonsAsync(Guid menuId, Guid[] buttons, Guid tenantId);

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="menuId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<bool> DeleteMenuAsync(
        Guid menuId,
        Guid tenantId);

    /// <summary>
    /// 获取用户角色权限
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<Guid[]> GetRoleMenus(Guid userId, Guid? tenantId);

    /// <summary>
    /// 优化获取菜单
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<SysMenuModel[]?> GetUserMenu2Async(Guid userId, Guid tenantId);

    #endregion
}
