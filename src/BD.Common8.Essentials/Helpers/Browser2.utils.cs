namespace BD.Common8.Essentials.Helpers;

partial class Browser2
{
    enum OpenResultCode : sbyte
    {
        /// <summary>
        /// 值为 Http URL
        /// </summary>
        HttpUrl = 0,

        /// <summary>
        /// 已由 Process2 启动，在分析时已执行，不需要后续处理
        /// </summary>
        StartedByProcess2 = 1,

        /// <summary>
        /// 出现异常，已由 <see cref="OnError"/> 或 <see cref="Toast"/> 处理
        /// </summary>
        Exception = -1,

        /// <summary>
        /// 值格式不正确或未知
        /// </summary>
        Unknown = -2,
    }

    const string TAG = nameof(Browser2);

    static void HandlerException(Exception e)
    {
        if (OnError == null)
        {
            try
            {
                e.LogAndShow(TAG);
            }
            catch
            {
            }
        }
        else
        {
            OnError(e);
        }
    }

    static OpenResultCode OpenAnalysis(string? url)
    {
        if (String2.IsStoreUrl(url) ||
            String2.IsEmailUrl(url) ||
            String2.IsFileUrl(url))
        {
            var r = IBrowserPlatformService.OpenCoreByProcess(url);
            return r ? OpenResultCode.StartedByProcess2 : OpenResultCode.Exception;
        }
        else if (String2.IsHttpUrl(url, HttpsOnly))
        {
            return OpenResultCode.HttpUrl;
        }
        return OpenResultCode.Unknown;
    }

    static async ValueTask<bool> OpenCoreAsync(Uri uri, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode)
    {
        try
        {
            var browserPlatformService = IBrowserPlatformService.Instance;
            if (browserPlatformService == null)
            {
                return IBrowserPlatformService.OpenCoreByProcess(uri.AbsoluteUri);
            }
            else
            {
                return await browserPlatformService.OpenAsync(uri, launchMode);
            }
        }
        catch (Exception e)
        {
            HandlerException(e);
            return false;
        }
    }

    static async ValueTask<bool> OpenCoreAsync(string uri, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode)
    {
        try
        {
            var browserPlatformService = IBrowserPlatformService.Instance;
            if (browserPlatformService == null)
            {
                return IBrowserPlatformService.OpenCoreByProcess(uri);
            }
            else
            {
                return await browserPlatformService.OpenAsync(uri, launchMode);
            }
        }
        catch (Exception e)
        {
            HandlerException(e);
            return false;
        }
    }

    static async ValueTask<bool> OpenCoreAsync(Uri uri, BrowserLaunchOptions options)
    {
        try
        {
            var browserPlatformService = IBrowserPlatformService.Instance;
            if (browserPlatformService == null)
            {
                return IBrowserPlatformService.OpenCoreByProcess(uri.AbsoluteUri);
            }
            else
            {
                return await browserPlatformService.OpenAsync(uri, options);
            }
        }
        catch (Exception e)
        {
            HandlerException(e);
            return false;
        }
    }

    static async ValueTask<bool> OpenCoreAsync(string uri, BrowserLaunchOptions options)
    {
        try
        {
            var browserPlatformService = IBrowserPlatformService.Instance;
            if (browserPlatformService == null)
            {
                return IBrowserPlatformService.OpenCoreByProcess(uri);
            }
            else
            {
                return await browserPlatformService.OpenAsync(uri, options);
            }
        }
        catch (Exception e)
        {
            HandlerException(e);
            return false;
        }
    }

    static bool OpenCore(string uri, BrowserLaunchMode launchMode = DefaultBrowserLaunchMode)
    {
        OpenCoreSync(uri, launchMode);
        return true;

        static async void OpenCoreSync(string uri, BrowserLaunchMode launchMode) => await OpenCoreAsync(uri, launchMode);
    }
}