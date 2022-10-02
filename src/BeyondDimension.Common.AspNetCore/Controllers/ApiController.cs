namespace BD.Common.Controllers;

public static class ApiController
{
    /// <summary>
    /// 匿名访问的 API 控制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [AllowAnonymous]
    public abstract class AllowAnonymous<T> : ApiControllerBase<T> where T : AllowAnonymous<T>
    {
        public AllowAnonymous(ILogger<T> logger) : base(logger)
        {
        }
    }

    /// <summary>
    /// 需要授权的 API 控制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class Authorize<T> : ApiAuthorizeControllerBase<T> where T : Authorize<T>
    {
        public Authorize(ILogger<T> logger) : base(logger)
        {
        }
    }
}