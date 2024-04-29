namespace BD.Common8.AspNetCore.Extensions;

public static partial class FilterContextExtensions
{
    /// <summary>
    /// https://github.com/dotnet/aspnetcore/blob/v5.0.3/src/Mvc/Mvc.Core/src/Authorization/AuthorizeFilter.cs#L221
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllowAnonymous(this FilterContext context)
    {
        var filters = context.Filters;
        for (var i = 0; i < filters.Count; i++)
            if (filters[i] is IAllowAnonymousFilter)
                return true;

        // When doing endpoint routing, MVC does not add AllowAnonymousFilters for AllowAnonymousAttributes that
        // were discovered on controllers and actions. To maintain compat with 2.x,
        // we'll check for the presence of IAllowAnonymous in endpoint metadata.
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            return true;

        return false;
    }
}
