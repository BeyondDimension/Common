using AddM = BD.Common8.AspNetCore.Models.AddBMUserModel;
using EditM = BD.Common8.AspNetCore.Models.EditBMUserModel;
using TableItemM = BD.Common8.AspNetCore.Models.SysUserTableItem;

namespace BD.Common8.AspNetCore.Controllers;

/// <summary>
/// 后台管理 - 用户管理，分页查询表格，新增，编辑，删除
/// </summary>
[Route("bm/users")]
public sealed class BMUsersController : BaseAuthorizeController<BMUsersController>
{
    readonly ISysUserRepository userRepo;
    readonly IUserManager userManager;
    const string ControllerName = Controllers_SystemUser;

    public BMUsersController(ISysUserRepository userRepo,
        IUserManager userManager,
        ILogger<BMUsersController> logger) : base(logger)
    {
        this.userRepo = userRepo;
        this.userManager = userManager;
    }

    /// <summary>
    /// 分页查询后台用户
    /// </summary>
    /// <param name="current">当前页码</param>
    /// <param name="pageSize">分页大小</param>
    /// <param name="userName">用户名</param>
    /// <returns></returns>
    [HttpGet, PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ApiResponse<PagedModel<TableItemM>>> Get(
        [FromQuery] int current = IPagedModel.DefaultCurrent,
        [FromQuery] int pageSize = IPagedModel.DefaultPageSize,
        [FromQuery] string? userName = null)
    {
        var r = await userRepo.QueryAsync(userName, current, pageSize);
        return r;
    }

    /// <summary>
    /// 新增后台用户
    /// </summary>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPost, PermissionFilter(ControllerName + nameof(SysButtonType.Add))]
    public async Task<ActionResult<ApiResponse>> Post([FromBody] AddM model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName))
            return Fail("请输入用户名");
        if (string.IsNullOrWhiteSpace(model.Password1))
            return Fail("请输入密码");
        if (model.Password1 != model.Password2)
            return Fail("两次输入的密码不一致");

        var user = await userManager.FindByNameAsync($"{model.UserName}");
        if (user != null) return Fail($"用户名 {model.UserName} 已存在");

        user = new()
        {
            UserName = model.UserName,
            TenantId = TenantConstants.RootTenantIdG,
        };
        var createResult = await userManager.CreateAsync(user, model.Password1);
        if (!createResult.Succeeded)
            return Fail(createResult);

        var hasRoles = model.Roles != null && model.Roles.Any();
        if (hasRoles)
        {
            var identityResult = await userManager.AddToRolesAsync(user, model.Roles!);
            if (!identityResult.Succeeded)
                return Fail(identityResult);
        }

        return Ok();
    }

    /// <summary>
    /// 编辑后台用户
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPut("{id}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
    public async Task<ActionResult<ApiResponse>> Put([FromRoute] Guid id, [FromBody] EditM model)
    {
        if (!userManager.TryGetUserId(HttpContext.User, out var userId))
            return Unauthorized();
        if (userId == id)
            return Fail("不能编辑自己，请在个人中心修改用户名或密码");

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var userName = await userManager.GetUserNameAsync(user);
        if (model.UserName != userName)
        {
            var identityResult = await userManager.SetUserNameAsync(user, model.UserName);
            if (!identityResult.Succeeded)
                return Fail(identityResult);
        }

        var roles = await userManager.GetRolesAsync(user);
        var hasRoles = roles != null && roles.Any();
        var hasEditRoles = model.Roles != null && model.Roles.Any();

        if (hasRoles && hasEditRoles && roles!.SequenceEqual(model.Roles!))
        {
            // 权限数据一致，不更改
        }
        else if (!hasRoles && !hasEditRoles)
        {
            // 无权限变更
        }
        else
        {
            if (hasRoles)
            {
                // 删除现有的权限
                var identityResult = await userManager.RemoveFromRolesAsync(user, roles!);
                if (!identityResult.Succeeded) return Fail(identityResult);
            }

            if (hasEditRoles)
            {
                // 添加新增的权限
                var identityResult = await userManager.AddToRolesAsync(user, model.Roles!);
                if (!identityResult.Succeeded)
                    return Fail(identityResult);
            }
        }

        return Ok();
    }
}