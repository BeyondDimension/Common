namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 由平台实现的主线程帮助类
/// </summary>
public interface IMainThreadPlatformService
{
    static IMainThreadPlatformService? Instance => Ioc.Get_Nullable<IMainThreadPlatformService>();

    bool PlatformIsMainThread { get; }

    void PlatformBeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal);

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