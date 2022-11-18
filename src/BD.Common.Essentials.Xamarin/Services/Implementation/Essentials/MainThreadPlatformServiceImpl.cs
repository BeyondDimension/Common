namespace BD.Common.Services.Implementation.Essentials;

sealed class MainThreadPlatformServiceImpl : IMainThreadPlatformService
{
    bool IMainThreadPlatformService.PlatformIsMainThread => MainThread.IsMainThread;

    void IMainThreadPlatformService.PlatformBeginInvokeOnMainThread(Action action,
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        ThreadingDispatcherPriority _) => MainThread.BeginInvokeOnMainThread(action);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
