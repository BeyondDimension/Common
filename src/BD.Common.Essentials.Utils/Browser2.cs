namespace BD.Common;

/// <summary>
/// 提供了一种在应用程序中显示网页的方法。
/// </summary>
public static partial class Browser2
{
    public static event Action<Exception>? OnError;

    public static bool HttpsOnly { get; set; }

    const BrowserLaunchMode DefaultBrowserLaunchMode = BrowserLaunchMode.SystemPreferred;

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    public static bool Open(string? url, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode) => OpenAnalysis(url) switch
    {
        OpenResultCode.HttpUrl => OpenCore(url, launchMode),
        OpenResultCode.StartedByProcess2 => true,
        _ => false,
    };

    /// <summary>
    /// 兼容 Windows/Linux/macOS/.Net Core/Android/iOS 的打开链接方法
    /// </summary>
    /// <param name="url"></param>
    /// <param name="launchMode"></param>
    /// <returns></returns>
    public static Task<bool> OpenAsync(string? url, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode) => OpenAnalysis(url) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(url, launchMode),
        OpenResultCode.StartedByProcess2 => Task.FromResult(true),
        _ => Task.FromResult(false),
    };

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    public static Task<bool> OpenAsync(string? url, BrowserLaunchOptions options) => OpenAnalysis(url) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(url, options),
        OpenResultCode.StartedByProcess2 => Task.FromResult(true),
        _ => Task.FromResult(false),
    };

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    public static Task<bool> OpenAsync(Uri uri, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode) => OpenAnalysis(uri.ToString()) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(uri, launchMode),
        OpenResultCode.StartedByProcess2 => Task.FromResult(true),
        _ => Task.FromResult(false),
    };

    /// <inheritdoc cref="OpenAsync(string?, BrowserLaunchMode)"/>
    public static Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options) => OpenAnalysis(uri.ToString()) switch
    {
        OpenResultCode.HttpUrl => OpenCoreAsync(uri, options),
        OpenResultCode.StartedByProcess2 => Task.FromResult(true),
        _ => Task.FromResult(false),
    };
}