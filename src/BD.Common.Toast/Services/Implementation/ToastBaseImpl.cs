namespace BD.Common.Services.Implementation;

public abstract class ToastBaseImpl : IToast
{
    protected readonly IToastIntercept intercept;
    protected readonly IMainThreadPlatformService mainThread;

    protected const string TAG = "Toast";

    public ToastBaseImpl(IToastIntercept intercept, IMainThreadPlatformService mainThread)
    {
        this.intercept = intercept;
        this.mainThread = mainThread;
    }

    /// <summary>
    /// 根据字符串长度计算持续时间
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    protected virtual int CalcDurationByStringLength(int len)
        => len > 7 ? ToDuration(ToastLength.Long) : ToDuration(ToastLength.Short);

    protected virtual bool IsMainThread
        => mainThread.PlatformIsMainThread;

    protected virtual void BeginInvokeOnMainThread(Action action)
        => mainThread.BeginInvokeOnMainThread(action);

    public void Show(string text, int? duration)
    {
        if (string.IsNullOrEmpty(text)) return;

        try
        {
            if (IsMainThread)
            {
                Show_();
            }
            else
            {
                BeginInvokeOnMainThread(Show_);
            }

            void Show_()
            {
                if (intercept.OnShowExecuting(text)) return;
                var duration_ = duration ?? CalcDurationByStringLength(text.Length);
                PlatformShow(text, duration_);
            }
        }
        catch (Exception e)
        {
            Log.Error(TAG, e, "ShowToast Error, text: {0}", text);
        }
    }

    public void Show(string text, ToastLength duration) => Show(text, ToDuration(duration));

    protected abstract void PlatformShow(string text, int duration);

    /// <summary>
    /// 将 <see cref="ToastLength"/> 转换为 持续时间
    /// </summary>
    /// <param name="toastLength"></param>
    /// <returns></returns>
    protected virtual int ToDuration(ToastLength toastLength) => (int)toastLength;

    protected static IServiceCollection TryAddToast<TToastImpl>(IServiceCollection services) where TToastImpl : class, IToast
    {
        services.TryAddSingleton<IToastIntercept, NoneToastIntercept>();
        services.TryAddSingleton<IToast, TToastImpl>();
        return services;
    }
}