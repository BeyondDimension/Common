namespace BD.Common8.Toast.Services;

#pragma warning disable SA1600 // Elements should be documented

interface IToast
{
    void Show(ToastIcon icon, string text, int? duration);

    void Show(ToastIcon icon, string text, ToastLength duration);

    void LogAndShow(Exception? e,
        string? tag, LogLevel level,
        string memberName,
        string? msg, params object?[] args);

    void LogAndShow(Exception? e,
        ILogger logger, LogLevel level,
        string memberName,
        string? msg, params object?[] args);
}
