namespace BD.Common8.Toast.Helpers;

/// <summary>
/// Toast 属于一种轻量级的反馈，常常以小弹框的形式出现，一般出现 2 秒或 3.5 秒后会自动消失，可以出现在屏幕上中下任意位置，但同个产品会模块尽量使用同一位置，让用户产生统一认知
/// </summary>
public static class ToastHelper
{
    /// <summary>
    /// 显示 Toast
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Show(ToastIcon icon, string text, int? duration = null)
    {
        var toast = Ioc.Get<IToast>();
        toast.Show(icon, text, duration);
    }

    /// <summary>
    /// 显示 Toast
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Show(ToastIcon icon, string text, ToastLength duration)
    {
        var toast = Ioc.Get<IToast>();
        toast.Show(icon, text, duration);
    }

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
    public static void LogAndShow(this Exception? e,
        string? tag = null, LogLevel level = LogLevel.Error,
        [CallerMemberName] string memberName = "",
        string? msg = null, params object?[] args)
    {
        var toast = Ioc.Get<IToast>();
        toast.LogAndShow(e, tag, level, memberName, msg, args);
    }

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
    public static void LogAndShow(this Exception? e,
        ILogger logger, LogLevel level = LogLevel.Error,
        [CallerMemberName] string memberName = "",
        string? msg = null, params object?[] args)
    {
        var toast = Ioc.Get<IToast>();
        toast.LogAndShow(e, logger, level, memberName, msg, args);
    }
}