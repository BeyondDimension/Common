namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IBrowserPlatformService
{
    static IBrowserPlatformService? Instance => Ioc.Get_Nullable<IBrowserPlatformService>();

    ValueTask<bool> OpenAsync(string uri);

    ValueTask<bool> OpenAsync(Uri uri);

    ValueTask<bool> OpenAsync(string uri, BrowserLaunchMode launchMode);

    ValueTask<bool> OpenAsync(Uri uri, BrowserLaunchMode launchMode);

    ValueTask<bool> OpenAsync(string uri, BrowserLaunchOptions options);

    ValueTask<bool> OpenAsync(Uri uri, BrowserLaunchOptions options);

    static bool OpenCoreByProcess(string url)
    {
        var r = Process2.OpenCoreByProcess(url,
            static s => ToastHelper.Show(ToastIcon.Error, s));
        return r;
    }
}
