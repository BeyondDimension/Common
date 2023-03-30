// ReSharper disable once CheckNamespace
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.Mvc;

public static class HttpRequestExtensions
{
    public const string CUSTOM_URL_SCHEME_NAME = "spp";

    public const string CUSTOM_URL_SCHEME = $"{CUSTOM_URL_SCHEME_NAME}://";

    /// <summary>
    /// 获取当前使用的客户端 <see cref="ClientOSPlatform"/>，早期版本会返回 default
    /// </summary>
    /// <param name="request"></param>
    /// <param name="customUrlScheme"></param>
    /// <returns></returns>
    public static ClientOSPlatform GetOSPlatform(this HttpRequest request, string customUrlScheme = CUSTOM_URL_SCHEME)
    {
        const string key = nameof(ClientOSPlatform);
        if (request.HttpContext.Items.ContainsKey(key) &&
            request.HttpContext.Items[key] is ClientOSPlatform value)
            return value;
        value = request.GetOSPlatformCore(customUrlScheme);
        request.HttpContext.Items[key] = value;
        return value;
    }

    static ClientOSPlatform GetOSPlatformCore(this HttpRequest request, string customUrlScheme = CUSTOM_URL_SCHEME)
    {
        if (!request.Headers.Referer.IsNullOrEmpty())
        {
            string referrer = request.Headers.Referer!;
            if (referrer.StartsWith(customUrlScheme))
            {
                try
                {
                    var uri = new Uri(referrer);
                    return ClientOSPlatformEnumExtensions.Parse(uri.Host);
                }
                catch
                {

                }
            }
        }
        var userAgent = request.Headers.UserAgent;
        if (!userAgent.IsNullOrEmpty())
        {
            if (userAgent.Contains("Android"))
                return ClientOSPlatform.AndroidPhone;
            if (userAgent.Contains("Windows NT"))
                return ClientOSPlatform.Windows;
            if (userAgent.Contains("Intel Mac OS X"))
                return ClientOSPlatform.macOS;
            if (userAgent.Contains("iPad"))
                return ClientOSPlatform.iPadOS;
            if (userAgent.Contains("iPhone"))
                return ClientOSPlatform.iOS;
            if (userAgent.Contains("Linux"))
                return ClientOSPlatform.Linux;
        }
        return default;
    }

    /// <inheritdoc cref="GetOSPlatform(HttpRequest,string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ClientOSPlatform GetOSPlatform(this HttpContext context, string customUrlScheme = CUSTOM_URL_SCHEME)
        => context.Request.GetOSPlatform(customUrlScheme);

    /// <inheritdoc cref="GetOSPlatform(HttpRequest,string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ClientOSPlatform GetOSPlatform(this ControllerBase controller, string customUrlScheme = CUSTOM_URL_SCHEME)
        => controller.Request.GetOSPlatform(customUrlScheme);

    /// <inheritdoc cref="GetOSPlatform(HttpRequest,string)"/>
    [Obsolete("use GetOSPlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ClientOSPlatform GetOSName(this HttpRequest request) => GetOSPlatform(request);

    /// <inheritdoc cref="GetOSPlatform(HttpRequest,string)"/>
    [Obsolete("use GetOSPlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ClientOSPlatform GetOSName(this HttpContext context) => GetOSPlatform(context.Request);

    /// <inheritdoc cref="GetOSPlatform(HttpRequest,string)"/>
    [Obsolete("use GetOSPlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ClientOSPlatform GetOSName(this ControllerBase controller) => GetOSPlatform(controller.Request);
}
