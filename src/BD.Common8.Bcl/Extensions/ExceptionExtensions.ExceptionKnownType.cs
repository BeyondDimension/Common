namespace System.Extensions;

public static partial class ExceptionExtensions // ExceptionKnownType
{
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
#if NET5_0_OR_GREATER
            if (OperatingSystem.IsAndroid())
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
#endif
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
}
