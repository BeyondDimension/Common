#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Avalonia;

#pragma warning disable SA1600 // Elements should be documented

public static partial class ApplicationExtensions
{
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
                {
                    return mainWindow;
                }
                var windows = classicDesktopStyleApplicationLifetime.Windows;
                if (windows != null)
                {
                    Window? firstWindow = null;
                    foreach (var window in windows)
                    {
                        firstWindow ??= window;
                        if (window.IsActive)
                        {
                            return window;
                        }
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
