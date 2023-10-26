#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.AspNetCore.Http;

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
