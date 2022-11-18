namespace BD.Common.Services;

/// <summary>
/// 由平台实现的主线程帮助类
/// </summary>
interface IMainThreadPlatformService
{
    static IMainThreadPlatformService Instance => Ioc.Get<IMainThreadPlatformService>();

    bool PlatformIsMainThread { get; }

    void PlatformBeginInvokeOnMainThread(Action action, ThreadingDispatcherPriority priority = ThreadingDispatcherPriority.Normal);
}