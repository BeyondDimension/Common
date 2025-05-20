using BD.Common8.Columns;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace BD.Common8.AspNetCore.Extensions;

public static partial class HttpContextExtensions
{
    /// <summary>
    /// 获取当前租户 Id
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid? GetTenantId(this HttpContext context)
        => ShortGuid.TryParse(context.User.Claims
            .FirstOrDefault(x => x.Type == nameof(ITenant.TenantId))?.Value, out Guid tenantId)
                ? tenantId : null;
}
