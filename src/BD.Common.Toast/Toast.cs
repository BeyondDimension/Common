namespace BD.Common;

/// <inheritdoc cref="IToast"/>
public static class Toast
{
    /// <inheritdoc cref="IToast.Show(string, int?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("use Show(ToastIcon, string, int?)")]
    public static void Show(string text, int? duration = null)
    {
        Show(ToastIcon.None, text, duration);
    }

    /// <inheritdoc cref="IToast.Show(string, ToastLength)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("use Show(ToastIcon, string, ToastLength)")]
    public static void Show(string text, ToastLength duration)
    {
        Show(ToastIcon.None, text, duration);
    }

    /// <inheritdoc cref="IToast.Show(ToastIcon, string, int?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Show(ToastIcon icon, string text, int? duration = null)
    {
        var toast = Ioc.Get<IToast>();
        toast.Show(icon, text, duration);
    }

    /// <inheritdoc cref="IToast.Show(ToastIcon, string, ToastLength)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Show(ToastIcon icon, string text, ToastLength duration)
    {
        var toast = Ioc.Get<IToast>();
        toast.Show(icon, text, duration);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ShowLongError(string text) => Show(ToastIcon.Error, text, ToastLength.Long);

#if DEBUG
    [Obsolete("use e.LogAndShowT(..", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Show(Exception e,
        string? tag = null, LogLevel level = LogLevel.Error,
        string? msg = null, params object?[] args) => e.LogAndShowT(tag, level, "", msg, args);
#endif

    /// <summary>
    /// 通过 <see cref="Exception"/> 纪录日志并在 Toast 上显示，传入 <see cref="LogLevel.None"/> 可不写日志
    /// </summary>
    /// <param name="e"></param>
    /// <param name="tag"></param>
    /// <param name="level"></param>
    /// <param name="memberName"></param>
    /// <param name="msg"></param>
    /// <param name="args"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogAndShowT(this Exception? e,
        string? tag = null, LogLevel level = LogLevel.Error,
        [CallerMemberName] string memberName = "",
        string? msg = null, params object?[] args)
        => ExceptionExtensions.LogAndShow(e,
            ShowLongError,
            string.IsNullOrWhiteSpace(tag) ? nameof(Toast) : tag, level,
            memberName,
            msg, args);

    /// <summary>
    /// 通过 <see cref="Exception"/> 纪录日志并在 Toast 上显示，传入 <see cref="LogLevel.None"/> 可不写日志
    /// </summary>
    /// <param name="e"></param>
    /// <param name="logger"></param>
    /// <param name="level"></param>
    /// <param name="memberName"></param>
    /// <param name="msg"></param>
    /// <param name="args"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogAndShowT(this Exception? e,
        ILogger logger, LogLevel level = LogLevel.Error,
        [CallerMemberName] string memberName = "",
        string? msg = null, params object?[] args) => ExceptionExtensions.LogAndShow(e,
            ShowLongError,
            logger, level,
            memberName, msg, args);
}