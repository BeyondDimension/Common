#if IOS || MACCATALYST
namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// Manager object that manages window states on iOS and macOS.
/// <para>https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/WindowStateManager.ios.cs</para>
/// </summary>
public static partial class WindowStateManager
{
    static Func<UIViewController?>? getCurrentController;

    /// <summary>
    /// Initializes this <see cref="WindowStateManager"/> instance.
    /// </summary>
    /// <param name="getCurrentUIViewController">The function task to retrieve the current <see cref="UIViewController"/>.</param>
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("ios")]
    public static void Init(Func<UIViewController?>? getCurrentUIViewController)
        => getCurrentController = getCurrentUIViewController;

    /// <summary>
    /// Gets the currently presented <see cref="UIViewController"/>.
    /// </summary>
    /// <param name="manager">The object to invoke this method on.</param>
    /// <param name="throwOnNull">Throws an exception if no current <see cref="UIViewController"/> can be found and this value is set to <see langword="true"/>, otherwise this method returns <see langword="null"/>.</param>
    /// <returns>The <see cref="UIViewController"/> object that is currently presented.</returns>
    /// <exception cref="NullReferenceException">Thrown if no current <see cref="UIViewController"/> can be found and <paramref name="throwOnNull"/> is set to <see langword="true"/>.</exception>
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("ios")]
    public static UIViewController? GetCurrentUIViewController([DoesNotReturnIf(true)] bool throwOnNull = false)
    {
        var vc = _GetCurrentUIViewController();
        if (throwOnNull && vc == null)
            throw new NullReferenceException("The current view controller can not be detected.");

        return vc;

        static UIViewController? _GetCurrentUIViewController()
        {
            var viewController = getCurrentController?.Invoke();

            if (viewController != null)
                return viewController;

            var window = GetKeyWindow();

            if (window != null && window.WindowLevel == UIWindowLevel.Normal)
                viewController = window.RootViewController;

            if (viewController == null)
            {
                window = GetWindows()?
                    .OrderByDescending(w => w.WindowLevel)
                    .FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);

                viewController = window?.RootViewController;
            }

            while (viewController?.PresentedViewController != null)
                viewController = viewController.PresentedViewController;

            return viewController;
        }
    }

    /// <summary>
    /// Gets the currently active <see cref="UIWindow"/>.
    /// </summary>
    /// <returns>The <see cref="UIWindow"/> object that is currently active.</returns>
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("ios")]
    public static UIWindow? GetCurrentUIWindow([DoesNotReturnIf(true)] bool throwOnNull = false)
    {
        var window = _GetCurrentUIWindow();
        if (throwOnNull && window == null)
            throw new NullReferenceException("The current window can not be detected.");

        return window;

        static UIWindow? _GetCurrentUIWindow()
        {
            var window = GetKeyWindow();

            if (window != null && window.WindowLevel == UIWindowLevel.Normal)
                return window;

            if (window == null)
            {
                window = GetWindows()?
                    .OrderByDescending(w => w.WindowLevel)
                    .FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);
            }

            return window;
        }
    }

    static UIWindow? GetKeyWindow()
    {
        // if we have scene support, use that
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
        {
            try
            {
#pragma warning disable CA1416 // 验证平台兼容性
                using var scenes = UIApplication.SharedApplication.ConnectedScenes;
                var windowScene = scenes.ToArray<UIWindowScene>().FirstOrDefault();
                return windowScene?.Windows.FirstOrDefault();
#pragma warning restore CA1416 // 验证平台兼容性
            }
            catch (InvalidCastException)
            {
                // HACK: Workaround for https://github.com/xamarin/xamarin-macios/issues/13704
                //       This only throws if the collection is empty.
                return null;
            }
        }

        // use the windows property (up to 13.0)
        return UIApplication.SharedApplication.KeyWindow;
    }

    static UIWindow[]? GetWindows()
    {
        // if we have scene support, use that
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
        {
            try
            {
#pragma warning disable CA1416 // 验证平台兼容性
                using var scenes = UIApplication.SharedApplication.ConnectedScenes;
                var windowScene = scenes.ToArray<UIWindowScene>().FirstOrDefault();
                return windowScene?.Windows;
#pragma warning restore CA1416 // 验证平台兼容性
            }
            catch (InvalidCastException)
            {
                // HACK: Workaround for https://github.com/xamarin/xamarin-macios/issues/13704
                //       This only throws if the collection is empty.
                return null;
            }
        }

        // use the windows property (up to 15.0)
        return UIApplication.SharedApplication.Windows;
    }
}
#endif