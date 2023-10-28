namespace BD.Common8.AspNetCore.Controllers;

/// <summary>
/// 需要授权的 API 控制器
/// </summary>
/// <typeparam name="T"></typeparam>
[Authorize(AuthenticationSchemes = "Bearer")]
public abstract class AuthorizeApiController<T>(ILogger<T> logger) : ApiAuthorizeControllerBase<T>(logger) where T : AuthorizeApiController<T>
{
}