using AddOrEditM = BD.Common8.AspNetCore.Models.SysRoleModel;
using TableItemM = BD.Common8.AspNetCore.Models.SysRoleModel;

namespace BD.Common8.AspNetCore.Controllers;

/// <summary>
/// 后台管理 - 角色管理
/// </summary>
[Route("bm/roles")]
public sealed class BMRolesController : BaseAuthorizeController<BMRolesController>
{
    readonly ISysRoleRepository roleRepo;
    readonly IRoleManager roleManager;
    //readonly ISysMenuRepository menuRepo;
    const string ControllerName = Controllers_RoleManage;

    /// <summary>
    /// Initializes a new instance of the <see cref="BMRolesController"/> class.
    /// </summary>
    /// <param name="roleRepo"></param>
    /// <param name="roleManager"></param>
    /// <param name="logger"></param>
    public BMRolesController(ISysRoleRepository roleRepo,
        //ISysMenuRepository menuRepo,
        IRoleManager roleManager,
        ILogger<BMRolesController> logger) : base(logger)
    {
        this.roleRepo = roleRepo;
        //this.menuRepo = menuRepo;
        this.roleManager = roleManager;
    }

    /// <summary>
    /// Select 下拉列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("select"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ApiResponse<SelectItemModel<Guid>[]>> GetList()
    {
        var r = await roleRepo.GetSelectAsync();
        return r;
    }

    /// <summary>
    /// 查询后台权限 表格
    /// </summary>
    /// <param name="current">当前页码</param>
    /// <param name="pageSize">分页大小</param>
    /// <param name="name">用户名</param>
    /// <returns></returns>
    [HttpGet, PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ApiResponse<PagedModel<TableItemM>>> Get(
        [FromQuery] int current = IPagedModel.DefaultCurrent,
        [FromQuery] int pageSize = IPagedModel.DefaultPageSize,
        [FromQuery] string? name = null)
    {
        var r = await roleRepo.QueryAsync(name, current, pageSize);
        return r;
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPost, PermissionFilter(ControllerName + nameof(SysButtonType.Add))]
    public async Task<ActionResult<ApiResponse<bool>>> Post([FromBody] AddOrEditM model)
    {
        var user = TryGetUserId(out Guid userId);
        if (!user) return NotFound();
        var role = await roleManager.FindByNameAsync(model.Name, TenantConstants.RootTenantIdG);
        if (role != null) return Fail($"权限名 {role.Name} 已存在");
        role = new()
        {
            TenantId = TenantConstants.RootTenantIdG,
            Name = model.Name!,
            CreateUserId = userId,
        };

        var identityResult = await roleManager.CreateAsync(role);
        if (!identityResult.Succeeded)
            return Fail(identityResult);
        return Ok();
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPut, PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
    public async Task<ActionResult<ApiResponse<bool>>> Put([FromBody] AddOrEditM model)
    {
        var role = await roleManager.FindByIdAsync(model.Id);
        if (role == null || role.TenantId != TenantConstants.RootTenantIdG) 
            return NotFound();

        //role.OperatorUserId = userId;
        role.Name = model.Name!;

        var identityResult = await roleManager.UpdateAsync(role);
        if (!identityResult.Succeeded)
            return Fail(identityResult);
        return Ok();
    }

    /// <summary>
    /// 获取角色菜单列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("menus/{roleId}"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ActionResult<ApiResponse<Guid[]>>> GetRoleMenus([FromRoute] Guid roleId)
    {
        var r = await roleRepo.GetRoleMenus(roleId, TenantConstants.RootTenantIdG);
        return Ok(r);
    }
}