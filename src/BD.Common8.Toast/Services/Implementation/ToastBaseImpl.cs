namespace BD.Common8.Toast.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

public abstract class ToastBaseImpl(IToastIntercept intercept) : IToast
{
    protected readonly IToastIntercept intercept = intercept;

    protected const string TAG = "Toast";

    /// <summary>
    /// 根据字符串长度计算持续时间
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    protected virtual int CalcDurationByStringLength(int len)
        => len > 7 ? ToDuration(ToastLength.Long) : ToDuration(ToastLength.Short);

    protected virtual bool IsMainThread => true;

    protected virtual void BeginInvokeOnMainThread(Action action) => action.Invoke();

    public void Show(ToastIcon icon, string text, int? duration)
    {
        if (string.IsNullOrEmpty(text))
            return;
        try
        {
            if (IsMainThread)
                Show_();
            else
                BeginInvokeOnMainThread(Show_);

            void Show_()
            {
                if (intercept.OnShowExecuting(icon, text, duration)) return;
                var duration_ = duration ?? CalcDurationByStringLength(text.Length);
                PlatformShow(icon, text, duration_);
            }
        }
        catch (Exception e)
        {
            Log.Error(TAG, e, "ShowToast Error, text: {0}", text);
        }
    }

    public void Show(ToastIcon icon, string text, ToastLength duration)
        => Show(icon, text, ToDuration(duration));

    protected abstract void PlatformShow(ToastIcon icon, string text, int duration);

    /// <summary>
    /// 将 <see cref="ToastLength"/> 转换为 持续时间
    /// </summary>
    /// <param name="toastLength"></param>
    /// <returns></returns>
    protected virtual int ToDuration(ToastLength toastLength) => (int)toastLength;

    protected static IServiceCollection TryAddToast<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TToastImpl>(IServiceCollection services) where TToastImpl : ToastBaseImpl
    {
        services.TryAddSingleton<IToastIntercept, NoneToastIntercept>();
        services.TryAddSingleton<IToast, TToastImpl>();
        return services;
    }

    protected virtual ToastIcon Convert(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace or LogLevel.Debug or LogLevel.Information => ToastIcon.Info,
        LogLevel.Warning => ToastIcon.Warning,
        LogLevel.Critical or LogLevel.Error => ToastIcon.Error,
        _ => ToastIcon.None,
    };

    public virtual void LogAndShow(Exception? e,
         string? tag, LogLevel level,
         string memberName,
         string? msg, params object?[] args)
    {
        tag ??= TAG;
        ExceptionExtensions.LogAndShow(e,
            text => Show(Convert(level), text, ToastLength.Long),
            string.IsNullOrWhiteSpace(tag) ? nameof(ToastHelper) : tag, level,
            memberName,
            msg, args);
    }

    public virtual void LogAndShow(Exception? e,
        ILogger logger, LogLevel level,
        string memberName,
        string? msg, params object?[] args)
    {
        logger ??= Log.CreateLogger(TAG);
        ExceptionExtensions.LogAndShow(e,
            text => Show(Convert(level), text, ToastLength.Long),
            logger, level,
            memberName, msg, args);
    }
}