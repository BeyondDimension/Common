namespace BD.Common8.AspNetCore.Controllers.Abstractions;

/// <summary>
/// 需要管理员授权的 API 控制器
/// </summary>
/// <typeparam name="T"></typeparam>
[Authorize(AuthenticationSchemes = "Bearer", Roles = RoleEnumHelper.Administrator)]
public abstract class AdminAuthorizeController<T>(ILogger<T> logger) : ApiAuthorizeControllerBase<T>(logger) where T : AdminAuthorizeController<T>
{
}
