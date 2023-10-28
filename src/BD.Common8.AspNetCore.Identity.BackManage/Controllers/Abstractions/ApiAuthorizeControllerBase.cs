namespace BD.Common8.AspNetCore.Controllers.Abstractions;

/// <summary>
/// 需要授权的 API 控制器基类
/// </summary>
/// <typeparam name="T"></typeparam>
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
public abstract class ApiAuthorizeControllerBase<T>(ILogger<T> logger) : ApiControllerBase<T>(logger) where T : ApiAuthorizeControllerBase<T>
{
}