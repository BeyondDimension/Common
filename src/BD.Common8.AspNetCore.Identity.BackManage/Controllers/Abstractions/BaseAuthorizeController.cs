namespace BD.Common8.AspNetCore.Controllers.Abstractions;

[Authorize(AuthenticationSchemes = "Bearer")]
public abstract class BaseAuthorizeController<T> : ApiAuthorizeControllerBase<T> where T : ApiAuthorizeControllerBase<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAuthorizeController{T}"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public BaseAuthorizeController(ILogger<T> logger) : base(logger)
    {
    }

    [NonAction]
    protected bool TryGetUserId(out Guid userId)
    {
        userId = UserIsLockedOutFilterAttribute.GetUserId(HttpContext);
        return userId != default;
        ////var claimType = (HttpContext.RequestServices
        ////    .GetService<IOptions<IdentityOptions>>()?.Value ?? new()).ClaimsIdentity.UserIdClaimType;
        ////var userIdS = User.FindFirstValue(claimType);
        //var r = HttpContext.Items.TryGetValue(nameof(Common.Services.IUserManager.FindByIdAsync), out object? thisUserId);
        //userId = (Guid)(thisUserId ?? Guid.Empty);
        //return r;
    }

    protected const string Controllers_RoleManage = "RoleManage";
    protected const string Controllers_SystemUser = "SystemUser";
    protected const string Controllers_SystemMenuManage = "SystemMenuManage";
}