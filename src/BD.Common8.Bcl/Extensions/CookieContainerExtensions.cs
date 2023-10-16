namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="CookieContainer"/> 类型的扩展函数
/// </summary>
public static partial class CookieContainerExtensions
{
    /// <summary>
    /// 从 Cookie 容器中根据 Uri 与键名获取 Cookie 值
    /// </summary>
    /// <param name="cookieContainer"></param>
    /// <param name="uri"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetCookieValue(this CookieContainer cookieContainer, Uri uri, string name)
    {
        var cookies = cookieContainer.GetCookies(uri);

#if NETFRAMEWORK || NETSTANDARD
        return cookies.Count > 0 ? (from Cookie cookie in cookies where cookie.Name == name select cookie.Value).FirstOrDefault() : null;
#else
        return cookies.Count > 0 ? cookies.FirstOrDefault(cookie => cookie.Name == name)?.Value : null;
#endif
    }
}
