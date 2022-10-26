// ReSharper disable once CheckNamespace
namespace BD.Common;

public static class HttpContextExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid? GetTenantId(this HttpContext context)
        => ShortGuid.TryParse(context.User.Claims
            .FirstOrDefault(x => x.Type == nameof(ITenant.TenantId))?.Value, out Guid tenantId)
                ? tenantId : null;
}
