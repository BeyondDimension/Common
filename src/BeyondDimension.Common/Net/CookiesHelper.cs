// ReSharper disable once CheckNamespace
namespace System.Net;

public static class CookiesHelper
{
    public static string? GetCookieValue(this CookieContainer cookieContainer, Uri uri, string name)
    {
        ArgumentNullException.ThrowIfNull(cookieContainer);
        ArgumentNullException.ThrowIfNull(uri);

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        CookieCollection cookies = cookieContainer.GetCookies(uri);

#if NETFRAMEWORK
		return cookies.Count > 0 ? (from Cookie cookie in cookies where cookie.Name == name select cookie.Value).FirstOrDefault() : null;
#else
        return cookies.Count > 0 ? cookies.FirstOrDefault(cookie => cookie.Name == name)?.Value : null;
#endif
    }

    // https://gist.github.com/mt89vein/9c9d7291d42dac0d1598f1a61da3117c

    //static readonly Lazy<Uri> defaultRequestUri = new(() => new Uri("https://steampp.net"));

    /// <summary>
    /// 从 Http 响应的请求头中读取 Cookie 集合
    /// </summary>
    /// <param name="response"></param>
    /// <returns>Список куки.</returns>
    //public static CookieCollection? GetCookies(this HttpResponseMessage response)
    //{
    //    if (!response.Headers.TryGetValues("Set-Cookie", out var cookieEntries))
    //        return null;
    //    var requestUri = response.RequestMessage?.RequestUri ?? defaultRequestUri.Value;
    //    CookieContainer cookieContainer = new();
    //    foreach (var cookieEntry in cookieEntries)
    //    {
    //        cookieContainer.SetCookies(requestUri, cookieEntry);
    //    }
    //    return cookieContainer.GetCookies(requestUri);
    //}

    /// <summary>
    /// 从 Http 请求中设置 Cookie 集合
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cookies"></param>
    //public static void SetCookies(this HttpRequestMessage request, IEnumerable<Cookie> cookies)
    //{
    //    request.Headers.Remove("Cookie");
    //    if (!cookies.Any())
    //        return;
    //    CookieContainer cookieContainer = new();
    //    var requestUri = request.RequestUri ?? defaultRequestUri.Value;
    //    foreach (var cookie in cookies)
    //    {
    //        cookieContainer.Add(requestUri, cookie);
    //    }
    //    var cookieHeader = cookieContainer.GetCookieHeader(requestUri);
    //    request.Headers.Add("Cookie", cookieHeader);
    //}
}
