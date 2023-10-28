namespace BD.Common8.AspNetCore.Controllers;

/// <summary>
/// 匿名访问的 API 控制器
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="logger"></param>
[AllowAnonymous]
public abstract class AllowAnonymousApiController<T>(ILogger<T> logger) : ApiControllerBase<T>(logger) where T : AllowAnonymousApiController<T>
{
}