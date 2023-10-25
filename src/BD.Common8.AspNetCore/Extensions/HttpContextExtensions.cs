#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.AspNetCore.Http;

#pragma warning disable SA1600 // Elements should be documented

public static partial class HttpContextExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
    {
        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (from scheme in await schemes.GetAllSchemesAsync()
                where !string.IsNullOrEmpty(scheme.DisplayName)
                select scheme).ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        return (from scheme in await context.GetExternalProvidersAsync()
                where string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase)
                select scheme).Any();
    }
}