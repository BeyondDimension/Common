namespace System;

/// <summary>
/// 日志
/// <para>使用说明：</para>
/// <para>在类中定义 const string TAG = 类名(长度小于等于23)</para>
/// <para>使用 Log.Debug(TAG,... / Log.Info(TAG,... / Log.Warn(TAG,... / Log.Error(TAG,...</para>
/// </summary>
public static class Log
{
    public static Func<ILoggerFactory>? LoggerFactory { private get; set; }

    static ILoggerFactory? factory;

    public static ILoggerFactory Factory => factory ??= (Ioc.IsConfigured ? Ioc.Get<ILoggerFactory>() : null) ?? LoggerFactory?.Invoke() ?? throw new ArgumentNullException(nameof(LoggerFactory));

    public static ILogger CreateLogger(string tag) => Factory.CreateLogger(tag);

    #region Debug

    [Conditional("DEBUG")]
    public static void Debug(string tag, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(msg);
    }

    [Conditional("DEBUG")]
    public static void Debug(string tag, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(msg, args);
    }

    [Conditional("DEBUG")]
    public static void Debug(string tag, Exception? exception, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(exception, msg);
    }

    [Conditional("DEBUG")]
    public static void Debug(string tag, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(exception, msg, args);
    }

    [Conditional("DEBUG")]
    public static void Debug(string tag, EventId eventId, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(eventId, exception, msg, args);
    }

    [Conditional("DEBUG")]
    public static void Debug(string tag, EventId eventId, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(eventId, msg, args);
    }

    #endregion

    #region Error

    public static void Error(string tag, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogError(msg);
    }

    public static void Error(string tag, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(msg, args);
    }

    public static void Error(string tag, Exception? exception, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogError(exception, msg);
    }

    public static void Error(string tag, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(exception, msg, args);
    }

    public static void Error(string tag, EventId eventId, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(eventId, exception, msg, args);
    }

    public static void Error(string tag, EventId eventId, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(eventId, msg, args);
    }

    #endregion

    #region Info

    public static void Info(string tag, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(msg);
    }

    public static void Info(string tag, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(msg, args);
    }

    public static void Info(string tag, Exception? exception, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(exception, msg);
    }

    public static void Info(string tag, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(exception, msg, args);
    }

    public static void Info(string tag, EventId eventId, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(eventId, exception, msg, args);
    }

    public static void Info(string tag, EventId eventId, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(eventId, msg, args);
    }

    #endregion

    #region Warn

    public static void Warn(string tag, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(msg);
    }

    public static void Warn(string tag, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(msg, args);
    }

    public static void Warn(string tag, Exception? exception, string msg)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(exception, msg);
    }

    public static void Warn(string tag, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(exception, msg, args);
    }

    public static void Warn(string tag, EventId eventId, Exception? exception, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(eventId, exception, msg, args);
    }

    public static void Warn(string tag, EventId eventId, string msg, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(eventId, msg, args);
    }

    #endregion
}

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
    /// <param name="msg"></param>
    /// <param name="args"></param>
    public static void LogAndShow(Exception? e,
        Action<string>? show,
        string tag, LogLevel level = LogLevel.Error,
        string memberName = "",
        string? msg = null,
        params object?[] args) => LogAndShowCore(e, show, tag, logger: null, level, memberName, msg, args);

    /// <inheritdoc cref="LogAndShow(Exception?, Action{string}?, string, LogLevel, string, string?, object?[])"/>
    public static void LogAndShow(Exception? e,
        Action<string>? show,
        ILogger? logger, LogLevel level = LogLevel.Error,
        string memberName = "",
        string? msg = null, params object?[] args) => LogAndShowCore(e, show, tag: null, logger, level, memberName, msg, args);

    static void LogAndShowCore(Exception? e,
        Action<string>? show,
        string? tag = null, ILogger? logger = null, LogLevel level = LogLevel.Error,
        string memberName = "",
        string? msg = null, params object?[] args)
    {
        bool has_msg = !string.IsNullOrWhiteSpace(msg);
        if (!has_msg)
        {
            if (!string.IsNullOrWhiteSpace(memberName))
            {
                msg = $"{memberName} Error";
                has_msg = true;
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
                if (has_args)
                {
                    logger.Log(level, e, msg!, args);
                }
                else
                {
                    logger.Log(level, e, msg!);
                }
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
            if (has_e) return GetAllMessageCore(e!, has_msg, has_args, msg, args);
            if (has_msg)
            {
                if (has_args)
                {
                    return msg!.Format(args);
                }
                else
                {
                    return msg!;
                }
            }
            return "";
        }
    }
}