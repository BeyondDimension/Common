namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供了一种在应用程序中显示网页的方法。
/// </summary>
public static partial class Browser2
{
    /// <summary>
    /// 当出现异常时触发的事件
    /// </summary>
    public static event Action<Exception>? OnError;

    /// <summary>
    /// 指示是否仅使用 HTTPS 连接
    /// </summary>
    public static bool HttpsOnly { get; set; }

    /// <summary>
    /// 默认的浏览器启动模式
    /// </summary>
    const BrowserLaunchMode DefaultBrowserLaunchMode = BrowserLaunchMode.SystemPreferred;

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    [Obsolete("use OpenAsync", true)]
    public static bool Open(string? url, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode) => OpenAnalysis(url) switch
    {
        OpenResultCode.HttpUrl => OpenCore(url!, launchMode),
        OpenResultCode.StartedByProcess2 => true,
        _ => false,
    };

    /// <summary>
    /// 兼容 Windows/Linux/macOS/.Net Core/Android/iOS 的打开链接方法
    /// </summary>
    /// <param name="url"></param>
    /// <param name="launchMode"></param>
    /// <returns></returns>
    public static ValueTask<bool> OpenAsync(string? url, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode) => OpenAnalysis(url) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(url!, launchMode),
        OpenResultCode.StartedByProcess2 => new(true),
        _ => new(false),
    };

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    public static ValueTask<bool> OpenAsync(string? url, BrowserLaunchOptions options) => OpenAnalysis(url) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(url!, options),
        OpenResultCode.StartedByProcess2 => new(true),
        _ => new(false),
    };

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    public static ValueTask<bool> OpenAsync(Uri uri, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode) => OpenAnalysis(uri.ToString()) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(uri, launchMode),
        OpenResultCode.StartedByProcess2 => new(true),
        _ => new(false),
    };

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    public static ValueTask<bool> OpenAsync(Uri uri, BrowserLaunchOptions options) => OpenAnalysis(uri.ToString()) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(uri, options),
        OpenResultCode.StartedByProcess2 => new(true),
        _ => new(false),
    };
}