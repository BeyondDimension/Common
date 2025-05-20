#if LINUX
using BD.Common8.Essentials.Enums;
using BD.Common8.Essentials.Models;

namespace BD.Common8.Essentials.Services.Implementation;

partial class BrowserPlatformServiceImpl
{
    /// <inheritdoc/>
    Task<bool> IBrowserPlatformService.OpenAsync(string uri)
    {
        var result = IBrowserPlatformService.OpenCoreByProcess(uri);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    Task<bool> IBrowserPlatformService.OpenAsync(Uri uri)
    {
        var result = IBrowserPlatformService.OpenCoreByProcess(uri.AbsoluteUri);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    Task<bool> IBrowserPlatformService.OpenAsync(string uri, BrowserLaunchMode launchMode)
    {
        var result = IBrowserPlatformService.OpenCoreByProcess(uri);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    Task<bool> IBrowserPlatformService.OpenAsync(Uri uri, BrowserLaunchMode launchMode)
    {
        var result = IBrowserPlatformService.OpenCoreByProcess(uri.AbsoluteUri);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    Task<bool> IBrowserPlatformService.OpenAsync(string uri, BrowserLaunchOptions options)
    {
        var result = IBrowserPlatformService.OpenCoreByProcess(uri);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    Task<bool> IBrowserPlatformService.OpenAsync(Uri uri, BrowserLaunchOptions options)
    {
        var result = IBrowserPlatformService.OpenCoreByProcess(uri.AbsoluteUri);
        return Task.FromResult(result);
    }
}
#endif