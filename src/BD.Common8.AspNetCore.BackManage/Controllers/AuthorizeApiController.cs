namespace BD.Common8.AspNetCore.Controllers;

/// <summary>
/// 需要授权的 API 控制器
/// </summary>
/// <typeparam name="T"></typeparam>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public abstract class AuthorizeApiController<T> : ApiAuthorizeControllerBase<T> where T : AuthorizeApiController<T>
{
    public AuthorizeApiController(ILogger<T> logger) : base(logger)
    {
    }
}