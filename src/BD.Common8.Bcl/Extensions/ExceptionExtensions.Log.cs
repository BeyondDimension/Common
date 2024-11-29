#if !NETFRAMEWORK || (NETSTANDARD && NETSTANDARD2_0_OR_GREATER)
namespace System.Extensions;

public static partial class ExceptionExtensions
{
    /// <summary>
    /// 通过 <see cref="Exception"/> 纪录日志并在 UI 上显示，传入 <see cref="LogLevel.None"/> 可不写日志
    /// </summary>
    /// <param name="e"></param>
    /// <param name="show"></param>
    /// <param name="tag"></param>
    /// <param name="level"></param>
    /// <param name="memberName"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogAndShow(Exception? e,
        Action<string>? show,
        string tag, LogLevel level = LogLevel.Error,
        string memberName = "",
        string? message = null,
        params object?[] args) => LogAndShowCore(e, show, tag, logger: null, level, memberName, message, args);

    /// <inheritdoc cref="LogAndShow(Exception?, Action{string}?, ILogger?, LogLevel, string, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogAndShow(Exception? e,
        Action<string>? show,
        ILogger? logger, LogLevel level = LogLevel.Error,
        string memberName = "",
        string? message = null, params object?[] args) => LogAndShowCore(e, show, tag: null, logger, level, memberName, message, args);

    static void LogAndShowCore(Exception? e,
        Action<string>? show,
        string? tag = null, ILogger? logger = null, LogLevel level = LogLevel.Error,
        string memberName = "",
        string? message = null, params object?[] args)
    {
        bool has_message = !string.IsNullOrWhiteSpace(message);
        if (!has_message)
        {
            if (!string.IsNullOrWhiteSpace(memberName))
            {
                message = $"{memberName} Error";
                has_message = true;
            }
        }
        var has_args = args.Any_Nullable();
        var has_e = e != null;
        if (has_e)
        {
            var knownType = e!.GetKnownType();
            if (knownType != ExceptionKnownType.Unknown) level = LogLevel.None;
        }
        var has_log_level = level < LogLevel.None;
        if (has_log_level)
        {
            if (logger == null && tag != null)
            {
                logger = Log.CreateLogger(tag);
            }
            if (logger != null)
            {
#pragma warning disable CA2254 // 模板应为静态表达式
                if (has_args)
                {
                    logger.Log(level, e, message!, args);
                }
                else
                {
                    logger.Log(level, e, message!);
                }
#pragma warning restore CA2254 // 模板应为静态表达式
            }
        }
        try
        {
            show?.Invoke(GetShowMsg());
        }
        catch
        {
        }
        string GetShowMsg()
        {
            if (has_e)
            {
                StringBuilder sb = new();
                GetAllMessageCore(sb, e!, has_message, has_args, message, args);
                return sb.ToString();
            }
            if (has_message)
            {
                if (has_args)
                {
                    return message!.Format(args);
                }
                else
                {
                    return message!;
                }
            }
            return "";
        }
    }
}
#endif