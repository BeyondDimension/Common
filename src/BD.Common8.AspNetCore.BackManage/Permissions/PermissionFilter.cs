using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace BD.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PermissionFilter : Attribute, IAsyncAuthorizationFilter
{
    public PermissionFilter(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, null, new PermissionAuthorizationRequirement(Name));
        if (!authorizationResult.Succeeded)
        {
            context.Result = new ForbidResult();
        }
    }
}
