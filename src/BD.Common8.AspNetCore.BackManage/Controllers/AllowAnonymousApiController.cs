namespace BD.Common8.AspNetCore.Controllers;

/// <summary>
/// 匿名访问的 API 控制器
/// </summary>
/// <typeparam name="T"></typeparam>
[AllowAnonymous]
public abstract class AllowAnonymousApiController<T> : ApiControllerBase<T> where T : AllowAnonymousApiController<T>
{
    public AllowAnonymousApiController(ILogger<T> logger) : base(logger)
    {
    }
}