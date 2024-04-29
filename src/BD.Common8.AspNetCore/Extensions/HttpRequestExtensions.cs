namespace BD.Common8.AspNetCore.Extensions;

public static partial class HttpRequestExtensions
{
    /// <summary>
    /// 自定义 URL 方案名称
    /// </summary>
    public const string CUSTOM_URL_SCHEME_NAME = "spp";

    /// <summary>
    /// 自定义 URL 方案
    /// </summary>
    public const string CUSTOM_URL_SCHEME = $"{CUSTOM_URL_SCHEME_NAME}://";

    /// <summary>
    /// 获取当前使用的客户端 <see cref="DevicePlatform2"/>，早期版本会返回 default
    /// </summary>
    /// <param name="request"></param>
    /// <param name="customUrlScheme"></param>
    /// <returns></returns>
    public static DevicePlatform2 GetDevicePlatform(this HttpRequest request, string customUrlScheme = CUSTOM_URL_SCHEME)
    {
        const string key = "ClientOSPlatform";
        if (request.HttpContext.Items.ContainsKey(key) &&
            request.HttpContext.Items[key] is DevicePlatform2 value)
            return value;
        value = request.GetDevicePlatformCore(customUrlScheme);
        request.HttpContext.Items[key] = value;
        return value;
    }

    static DevicePlatform2 GetDevicePlatformCore(
        this HttpRequest request,
        string customUrlScheme = CUSTOM_URL_SCHEME)
    {
        if (!request.Headers.Referer.IsNullOrEmpty())
        {
            string referrer = request.Headers.Referer!;
            if (referrer.StartsWith(customUrlScheme))
                try
                {
                    var uri = new Uri(referrer);
                    return DevicePlatform2EnumExtensions.Parse(uri.Host);
                }
                catch
                {
                }
        }
        var userAgent = request.Headers.UserAgent;
        if (!userAgent.IsNullOrEmpty())
        {
            if (userAgent.Contains("Android"))
                return DevicePlatform2.AndroidPhone;
            if (userAgent.Contains("Windows NT"))
                return DevicePlatform2.Windows;
            if (userAgent.Contains("Intel Mac OS X"))
                return DevicePlatform2.macOS;
            if (userAgent.Contains("iPad"))
                return DevicePlatform2.iPadOS;
            if (userAgent.Contains("iPhone"))
                return DevicePlatform2.iOS;
            if (userAgent.Contains("Linux"))
                return DevicePlatform2.Linux;
        }
        return default;
    }

    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetDevicePlatform(this HttpContext context, string customUrlScheme = CUSTOM_URL_SCHEME)
        => context.Request.GetDevicePlatform(customUrlScheme);

    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetDevicePlatform(this ControllerBase controller, string customUrlScheme = CUSTOM_URL_SCHEME)
        => controller.Request.GetDevicePlatform(customUrlScheme);

#if DEBUG
    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [Obsolete("use GetDevicePlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetOSPlatform(this HttpContext context, string customUrlScheme = CUSTOM_URL_SCHEME)
        => context.Request.GetOSPlatform(customUrlScheme);

    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [Obsolete("use GetDevicePlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetOSPlatform(this ControllerBase controller, string customUrlScheme = CUSTOM_URL_SCHEME)
        => controller.Request.GetOSPlatform(customUrlScheme);

    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [Obsolete("use GetDevicePlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetOSPlatform(this HttpRequest request, string customUrlScheme = CUSTOM_URL_SCHEME)
        => request.GetDevicePlatform(customUrlScheme);

    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [Obsolete("use GetDevicePlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetOSName(this HttpRequest request) => GetOSPlatform(request);

    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [Obsolete("use GetDevicePlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetOSName(this HttpContext context) => GetOSPlatform(context.Request);

    /// <inheritdoc cref="GetDevicePlatform(HttpRequest, string)"/>
    [Obsolete("use GetDevicePlatform", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DevicePlatform2 GetOSName(this ControllerBase controller) => GetOSPlatform(controller.Request);
#endif
}
