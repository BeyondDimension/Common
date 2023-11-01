namespace BD.Common8.Essentials.Services;

/// <summary>
/// 提供了浏览器平台相关的功能
/// </summary>
public interface IBrowserPlatformService
{
    /// <summary>
    /// 获取 <see cref="IBrowserPlatformService"/>  的实例
    /// </summary>
    static IBrowserPlatformService? Instance => Ioc.Get_Nullable<IBrowserPlatformService>();

    /// <summary>
    /// 打开指定的 URI
    /// </summary>
    ValueTask<bool> OpenAsync(string uri);

    /// <summary>
    /// 打开指定的 URI 对象
    /// </summary>
    ValueTask<bool> OpenAsync(Uri uri);

    /// <summary>
    /// 按指定的浏览器启动模式打开 URI
    /// </summary>
    ValueTask<bool> OpenAsync(string uri, BrowserLaunchMode launchMode);

    /// <summary>
    /// 按指定的浏览器启动模式打开 URI 对象
    /// </summary>
    ValueTask<bool> OpenAsync(Uri uri, BrowserLaunchMode launchMode);

    /// <summary>
    /// 按指定的浏览器选项打开 URI
    /// </summary>
    ValueTask<bool> OpenAsync(string uri, BrowserLaunchOptions options);

    /// <summary>
    /// 按指定的浏览器选项打开 URI 对象
    /// </summary>
    ValueTask<bool> OpenAsync(Uri uri, BrowserLaunchOptions options);

    /// <summary>
    /// 使用进程打开指定的 URL
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    static bool OpenCoreByProcess(string url)
    {
        var r = Process2.OpenCoreByProcess(url,
            static s => ToastHelper.Show(ToastIcon.Error, s));
        return r;
    }
}
