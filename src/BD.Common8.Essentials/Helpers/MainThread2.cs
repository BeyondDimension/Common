namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 适用于桌面端的主线程帮助类，参考 Xamarin.Essentials.MainThread
/// <para>https://docs.microsoft.com/zh-cn/xamarin/essentials/main-thread</para>
/// <para>https://github.com/xamarin/Essentials/blob/main/Xamarin.Essentials/MainThread/MainThread.shared.cs</para>
/// </summary>
public static class MainThread2
{
    /// <summary>
    /// 获取当前是否为主线程
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMainThread()
    {
        var mainThreadPlatformService = IMainThreadPlatformService.Instance;
        if (mainThreadPlatformService != null)
        {
            return mainThreadPlatformService.PlatformIsMainThread;
        }
        return default;
    }

    /// <summary>
    /// 调用应用程序主线程上的操作
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    /// <param name="priority"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        var mainThreadPlatformService = IMainThreadPlatformService.Instance;
        if (mainThreadPlatformService != null)
        {
            mainThreadPlatformService.BeginInvokeOnMainThread(action, priority);
            return;
        }
        action?.Invoke();
    }

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task InvokeOnMainThreadAsync(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        var mainThreadPlatformService = IMainThreadPlatformService.Instance;
        if (mainThreadPlatformService != null)
        {
            return mainThreadPlatformService.InvokeOnMainThreadAsync(action, priority);
        }
        action?.Invoke();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        var mainThreadPlatformService = IMainThreadPlatformService.Instance;
        if (mainThreadPlatformService != null)
        {
            return mainThreadPlatformService.InvokeOnMainThreadAsync(func, priority);
        }
        return Task.FromResult(func == null ? default! : func());
    }

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <param name="funcTask"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task InvokeOnMainThreadAsync(Func<Task> funcTask, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        var mainThreadPlatformService = IMainThreadPlatformService.Instance;
        if (mainThreadPlatformService != null)
        {
            return mainThreadPlatformService.InvokeOnMainThreadAsync(funcTask, priority);
        }
        return funcTask == null ? Task.CompletedTask : funcTask();
    }

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="funcTask"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        var mainThreadPlatformService = IMainThreadPlatformService.Instance;
        if (mainThreadPlatformService != null)
        {
            return mainThreadPlatformService.InvokeOnMainThreadAsync(funcTask, priority);
        }
        return funcTask == null ? Task.FromResult<T>(default!) : funcTask();
    }
}
