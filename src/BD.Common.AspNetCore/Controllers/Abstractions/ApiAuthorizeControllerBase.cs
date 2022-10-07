namespace BD.Common.Controllers.Abstractions;

/// <summary>
/// 需要授权的 API 控制器基类
/// </summary>
/// <typeparam name="T"></typeparam>
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
public abstract class ApiAuthorizeControllerBase<T> : ApiControllerBase<T> where T : ApiAuthorizeControllerBase<T>
{
    public ApiAuthorizeControllerBase(ILogger<T> logger) : base(logger)
    {

    }
}