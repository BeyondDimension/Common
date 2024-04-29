namespace BD.Common8.AspNetCore.Extensions;

/// <summary>
/// 提供对 <see cref="HttpContext"/> 的扩展方法的访问
/// </summary>
public static partial class HttpContextExtensions
{
    /// <summary>
    /// 获取所有外部认证提供程序的身份验证方案
    /// </summary>
    /// <returns>外部认证提供程序的身份验证方案数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
    {
        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (from scheme in await schemes.GetAllSchemesAsync()
                where !string.IsNullOrEmpty(scheme.DisplayName)
                select scheme).ToArray();
    }

    /// <summary>
    /// 判断指定的提供程序是否被支持，如果支持指定的提供程序则返回 <see langword="true"/>；否则为 <see langword="false"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        return (from scheme in await context.GetExternalProvidersAsync()
                where string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase)
                select scheme).Any();
    }
}