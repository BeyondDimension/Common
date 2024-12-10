namespace BD.Common8.Essentials.Services.Implementation;

/// <summary>
/// Avalonia 主线程平台服务实现
/// </summary>
sealed class AvaloniaMainThreadPlatformServiceImpl : IMainThreadPlatformService
{
    /// <summary>
    /// 判断当前平台是否为主线程
    /// </summary>
    public bool PlatformIsMainThread
    {
        get
        {
            var r = Dispatcher.UIThread.CheckAccess();
            return r;
        }
    }

    /// <summary>
    /// 在主线程上开始执行指定的操作
    /// </summary>
    public void PlatformBeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal)
    {
#if PROJ_MOBIUS
        if (!HostConstants.UserInteractive)
        {
            action?.Invoke();
            return;
        }
#endif
        var priority_ = GetPriority(priority);
        Dispatcher.UIThread.Post(action, priority_);
    }

    /// <summary>
    /// 根据给定的优先级获取对应的 Avalonia 调度器优先级
    /// </summary>
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
