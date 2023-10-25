namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

sealed class AvaloniaMainThreadPlatformServiceImpl : IMainThreadPlatformService
{
    public bool PlatformIsMainThread
    {
        get
        {
            var r = Dispatcher.UIThread.CheckAccess();
            return r;
        }
    }

    public void PlatformBeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
        var priority_ = GetPriority(priority);
        Dispatcher.UIThread.Post(action, priority_);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DispatcherPriority GetPriority(ThreadingDispatcherPriority priority) => priority switch
    {
#pragma warning disable CS0618 // 类型或成员已过时
        ThreadingDispatcherPriority.Invalid or ThreadingDispatcherPriority.Inactive => DispatcherPriority.Inactive,
        ThreadingDispatcherPriority.SystemIdle => DispatcherPriority.SystemIdle,
        ThreadingDispatcherPriority.ApplicationIdle => DispatcherPriority.ApplicationIdle,
        ThreadingDispatcherPriority.ContextIdle => DispatcherPriority.ContextIdle,
        ThreadingDispatcherPriority.Background => DispatcherPriority.Background,
        ThreadingDispatcherPriority.Input => DispatcherPriority.Input,
        ThreadingDispatcherPriority.Loaded => DispatcherPriority.Loaded,
        ThreadingDispatcherPriority.Render => DispatcherPriority.Render,
        ThreadingDispatcherPriority.DataBind => DispatcherPriority.DataBind,
        ThreadingDispatcherPriority.Normal => DispatcherPriority.Normal,
        ThreadingDispatcherPriority.Send => DispatcherPriority.Send,
        _ => priority > ThreadingDispatcherPriority.Send ? DispatcherPriority.Send : DispatcherPriority.Inactive,
#pragma warning restore CS0618 // 类型或成员已过时
    };
}
