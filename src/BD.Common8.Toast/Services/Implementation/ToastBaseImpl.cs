namespace BD.Common8.Toast.Services.Implementation;

/// <summary>
/// 提供用于显示 Toast 提示的通用逻辑，并定义了一些可重写的方法和属性供派生类实现具体的平台相关的操作
/// </summary>
public abstract class ToastBaseImpl(IToastIntercept intercept) : IToast
{
    /// <summary>
    /// <see cref="IToastIntercept"/> 实列
    /// </summary>
    protected readonly IToastIntercept intercept = intercept;

    /// <summary>
    /// TAG
    /// </summary>
    protected const string TAG = "Toast";

    /// <summary>
    /// 根据字符串长度计算持续时间
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    protected virtual int CalcDurationByStringLength(int len)
        => len > 7 ? ToDuration(ToastLength.Long) : ToDuration(ToastLength.Short);

    /// <summary>
    /// 获取当前线程是否为主线程
    /// </summary>
    protected virtual bool IsMainThread => true;

    /// <summary>
    /// 在主线程上执行指定的操作
    /// </summary>
    protected virtual void BeginInvokeOnMainThread(Action action) => action.Invoke();

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void Show(ToastIcon icon, string text, ToastLength duration)
        => Show(icon, text, ToDuration(duration));

    /// <summary>
    /// 平台特定的显示 Toast 方法，需要在派生类中实现
    /// </summary>
    protected abstract void PlatformShow(ToastIcon icon, string text, int duration);

    /// <summary>
    /// 将 <see cref="ToastLength"/> 转换为 持续时间
    /// </summary>
    /// <param name="toastLength"></param>
    /// <returns></returns>
    protected virtual int ToDuration(ToastLength toastLength) => (int)toastLength;

    /// <summary>
    /// 尝试添加 Toast 的服务依赖
    /// </summary>
    protected static IServiceCollection TryAddToast<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TToastImpl>(IServiceCollection services) where TToastImpl : ToastBaseImpl
    {
        services.TryAddSingleton<IToastIntercept, NoneToastIntercept>();
        services.TryAddSingleton<IToast, TToastImpl>();
        return services;
    }

    /// <summary>
    /// 根据日志级别转换为对应的 <see cref="ToastIcon"/> 图标类型
    /// </summary>
    protected virtual ToastIcon Convert(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace or LogLevel.Debug or LogLevel.Information => ToastIcon.Info,
        LogLevel.Warning => ToastIcon.Warning,
        LogLevel.Critical or LogLevel.Error => ToastIcon.Error,
        _ => ToastIcon.None,
    };

    /// <inheritdoc />
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

    /// <inheritdoc />
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