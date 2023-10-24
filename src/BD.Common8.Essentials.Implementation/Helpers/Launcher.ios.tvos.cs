#if ANDROID
using Android.Content;
using AndroidUri = Android.Net.Uri;
using Application = Android.App.Application;
#endif

namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// The Launcher API enables an application to open a URI by the system.
/// This is often used when deep linking into another application's custom URI schemes.
/// </summary>
/// <remarks>
/// <para>If you are looking to open the browser to a website then you should refer to the <see cref="Browser2"/> API.</para>
/// <para>On iOS 9+, you will have to specify the <c>LSApplicationQueriesSchemes</c> key in the <c>info.plist</c> file with URI schemes you want to query from your app.</para>
/// <para>https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/Launcher</para>
/// </remarks>
[SupportedOSPlatform("android")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("maccatalyst")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
public static partial class Launcher
{
    /// <summary>
    /// Queries if the device supports opening the given URI scheme.
    /// </summary>
    /// <param name="uri">URI scheme to query.</param>
    /// <returns><see langword="true"/> if opening is supported, otherwise <see langword="false"/>.</returns>
    /// <exception cref="UriFormatException">Thrown when <paramref name="uri"/> is malformed.</exception>
    public static ValueTask<bool> CanOpenAsync(Uri uri)
    {
#if IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = WebUtils.GetNativeUrl(uri);
        var r = UIApplication.SharedApplication.CanOpenUrl(nativeUrl);
#pragma warning restore CA1416 // 验证平台兼容性
        return new(r);
#elif MACOS
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = WebUtils.GetNativeUrl(uri);
        var r = NSWorkspace.SharedWorkspace.UrlForApplication(nativeUrl) != null;
        return new(r);
#pragma warning restore CA1416 // 验证平台兼容性
#elif ANDROID
        var nativeUrl = AndroidUri.Parse(uri.OriginalString);
        var intent = new Intent(Intent.ActionView, nativeUrl);
#pragma warning disable CA1416 // 验证平台兼容性
        var r = PlatformUtils.IsIntentSupported(intent);
#pragma warning restore CA1416 // 验证平台兼容性
        return new(r);
#else
        return new(true);
#endif
    }

    /// <summary>
    /// Queries if the device supports opening the given URI scheme.
    /// </summary>
    /// <param name="uri">URI scheme to query.</param>
    /// <returns><see langword="true"/> if opening is supported, otherwise <see langword="false"/>.</returns>
    /// <exception cref="UriFormatException">Thrown when <paramref name="uri"/> is malformed.</exception>
    public static ValueTask<bool> CanOpenAsync(string uri)
    {
#if IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = new NSUrl(uri);
        var r = UIApplication.SharedApplication.CanOpenUrl(nativeUrl);
#pragma warning restore CA1416 // 验证平台兼容性
        return new(r);
#elif MACOS
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = new NSUrl(uri);
        var r = NSWorkspace.SharedWorkspace.UrlForApplication(nativeUrl) != null;
        return new(r);
#pragma warning restore CA1416 // 验证平台兼容性
#elif ANDROID
        var nativeUrl = AndroidUri.Parse(uri);
        var intent = new Intent(Intent.ActionView, nativeUrl);
#pragma warning disable CA1416 // 验证平台兼容性
        var r = PlatformUtils.IsIntentSupported(intent);
#pragma warning restore CA1416 // 验证平台兼容性
        return new(r);
#else
        return new(true);
#endif
    }

    /// <summary>
    /// Opens the app specified by the URI scheme.
    /// </summary>
    /// <param name="uri">URI to open.</param>
    /// <returns><see langword="true"/> if the URI was opened, otherwise <see langword="false"/>.</returns>
    /// <exception cref="UriFormatException">Thrown when <paramref name="uri"/> is malformed.</exception>
    public static ValueTask<bool> OpenAsync(Uri uri)
    {
#if IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = WebUtils.GetNativeUrl(uri);
        return OpenAsync(nativeUrl);
#pragma warning restore CA1416 // 验证平台兼容性
#elif MACOS
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = WebUtils.GetNativeUrl(uri);
        var r = NSWorkspace.SharedWorkspace.OpenUrl(nativeUrl);
        return new(r);
#pragma warning restore CA1416 // 验证平台兼容性
#elif ANDROID
        return OpenAsync(uri.OriginalString);
#else
        var r = IBrowserPlatformService.OpenCoreByProcess(uri.AbsoluteUri);
        return new(r);
#endif
    }

#if IOS || MACCATALYST
    /// <summary>
    /// Opens the app specified by the URI scheme.
    /// </summary>
    /// <param name="nativeUrl">URI to open.</param>
    /// <returns><see langword="true"/> if the URI was opened, otherwise <see langword="false"/>.</returns>
    /// <exception cref="UriFormatException">Thrown when <paramref name="nativeUrl"/> is malformed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> OpenAsync(NSUrl nativeUrl)
    {
#pragma warning disable CA1416 // 验证平台兼容性
        UIApplicationOpenUrlOptions options = new();
#pragma warning restore CA1416 // 验证平台兼容性
#pragma warning disable CA1416 // 验证平台兼容性
        var r = await UIApplication.SharedApplication.OpenUrlAsync(nativeUrl, options);
#pragma warning restore CA1416 // 验证平台兼容性
        return r;
    }
#endif

    /// <summary>
    /// Opens the app specified by the URI scheme.
    /// </summary>
    /// <param name="uri">URI to open.</param>
    /// <returns><see langword="true"/> if the URI was opened, otherwise <see langword="false"/>.</returns>
    /// <exception cref="UriFormatException">Thrown when <paramref name="uri"/> is malformed.</exception>
    public static ValueTask<bool> OpenAsync(string uri)
    {
#if IOS || MACCATALYST
        var nativeUrl = new NSUrl(uri);
        return OpenAsync(nativeUrl);
#elif MACOS
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = new NSUrl(uri);
        var r = NSWorkspace.SharedWorkspace.OpenUrl(nativeUrl);
        return new(r);
#pragma warning restore CA1416 // 验证平台兼容性
#elif ANDROID
        var nativeUrl = AndroidUri.Parse(uri);
        var intent = new Intent(Intent.ActionView, nativeUrl);
        var flags = ActivityFlags.ClearTop | ActivityFlags.NewTask;
        intent.SetFlags(flags);
        Application.Context.StartActivity(intent);
        return new(true);
#else
        var r = IBrowserPlatformService.OpenCoreByProcess(uri);
        return new(r);
#endif
    }

#if ANDROID
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async ValueTask<bool> PlatformTryOpenAsync(string uri)
    {
        var canOpen = await CanOpenAsync(uri);

        if (canOpen)
            await OpenAsync(uri);

        return canOpen;
    }
#endif

    /// <summary>
    /// First checks if the provided URI is supported, then opens the app specified by the URI.
    /// </summary>
    /// <param name="uri">URI to try and open.</param>
    /// <returns><see langword="true"/> if the URI was opened, otherwise <see langword="false"/>.</returns>
    /// <exception cref="UriFormatException">Thrown when <paramref name="uri"/> is malformed.</exception>
    public static ValueTask<bool> TryOpenAsync(Uri uri)
    {
#if IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = WebUtils.GetNativeUrl(uri);
        if (UIApplication.SharedApplication.CanOpenUrl(nativeUrl))
            return OpenAsync(nativeUrl);
        return new(false);
#pragma warning restore CA1416 // 验证平台兼容性
#elif MACOS
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = WebUtils.GetNativeUrl(uri);
        var r = NSWorkspace.SharedWorkspace.OpenUrl(nativeUrl);
        return new(r);
#pragma warning restore CA1416 // 验证平台兼容性
#elif ANDROID
        return PlatformTryOpenAsync(uri.OriginalString);
#else
        var r = IBrowserPlatformService.OpenCoreByProcess(uri.AbsoluteUri);
        return new(r);
#endif
    }

    /// <summary>
    /// First checks if the provided URI is supported, then opens the app specified by the URI.
    /// </summary>
    /// <param name="uri">URI to try and open.</param>
    /// <returns><see langword="true"/> if the URI was opened, otherwise <see langword="false"/>.</returns>
    /// <exception cref="UriFormatException">Thrown when <paramref name="uri"/> is malformed.</exception>
    public static ValueTask<bool> TryOpenAsync(string uri)
    {
#if IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = new NSUrl(uri);
        if (UIApplication.SharedApplication.CanOpenUrl(nativeUrl))
            return OpenAsync(nativeUrl);
        return new(false);
#pragma warning restore CA1416 // 验证平台兼容性
#elif MACOS
#pragma warning disable CA1416 // 验证平台兼容性
        var nativeUrl = new NSUrl(uri);
        var r = NSWorkspace.SharedWorkspace.OpenUrl(nativeUrl);
        return new(r);
#pragma warning restore CA1416 // 验证平台兼容性
#elif ANDROID
        return PlatformTryOpenAsync(uri);
#else
        var r = IBrowserPlatformService.OpenCoreByProcess(uri);
        return new(r);
#endif
    }
}
