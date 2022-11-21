namespace BD.Common;

/// <summary>
/// 适用于桌面端的主线程帮助类，参考 Xamarin.Essentials.MainThread
/// <para>https://docs.microsoft.com/zh-cn/xamarin/essentials/main-thread</para>
/// <para>https://github.com/xamarin/Essentials/blob/main/Xamarin.Essentials/MainThread/MainThread.shared.cs</para>
/// </summary>
public static class MainThread2
{
    static IMainThreadPlatformService? mInterface;

    static IMainThreadPlatformService Interface => mInterface ??= IMainThreadPlatformService.Instance;

    /// <summary>
    /// 获取当前是否为主线程
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMainThread() => Interface.PlatformIsMainThread;

    /// <summary>
    /// 调用应用程序主线程上的操作
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    /// <param name="priority"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
        => Interface.BeginInvokeOnMainThread(action, priority);

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task InvokeOnMainThreadAsync(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
        => Interface.InvokeOnMainThreadAsync(action, priority);

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
        => Interface.InvokeOnMainThreadAsync(func, priority);

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <param name="funcTask"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task InvokeOnMainThreadAsync(Func<Task> funcTask, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
        => Interface.InvokeOnMainThreadAsync(funcTask, priority);

    /// <summary>
    /// 异步调用主线程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="funcTask"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
        => Interface.InvokeOnMainThreadAsync(funcTask, priority);
}
