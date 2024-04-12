namespace BD.Common8.AspNetCore.Filters;

public sealed class UserIsLockedOutFilterAttribute : ActionFilterAttribute
{
    const string KEY_UID = nameof(IUserManager.FindByIdAsync);

    static void SetUserId(HttpContext ctx, Guid userId) => ctx.Items.TryAdd(KEY_UID, userId);

    public static Guid GetUserId(HttpContext ctx)
        => (ctx.Items.TryGetValue(KEY_UID, out var value) && value is Guid userId)
        ? userId : default;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!HasAllowAnonymous(context))
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<IUserManager>();
            var user = await userManager.GetUserAsync(context.HttpContext.User);
            if (user == null)
            {
                var rsp = new ApiResponse
                {
                    Messages = ["错误 401，用户为空。"],
                };
                context.Result = new ObjectResult(rsp);
                return;
            }
            var isLockedOut = await userManager.IsLockedOutAsync(user);
            if (isLockedOut)
            {
                var rsp = new ApiResponse
                {
                    Messages = ["错误 401，用户被锁定。"],
                };
                context.Result = new ObjectResult(rsp);
                return;
            }
            SetUserId(context.HttpContext, user.Id);
        }

        await base.OnActionExecutionAsync(context, next);
    }

    /// <summary>
    /// https://github.com/dotnet/aspnetcore/blob/v7.0.0-rc.1.22427.2/src/Mvc/Mvc.Core/src/Authorization/AuthorizeFilter.cs#L218
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    static bool HasAllowAnonymous(FilterContext context)
    {
        var filters = context.Filters;
        for (var i = 0; i < filters.Count; i++)
        {
            if (filters[i] is IAllowAnonymousFilter)
            {
                return true;
            }
        }

        // When doing endpoint routing, MVC does not add AllowAnonymousFilters for AllowAnonymousAttributes that
        // were discovered on controllers and actions. To maintain compat with 2.x,
        // we'll check for the presence of IAllowAnonymous in endpoint metadata.
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            return true;
        }

        return false;
    }
}
