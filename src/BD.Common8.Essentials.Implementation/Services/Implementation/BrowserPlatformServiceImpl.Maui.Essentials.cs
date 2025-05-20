#if !LINUX
using BD.Common8.Essentials.Enums;
using BD.Common8.Essentials.Models;

namespace BD.Common8.Essentials.Services.Implementation;

partial class BrowserPlatformServiceImpl
{
    /// <inheritdoc/>
    async Task<bool> IBrowserPlatformService.OpenAsync(string uri)
    {
        var result = await global::Microsoft.Maui.ApplicationModel.Browser.OpenAsync(uri);
        return result;
    }

    /// <inheritdoc/>
    async Task<bool> IBrowserPlatformService.OpenAsync(Uri uri)
    {
        var result = await global::Microsoft.Maui.ApplicationModel.Browser.OpenAsync(uri);
        return result;
    }

    /// <inheritdoc/>
    async Task<bool> IBrowserPlatformService.OpenAsync(string uri, BrowserLaunchMode launchMode)
    {
        var result = await global::Microsoft.Maui.ApplicationModel.Browser.OpenAsync(uri, unchecked((global::Microsoft.Maui.ApplicationModel.BrowserLaunchMode)launchMode));
        return result;
    }

    /// <inheritdoc/>
    async Task<bool> IBrowserPlatformService.OpenAsync(Uri uri, BrowserLaunchMode launchMode)
    {
        var result = await global::Microsoft.Maui.ApplicationModel.Browser.OpenAsync(uri, unchecked((global::Microsoft.Maui.ApplicationModel.BrowserLaunchMode)launchMode));
        return result;
    }

    static global::Microsoft.Maui.Graphics.Color? GetColor(global::System.Drawing.Color? color)
        => color.HasValue ?
            new global::Microsoft.Maui.Graphics.Color(color.Value.R, color.Value.G, color.Value.B, color.Value.A) :
            null;

    static global::Microsoft.Maui.ApplicationModel.BrowserLaunchOptions GetOptions(BrowserLaunchOptions options)
    {
        return new()
        {
            Flags = unchecked((global::Microsoft.Maui.ApplicationModel.BrowserLaunchFlags)options.Flags),
            LaunchMode = unchecked((global::Microsoft.Maui.ApplicationModel.BrowserLaunchMode)options.LaunchMode),
            PreferredControlColor = GetColor(options.PreferredControlColor),
            PreferredToolbarColor = GetColor(options.PreferredToolbarColor),
            TitleMode = unchecked((global::Microsoft.Maui.ApplicationModel.BrowserTitleMode)options.TitleMode),
        };
    }

    /// <inheritdoc/>
    async Task<bool> IBrowserPlatformService.OpenAsync(string uri, BrowserLaunchOptions options)
    {
        var options2 = GetOptions(options);
        var result = await global::Microsoft.Maui.ApplicationModel.Browser.OpenAsync(uri, options2);
        return result;
    }

    /// <inheritdoc/>
    async Task<bool> IBrowserPlatformService.OpenAsync(Uri uri, BrowserLaunchOptions options)
    {
        var options2 = GetOptions(options);
        var result = await global::Microsoft.Maui.ApplicationModel.Browser.OpenAsync(uri, options2);
        return result;
    }
}
#endif