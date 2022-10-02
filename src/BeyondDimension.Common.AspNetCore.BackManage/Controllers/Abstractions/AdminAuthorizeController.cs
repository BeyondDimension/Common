namespace BD.Common.Controllers.Abstractions;

/// <summary>
/// 需要管理员授权的 API 控制器
/// </summary>
/// <typeparam name="T"></typeparam>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(BMRole.Administrator))]
public abstract class AdminAuthorizeController<T> : ApiAuthorizeControllerBase<T> where T : AdminAuthorizeController<T>
{
    public AdminAuthorizeController(ILogger<T> logger) : base(logger)
    {

    }
}
