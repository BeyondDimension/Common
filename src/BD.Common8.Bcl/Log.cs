#if ANDROID
using ALog = global::Android.Util.Log;
using ALogPriority = global::Android.Util.LogPriority;
#endif

namespace System;

#if !NETFRAMEWORK || (NETSTANDARD && NETSTANDARD2_0_OR_GREATER)
/// <summary>
/// 日志
/// <para>使用说明：</para>
/// <para>在类中定义 const string TAG = 类名(长度小于等于23)</para>
/// <para>使用 Log.Debug(TAG,... / Log.Info(TAG,... / Log.Warn(TAG,... / Log.Error(TAG,...</para>
/// </summary>
public static partial class Log
{
    /// <inheritdoc cref="ILoggerFactory"/>
    public static Func<ILoggerFactory>? LoggerFactory { private get; set; }

    static ILoggerFactory? factory;

    /// <inheritdoc cref="ILoggerFactory"/>
    public static ILoggerFactory Factory
    {
        get
        {
            try
            {
                var value = factory ??= (Ioc.IsConfigured ? Ioc.Get<ILoggerFactory>() : null) ?? LoggerFactory?.Invoke() ?? new EmptyLoggerFactory();
                return value;
            }
            catch (ObjectDisposedException)
            {
                return new EmptyLoggerFactory();
            }
        }
    }

    /// <inheritdoc cref="ILoggerFactory.CreateLogger(string)"/>
    public static ILogger CreateLogger(string tag)
    {
        try
        {
            var logger = Factory.CreateLogger(tag);
            return logger;
        }
        catch (ObjectDisposedException)
        {
            return new EmptyLogger();
        }
    }

    sealed class EmptyLogger : ILogger
    {
        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return null;
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }
    }

    sealed class EmptyLoggerFactory : ILoggerFactory
    {
        void ILoggerFactory.AddProvider(ILoggerProvider provider)
        {
        }

        ILogger ILoggerFactory.CreateLogger(string categoryName)
        {
            return new EmptyLogger();
        }

        void IDisposable.Dispose()
        {
        }
    }

#pragma warning disable CA2254 // 模板应为静态表达式

    #region Debug

    /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, string?, object?[])"/>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string tag, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, string?, object?[])"/>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string tag, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, Exception?, string?, object?[])"/>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string tag, Exception? exception, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(exception, message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, Exception?, string?, object?[])"/>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string tag, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, EventId, Exception?, string?, object?[])"/>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string tag, EventId eventId, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(eventId, exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, EventId, string?, object?[])"/>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string tag, EventId eventId, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogDebug(eventId, message, args);
    }

    #endregion

    #region Error

    /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string tag, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogError(message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string tag, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string tag, Exception? exception, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogError(exception, message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string tag, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, EventId, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string tag, EventId eventId, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(eventId, exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, EventId, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string tag, EventId eventId, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogError(eventId, message, args);
    }

    #endregion

    #region Info

    /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string tag, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string tag, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string tag, Exception? exception, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(exception, message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string tag, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, EventId, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string tag, EventId eventId, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(eventId, exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, EventId, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string tag, EventId eventId, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogInformation(eventId, message, args);
    }

    #endregion

    #region Warn

    /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(string tag, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(string tag, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(string tag, Exception? exception, string message)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(exception, message);
    }

    /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(string tag, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, EventId, Exception?, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(string tag, EventId eventId, Exception? exception, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(eventId, exception, message, args);
    }

    /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, EventId, string?, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(string tag, EventId eventId, string message, params object?[] args)
    {
        var logger = CreateLogger(tag);
        logger.LogWarning(eventId, message, args);
    }

    #endregion

    /// <summary>
    /// 日志接口定义
    /// </summary>
    public interface I
    {
        /// <inheritdoc cref="ILogger"/>
        ILogger Logger { get; }
    }

    /// <summary>
    /// 客户端日志
    /// <para>https://github.com/dotnet/extensions/blob/v3.1.5/src/Logging/Logging.Console/src/ConsoleLogger.cs</para>
    /// </summary>
    abstract class ClientLogger : ILogger
    {
        static readonly string _messagePadding;
        static readonly string _newLineWithMessagePadding;

        static ClientLogger()
        {
            _messagePadding = new string(' ', 6);
            _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        }

        protected readonly string name;

        public ClientLogger(string name) => this.name = name;

        public virtual IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public virtual bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                WriteMessage(logLevel, name, eventId.Id, message, exception);
            }
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string? message, Exception? exception)
        {
            var logBuilder = new StringBuilder();

            CreateDefaultLogMessage(logBuilder, logName, eventId, message, exception);

            var logMessage = logBuilder.ToString();
            WriteMessage(logLevel, logMessage);
        }

        void CreateDefaultLogMessage(StringBuilder logBuilder, string logName, int eventId, string? message, Exception? exception)
        {
            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received

            // category and event id
            logBuilder.Append(logName);
            logBuilder.Append("[");
            logBuilder.Append(eventId);
            logBuilder.AppendLine("]");

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(_messagePadding);

                var len = logBuilder.Length;
                logBuilder.AppendLine(message);
                logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, message.Length);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.AppendLine(exception.ToString());
            }
        }

        public abstract void WriteMessage(LogLevel logLevel, string message);
    }

    /// <summary>
    /// An empty scope without any logic
    /// <para>https://github.com/dotnet/extensions/blob/v3.1.5/src/Logging/shared/NullScope.cs</para>
    /// <para>https://github.com/dotnet/runtime/blob/v5.0.0-rtm.20519.4/src/libraries/Common/src/Extensions/Logging/NullScope.cs</para>
    /// </summary>
    class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope()
        {
        }

#pragma warning disable CA1816 // Dispose 方法应调用 SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose 方法应调用 SuppressFinalize
        {
        }
    }
}
#endif

#if ANDROID
static partial class Log
{
    /// <summary>
    /// Android 的 Log Tag 名称最大长度
    /// <para>如果超出此长度，会引发异常：</para>
    /// <para>Java.Lang.IllegalArgumentException: Log tag "Microsoft.EntityFrameworkCore.Infrastructure" exceeds limit of 23 characters</para>
    /// </summary>
    public const int droid_tag_max_len = 23;
    public const string Droid = "Droid";

    static string CutDroidTag(string name)
    {
        var array = name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        var lastItem = array.LastOrDefault(x => !string.IsNullOrWhiteSpace(x));
        if (lastItem != null)
        {
            if (lastItem.StartsWith(Droid, StringComparison.OrdinalIgnoreCase))
            {
                lastItem = lastItem[Droid.Length..];
            }
            if (lastItem.Length > droid_tag_max_len)
            {
                return lastItem[..droid_tag_max_len];
            }
            else
            {
                return lastItem;
            }
        }
        else
        {
            return "Droid";
        }
    }

    /// <summary>
    /// 根据 Name 获取 Android Log Tag
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetDroidTag(string name)
    {
        if (name.Length > droid_tag_max_len)
        {
            return CutDroidTag(name);
        }
        return name;
    }

    /// <summary>
    /// 将MS扩展日志等级(<see cref="LogLevel"/>)转换为安卓日志等级(<see cref="ALogPriority"/>)
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static ALogPriority ToLogPriority(this LogLevel level) => level switch
    {
        LogLevel.Trace => ALogPriority.Verbose,
        LogLevel.Debug => ALogPriority.Debug,
        LogLevel.Information => ALogPriority.Info,
        LogLevel.Warning => ALogPriority.Warn,
        LogLevel.Error => ALogPriority.Error,
        LogLevel.Critical => ALogPriority.Assert,
        _ => (ALogPriority)int.MaxValue,
    };

    /// <inheritdoc cref="ClientLogger"/>
    class PlatformLogger(string name) : ClientLogger(name)
    {
        readonly string tag = GetDroidTag(name);

        public override bool IsEnabled(LogLevel logLevel)
        {
            var priority = logLevel.ToLogPriority();
            var result = ALog.IsLoggable(tag, priority);
            return result;
        }

        public override void WriteMessage(LogLevel logLevel, string message)
        {
            var priority = logLevel.ToLogPriority();
            ALog.WriteLine(priority, tag, message);
        }
    }

    [ProviderAlias("Droid")]
    public class PlatformLoggerProvider : ILoggerProvider
    {
        private PlatformLoggerProvider() { }

        public ILogger CreateLogger(string name)
        {
            return new PlatformLogger(name);
        }

        void IDisposable.Dispose()
        {
        }

        public static ILoggerProvider Instance { get; } = new PlatformLoggerProvider();
    }
}
#endif