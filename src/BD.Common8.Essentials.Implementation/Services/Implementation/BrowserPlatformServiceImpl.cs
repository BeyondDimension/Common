#if ANDROID
using Android.Content;
using AndroidX.Browser.CustomTabs;
using AndroidUri = Android.Net.Uri;
using Application = Android.App.Application;
using Context = Android.Content.Context;
#elif IOS || MACCATALYST
using SafariServices;
#endif

namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// <para>https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Browser/Browser.shared.cs</para>
/// <para>https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Browser/Browser.android.cs</para>
/// <para>https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Browser/Browser.ios.cs</para>
/// </summary>
sealed class BrowserPlatformServiceImpl : IBrowserPlatformService
{
#if ANDROID
    static ValueTask<bool> OpenAsync(string uri, BrowserLaunchOptions? options)
    {
        var launchMode = options == null ? BrowserLaunchOptions.DefaultLaunchMode : options.LaunchMode;
        switch (launchMode)
        {
            case BrowserLaunchMode.SystemPreferred:
                LaunchChromeTabs(options, AndroidUri.Parse(uri));
                break;
            case BrowserLaunchMode.External:
                LaunchExternalBrowser(options, AndroidUri.Parse(uri));
                break;
        }
        return new(true);
    }

    static void LaunchChromeTabs(BrowserLaunchOptions? options, AndroidUri? nativeUri)
    {
        var tabsBuilder = new CustomTabsIntent.Builder();
        tabsBuilder.SetShowTitle(true);
#pragma warning disable CS0618 // Type or member is obsolete
        if (options?.PreferredToolbarColor != null)
            tabsBuilder.SetToolbarColor(options.PreferredToolbarColor.Value.ToArgb());
#pragma warning restore CS0618 // Type or member is obsolete
        if (options != null && options.TitleMode != BrowserTitleMode.Default)
            tabsBuilder.SetShowTitle(options.TitleMode == BrowserTitleMode.Show);

        var tabsIntent = tabsBuilder.Build();
        ActivityFlags? tabsFlags = null;

        Context? context = ActivityStateManager.GetCurrentActivity(false);

        if (context == null)
        {
            context = Application.Context;

            // If using ApplicationContext we need to set ClearTop/NewTask (See #225)
            tabsFlags = ActivityFlags.ClearTop | ActivityFlags.NewTask;
        }

#if __ANDROID_24__
        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            if (options != null && options.Flags.HasFlag(BrowserLaunchFlags.LaunchAdjacent))
            {
                if (tabsFlags.HasValue)
                    tabsFlags |= ActivityFlags.LaunchAdjacent | ActivityFlags.NewTask;
                else
                    tabsFlags = ActivityFlags.LaunchAdjacent | ActivityFlags.NewTask;
            }
        }
#endif

        // Check if there's flags specified to use
        if (tabsFlags.HasValue)
            tabsIntent.Intent.SetFlags(tabsFlags.Value);

        if (nativeUri != null)
            tabsIntent.LaunchUrl(context, nativeUri);
    }

    static void LaunchExternalBrowser(BrowserLaunchOptions? options, AndroidUri? nativeUri)
    {
        var intent = new Intent(Intent.ActionView, nativeUri);
        var flags = ActivityFlags.ClearTop | ActivityFlags.NewTask;

#if __ANDROID_24__
        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            if (options != null && options.Flags.HasFlag(BrowserLaunchFlags.LaunchAdjacent))
                flags |= ActivityFlags.LaunchAdjacent;
        }
#endif
        intent.SetFlags(flags);

        if (!PlatformUtils.IsIntentSupported(intent))
        {
            LaunchChromeTabs(options, nativeUri);
            return;
        }

        Application.Context.StartActivity(intent);
    }
#elif IOS || MACCATALYST
    static async ValueTask<bool> OpenAsync(string uri, BrowserLaunchOptions? options)
    {
        var launchMode = options == null ? BrowserLaunchOptions.DefaultLaunchMode : options.LaunchMode;
        switch (launchMode)
        {
            case BrowserLaunchMode.SystemPreferred:
                await LaunchSafariViewController(uri, options);
                break;
            case BrowserLaunchMode.External:
                return await Launcher.OpenAsync(uri);
        }

        return true;
    }

    static async Task LaunchSafariViewController(string uri, BrowserLaunchOptions? options)
    {
        var nativeUrl = new NSUrl(uri);
#pragma warning disable CA1422 // Validate platform compatibility
        var sfViewController = new SFSafariViewController(nativeUrl, false);
#pragma warning restore CA1422 // Validate platform compatibility
        var vc = WindowStateManager.GetCurrentUIViewController(true)!;

        if (options?.PreferredToolbarColor != null)
            sfViewController.PreferredBarTintColor = options.PreferredToolbarColor.AsUIColor();

        if (options?.PreferredControlColor != null)
            sfViewController.PreferredControlTintColor = options.PreferredControlColor.AsUIColor();

        if (sfViewController.PopoverPresentationController != null)
            sfViewController.PopoverPresentationController.SourceView = vc.View!;

        if (options != null)
        {
            if (options.Flags.HasFlag(BrowserLaunchFlags.PresentAsFormSheet))
                sfViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            else if (options.Flags.HasFlag(BrowserLaunchFlags.PresentAsPageSheet))
                sfViewController.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
        }

        await vc.PresentViewControllerAsync(sfViewController, true);
    }
#elif MACOS
    static ValueTask<bool> OpenAsync(string uri, BrowserLaunchOptions? options)
    {
        NSUrl url = new(uri);
        var r = NSWorkspace.SharedWorkspace.OpenUrl(url);
        return new(r);
    }
#else
    static ValueTask<bool> OpenAsync(string uri, BrowserLaunchOptions? options)
    {
        var r = IBrowserPlatformService.OpenCoreByProcess(uri);
        return new(r);
    }
#endif

    ValueTask<bool> IBrowserPlatformService.OpenAsync(string uri)
    {
        return OpenAsync(uri, null);
    }

    ValueTask<bool> IBrowserPlatformService.OpenAsync(Uri uri)
    {
        return OpenAsync(uri.AbsoluteUri, null);
    }

    ValueTask<bool> IBrowserPlatformService.OpenAsync(string uri, BrowserLaunchMode launchMode)
    {
        return OpenAsync(uri, new() { LaunchMode = launchMode, });
    }

    ValueTask<bool> IBrowserPlatformService.OpenAsync(Uri uri, BrowserLaunchMode launchMode)
    {
        return OpenAsync(uri.AbsoluteUri, new() { LaunchMode = launchMode, });
    }

    ValueTask<bool> IBrowserPlatformService.OpenAsync(string uri, BrowserLaunchOptions options)
    {
        return OpenAsync(uri, null);
    }

    ValueTask<bool> IBrowserPlatformService.OpenAsync(Uri uri, BrowserLaunchOptions options)
    {
        return OpenAsync(uri.AbsoluteUri, null);
    }
}