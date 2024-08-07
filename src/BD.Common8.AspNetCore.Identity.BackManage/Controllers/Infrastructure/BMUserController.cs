using EditM = BD.Common8.AspNetCore.Models.Users.EditCurrentBMUserInfoModel;

namespace BD.Common8.AspNetCore.Controllers.Infrastructure;

/// <summary>
/// 当前登录的后台用户管理个人资料
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BMUserController"/> class.
/// </remarks>
/// <param name="menuRepo"></param>
/// <param name="userManager"></param>
/// <param name="logger"></param>
[Route("bm/user")]
public sealed class BMUserController(IBMMenuRepository menuRepo,
    IUserManager userManager,
    ILogger<BMUserController> logger) : BaseAuthorizeController<BMUserController>(logger)
{
    readonly IUserManager userManager = userManager;
    readonly IBMMenuRepository menuRepo = menuRepo;

    /// <summary>
    /// 获取当前登录的个人资料
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CurrentBMUserInfoModel>>> Get()
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null) return Unauthorized();

        var menus_r = await menuRepo.GetUserMenuAsync(user.Id, TenantConstants.RootTenantIdG);

        return Ok(new CurrentBMUserInfoModel
        {
            UserName = user.UserName,
            Avatar = "/img/default-avatar.png",
            TenantId = user.TenantId,
            Menus = menus_r,
        });
    }

    /// <summary>
    /// 编辑当前登录的个人资料
    /// </summary>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> Put([FromBody] EditM model)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null) return Unauthorized();

        if (user.UserName != model.UserName)
        {
            var identityResult = await userManager.SetUserNameAsync(user, model.UserName);
            if (!identityResult.Succeeded)
                return Fail(identityResult);
        }

        return Ok();
    }

    /// <summary>
    /// 获取角色菜单列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("menus")]
    public async Task<ActionResult<ApiResponse<Guid[]>>> GetRoleMenus()
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null) return Unauthorized();
        var r = await menuRepo.GetRoleMenus(user.Id, TenantConstants.RootTenantIdG);
        return Ok(r);
    }

    /// <summary>
    /// 使用旧密码修改密码
    /// </summary>
    /// <param name="model">模型</param>
    /// <returns></returns>
    [HttpPut("cpwd")]
    public async Task<ActionResult<ApiResponse>> Put([FromBody] ChangePasswordRequestModel model)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null) return Unauthorized();

        var identityResult = await userManager.ChangePasswordAsync(user,
            model.OldPassword, model.NewPassword);
        if (!identityResult.Succeeded)
            return Fail(identityResult);

        return Ok();
    }
}