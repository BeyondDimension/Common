namespace BD.Common8.Toast.Services;

/// <summary>
/// Toast通知的接口
/// </summary>
interface IToast
{
    /// <summary>
    /// 显示具有指定图标、文本和自定义持续时间的通知
    /// </summary>
    void Show(ToastIcon icon, string text, int? duration);

    /// <summary>
    /// 显示具有指定图标、文本和持续时间的通知
    /// </summary>
    void Show(ToastIcon icon, string text, ToastLength duration);

    /// <summary>
    /// 日志记录异常并显示、标签、日志级别、成员名称、消息和参数的通知
    /// </summary>
    void LogAndShow(Exception? e,
        string? tag, LogLevel level,
        string memberName,
        string? msg, params object?[] args);

    /// <summary>
    /// 日志记录异常并显示、日志记录器、日志级别、成员名称、消息和参数的通知
    /// </summary>
    void LogAndShow(Exception? e,
        ILogger logger, LogLevel level,
        string memberName,
        string? msg, params object?[] args);
}
