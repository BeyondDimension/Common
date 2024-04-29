using EditM = BD.Common8.AspNetCore.Models.Menus.BMMenuEdit;

namespace BD.Common8.AspNetCore.Controllers.Infrastructure;

/// <summary>
/// 后台管理 - 菜单管理
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BMMenusController"/> class.
/// </remarks>
/// <param name="menuRepo"></param>
/// <param name="logger"></param>
[Route("bm/menus")]
public class BMMenusController(IBMMenuRepository menuRepo,
    ILogger<BMMenusController> logger) : BaseAuthorizeController<BMMenusController>(logger)
{
    readonly IBMMenuRepository menuRepo = menuRepo;
    const string ControllerName = Controllers_SystemMenuManage;

    /// <summary>
    /// 查询后台菜单树结构 只支持 2 级
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ApiResponse<BMMenuTreeItem[]>> Tree()
    {
        var r = await menuRepo.GetTreeAsync();
        return r;
    }

    /// <summary>
    /// 查询 详情
    /// </summary>
    /// <param name="id">主键</param>
    /// <returns></returns>
    [HttpGet("{id}"), PermissionFilter(ControllerName + nameof(SysButtonType.Detail))]
    public async Task<ApiResponse<BMMenuModel2>> Get([FromRoute] Guid id)
    {
        var r = await menuRepo.InfoAsync(id);
        return r;
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPost, PermissionFilter(ControllerName + nameof(SysButtonType.Add))]
    public async Task<ApiResponse<bool>> Post([FromBody] EditM model)
    {
        if (!TryGetUserId(out var userId))
            return false;
        var (rowCount, _) = await menuRepo.InsertOrUpdateAsync(model, userId, TenantConstants.RootTenantIdG);
        return rowCount > 0;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns></returns>
    [HttpDelete("{id}"), PermissionFilter(ControllerName + nameof(SysButtonType.Delete))]
    public async Task<ApiResponse<bool>> Delete([FromRoute] Guid id)
    {
        if (!TryGetUserId(out var userId))
            return false;
        var state = await menuRepo.DeleteMenuAsync(id, TenantConstants.RootTenantIdG);
        return state;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPut, PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
    public async Task<ActionResult<ApiResponse<bool>>> Put([FromBody] EditM model)
    {
        var user = TryGetUserId(out var userId);
        if (!user) return NotFound();
        var (rowCount, _) = await menuRepo.InsertOrUpdateAsync(model, userId, TenantConstants.RootTenantIdG);
        return Ok(rowCount > 0);
    }

    #region 菜单权限

    /// <summary>
    /// 查询后台菜单树结构 只支持 2 级
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ApiResponse<BMMenuModel2[]>> RoleTree()
    {
        var r = await menuRepo.GetRoleTreeAsync();
        return r;
    }

    /// <summary>
    /// 获取按钮列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("bottons"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ActionResult<ApiResponse<BMButtonModel[]>>> GetButtons()
    {
        var r = await menuRepo.GetButtonsAsync();
        return Ok(r);
    }

    /// <summary>
    /// 获取菜单拥有的按钮列表
    /// </summary>
    /// <param name="menuId">菜单主键</param>
    /// <returns></returns>
    [HttpGet("bottons/{menuId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ActionResult<ApiResponse<Guid[]>>> GetMenuButtons([FromRoute] Guid menuId)
    {
        var r = await menuRepo.GetMenuButtonsAsync(menuId, TenantConstants.RootTenantIdG);
        return Ok(r);
    }

    /// <summary>
    /// 添加菜单拥有的按钮列表
    /// </summary>
    /// <param name="menuId">菜单主键</param>
    /// <param name="buttons">按钮列表</param>
    /// <returns></returns>
    [HttpPost("bottons/{menuId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Add))]
    public async Task<ActionResult<ApiResponse<bool>>> AddMenuButtons([FromRoute] Guid menuId, [FromBody] Guid[] buttons)
    {
        var r = await menuRepo.EditMenuButtonsAsync(menuId, buttons, TenantConstants.RootTenantIdG);
        return Ok(r);
    }

    /// <summary>
    /// 修改菜单拥有的按钮列表
    /// </summary>
    /// <param name="menuId">菜单主键</param>
    /// <param name="buttons">按钮列表</param>
    /// <returns></returns>
    [HttpPut("bottons/{menuId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
    public async Task<ActionResult<ApiResponse<bool>>> EditMenuButtons([FromRoute] Guid menuId, [FromBody] Guid[] buttons)
    {
        var r = await menuRepo.EditMenuButtonsAsync(menuId, buttons, TenantConstants.RootTenantIdG);
        return Ok(r);
    }

    /// <summary>
    /// 获取菜单拥有的按钮列表
    /// </summary>
    /// <param name="roleId">角色主键</param>
    /// <param name="menuId">菜单主键</param>
    /// <returns></returns>
    [HttpGet("bottons/{roleId}/{menuId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ActionResult<ApiResponse<BMButtonModel[]>>> GetRoleMenuButtonsAsync([FromRoute] Guid roleId, [FromRoute] Guid menuId)
    {
        var r = await menuRepo.GetRoleMenuButtonsAsync(roleId, menuId, TenantConstants.RootTenantIdG);
        return Ok(r);
    }

    /// <summary>
    /// 设置菜单权限 前端点击请求 添加查询权限
    /// </summary>
    /// <param name="roleId">角色主键</param>
    /// <param name="menuId">菜单主键</param>
    /// <param name="buttons">按钮列表</param>
    /// <returns></returns>
    [HttpPost("menus/{roleId}/{menuId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Add))]
    public async Task<ActionResult<ApiResponse<bool>>> AddMenuButtons([FromRoute] Guid roleId, [FromRoute] Guid menuId, [FromBody] BMButtonModel[] buttons)
    {
        var user = TryGetUserId(out var userId);
        if (!user) return NotFound();
        var r = await menuRepo.AddMenuButtonsAsync(userId, roleId, menuId, TenantConstants.RootTenantIdG, buttons);
        return Ok(r);
    }

    /// <summary>
    /// 设置菜单权限 前端保存
    /// </summary>
    /// <param name="roleId">角色主键</param>
    /// <param name="menuId">菜单主键</param>
    /// <param name="name">名称</param>
    /// <param name="buttons">按钮列表</param>
    /// <returns></returns>
    [HttpPut("menus/{roleId}/{menuId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
    public async Task<ActionResult<ApiResponse<bool>>> EditMenuButtons(
        [FromRoute] Guid roleId,
        [FromRoute] Guid menuId,
        [FromQuery] string name,
        [FromBody] BMButtonModel[] buttons)
    {
        var user = TryGetUserId(out var userId);
        if (!user) return NotFound();
        var r = await menuRepo.EditMenuButtonsAsync(name, userId, roleId, menuId, TenantConstants.RootTenantIdG, buttons);
        return Ok(r);
    }

    /// <summary>
    /// 设置菜单权限 前端点击请求
    /// </summary>
    /// <param name="roleId">角色主键</param>
    /// <param name="menuId">菜单主键</param>
    /// <returns></returns>
    [HttpDelete("menus/{roleId}/{menuId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMenuButtons([FromRoute] Guid roleId, [FromRoute] Guid menuId)
    {
        var user = TryGetUserId(out var userId);
        if (!user) return NotFound();
        var r = await menuRepo.DeleteMenuButtonsAsync(userId, roleId, menuId, TenantConstants.RootTenantIdG);
        return Ok(r);
    }

    #endregion
}