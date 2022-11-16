// ReSharper disable once CheckNamespace
namespace System;

public static partial class ExceptionExtensions
{
    public const string see_inner_exception = "see inner exception";
    public const byte each_inner_exception_cycle = 12;

    /// <summary>
    /// 当异常存在 see inner exception 时获取内部异常
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="cycle">查找深度，默认值为<see cref="each_inner_exception_cycle"/></param>
    /// <returns></returns>
    public static Exception GetRealException(this Exception ex, byte cycle = each_inner_exception_cycle)
    {
        for (byte i = 0; i < cycle; i++)
        {
            if (ex.InnerException != null &&
                ex.Message.Contains(see_inner_exception, StringComparison.OrdinalIgnoreCase))
            {
                ex = ex.InnerException;
            }
            else
            {
                break;
            }
        }
        return ex;
    }

    /// <summary>
    /// 当前平台是否包含 Java Interop，这通常仅在 Android 上包含，也许在未来支持 Java 互操作性的服务端上也会包含
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool JavaInterop() => OperatingSystem.IsAndroid();

    /// <summary>
    /// 获取异常是否为已知类型，通常不为 <see cref="ExceptionKnownType.Unknown"/> 的异常不需要纪录日志
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static ExceptionKnownType GetKnownType(this Exception? e)
    {
        if (e != null)
        {
            if (e is TaskCanceledException)
            {
                return ExceptionKnownType.TaskCanceled;
            }
            else if (e is OperationCanceledException)
            {
                return ExceptionKnownType.OperationCanceled;
            }
            if (JavaInterop())
            {
                var typeName = e.GetType().FullName;
                switch (typeName)
                {
                    case "Java.Security.Cert.CertificateNotYetValidException":
                        // https://docs.oracle.com/javase/8/docs/api/java/security/cert/CertificateNotYetValidException.html
                        // https://learn.microsoft.com/en-us/dotnet/api/javax.security.cert.certificatenotyetvalidexception
                        return ExceptionKnownType.CertificateNotYetValid;
                    case "Java.IO.IOException":
                        // https://docs.oracle.com/javase/8/docs/api/java/io/IOException.html
                        // https://learn.microsoft.com/en-us/dotnet/api/java.io.ioexception
                        if (e.Message == "Canceled")
                        {
                            return ExceptionKnownType.Canceled;
                        }
                        break;
                }
            }
            e = e.InnerException;
            if (e != null)
            {
                return GetKnownType(e);
            }
        }
        return ExceptionKnownType.Unknown;
    }

    /// <summary>
    /// 是否为取消操作异常
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCanceledException(this ExceptionKnownType value)
        => value == ExceptionKnownType.Canceled ||
        value == ExceptionKnownType.OperationCanceled ||
        value == ExceptionKnownType.TaskCanceled;

    /// <summary>
    /// 获取异常中所有错误信息
    /// </summary>
    /// <param name="e">当前捕获的异常</param>
    /// <param name="msg">可选的消息，将写在第一行</param>
    /// <param name="args">可选的消息参数</param>
    /// <returns></returns>
    public static string GetAllMessage(this Exception e, string? msg = null, params object?[] args)
    {
        var has_msg = !string.IsNullOrWhiteSpace(msg);
        var has_args = args.Any_Nullable();
        return GetAllMessageCore(e, has_msg, has_args, msg, args);
    }

    static string GetAllMessageCore(Exception e,
        bool has_msg, bool has_args,
        string? msg = null, params object?[] args)
    {
        StringBuilder sb = new();

        if (has_msg)
        {
            if (has_args)
            {
                try
                {
                    sb.AppendFormat(msg!, args);
                }
                catch
                {
                    sb.Append(msg);
                    foreach (var item in args)
                    {
                        sb.Append(' ');
                        sb.Append(item);
                    }
                }
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine(msg!);
            }
        }

        var exception = e;
        ushort i = 0;
        while (exception != null && i++ < byte.MaxValue)
        {
            var exception_message = exception.Message;
            if (!string.IsNullOrWhiteSpace(exception_message))
            {
                sb.AppendLine(exception_message);
            }
            exception = exception.InnerException;
        }

        var text = sb.ToString().Trim();
        return text;
    }
}