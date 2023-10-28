namespace BD.Common8.AspNetCore.Permissions;

#pragma warning disable SA1600 // Elements should be documented

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PermissionFilter(string name) : Attribute, IAsyncAuthorizationFilter
{
    public string Name { get; set; } = name;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, null, new PermissionAuthorizationRequirement(Name));
        if (!authorizationResult.Succeeded)
            context.Result = new ForbidResult();
    }
}
