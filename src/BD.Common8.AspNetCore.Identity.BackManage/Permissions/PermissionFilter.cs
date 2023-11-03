namespace BD.Common8.AspNetCore.Permissions;

/// <summary>
/// 权限过滤器
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PermissionFilter(string name) : Attribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// 权限名称
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 实现授权验证和拦截功能
    /// </summary>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, null, new PermissionAuthorizationRequirement(Name));
        if (!authorizationResult.Succeeded)
            context.Result = new ForbidResult();
    }
}
