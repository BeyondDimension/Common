namespace BD.Common8.Essentials.Extensions;

public static partial class ApplicationExtensions
{
    /// <summary>
    /// 获取主窗口、活动窗口或主视图
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TopLevel? GetMainWindowOrActiveWindowOrMainView(this AvaApplication? avaloniaApp)
    {
        if (avaloniaApp != null)
        {
            var applicationLifetime = avaloniaApp.ApplicationLifetime;
            if (applicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime)
            {
                var mainWindow = classicDesktopStyleApplicationLifetime.MainWindow;
                if (mainWindow != null)
                    return mainWindow;
                var windows = classicDesktopStyleApplicationLifetime.Windows;
                if (windows != null)
                {
                    Window? firstWindow = null;
                    foreach (var window in windows)
                    {
                        firstWindow ??= window;
                        if (window.IsActive)
                            return window;
                    }
                    return firstWindow;
                }
            }
            else if (applicationLifetime is ISingleViewApplicationLifetime singleViewApplicationLifetime)
            {
                var mainView = singleViewApplicationLifetime.MainView;
                if (mainView is TopLevel topLevel)
                    return topLevel;
                else
                    return TopLevel.GetTopLevel(mainView);
            }
        }
        return null;
    }
}
