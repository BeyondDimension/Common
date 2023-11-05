namespace BD.Common8.Essentials.Services;

/// <summary>
/// 由平台实现的主线程帮助类
/// </summary>
public interface IMainThreadPlatformService
{
    /// <summary>
    /// 获取 <see cref="IMainThreadPlatformService"/> 的实例
    /// </summary>
    static IMainThreadPlatformService? Instance => Ioc.Get_Nullable<IMainThreadPlatformService>();

    /// <summary>
    /// 获取当前平台是否处于主线程中
    /// </summary>
    bool PlatformIsMainThread { get; }

    /// <summary>
    /// 在主线程上调度指定的操作
    /// </summary>
    /// <param name="action">要在主线程上执行的操作</param>
    /// <param name="priority">操作的优先级</param>
    void PlatformBeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal);

    /// <summary>
    /// 在主线程上执行给定的操作，如果是在主线程上运行则直接执行操作，否则在主线程上调度执行
    /// </summary>
    /// <param name="action">要在主线程上执行的操作</param>
    /// <param name="priority">操作的优先级</param>
    void BeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        if (PlatformIsMainThread)
        {
            action();
        }
        else
        {
            try
            {
                PlatformBeginInvokeOnMainThread(action, priority);
            }
            catch (InvalidOperationException)
            {
                // https://github.com/dotnet/maui/blob/48840b8dd4f63e298ac63af7f9696f7e0581589c/src/Essentials/src/MainThread/MainThread.uwp.cs#L16-L20
                action();
            }
        }
    }

    /// <summary>
    /// 在主线程上异步执行指定的操作，并返回表示异步操作的任务
    /// </summary>
    Task InvokeOnMainThreadAsync(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        if (PlatformIsMainThread)
        {
            action();
#if NETSTANDARD1_0
            return Task.FromResult(true);
#else
            return Task.CompletedTask;
#endif
        }

        var tcs = new TaskCompletionSource<bool>();

        BeginInvokeOnMainThread(() =>
        {
            try
            {
                action();
                tcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, priority);

        return tcs.Task;
    }

    /// <summary>
    /// 在主线程上异步执行指定的操作，并返回操作的结果，通过泛型参数指定操作结果的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    Task<T> InvokeOnMainThreadAsync<T>(Func<T> func, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        if (PlatformIsMainThread)
        {
            return Task.FromResult(func());
        }

        var tcs = new TaskCompletionSource<T>();

        BeginInvokeOnMainThread(() =>
        {
            try
            {
                var result = func();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, priority);

        return tcs.Task;
    }

    /// <summary>
    /// 在主线程上异步调用指定的函数任务
    /// </summary>
    Task InvokeOnMainThreadAsync(Func<Task> funcTask, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        if (PlatformIsMainThread)
        {
            return funcTask();
        }

        var tcs = new TaskCompletionSource<object?>();

        BeginInvokeOnMainThread(
            async () =>
            {
                try
                {
                    await funcTask().ConfigureAwait(false);
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, priority);

        return tcs.Task;
    }

    /// <summary>
    /// 在主线程上异步调用指定的泛型函数任务
    /// </summary>
    Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        if (PlatformIsMainThread)
        {
            return funcTask();
        }

        var tcs = new TaskCompletionSource<T>();

        BeginInvokeOnMainThread(
            async () =>
            {
                try
                {
                    var ret = await funcTask().ConfigureAwait(false);
                    tcs.SetResult(ret);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, priority);

        return tcs.Task;
    }
}