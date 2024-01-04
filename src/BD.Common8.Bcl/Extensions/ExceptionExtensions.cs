namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="Exception"/> 类型的扩展函数
/// </summary>
public static partial class ExceptionExtensions
{
    /// <summary>
    /// see inner exception
    /// </summary>
    public const string see_inner_exception = "see inner exception";

    /// <summary>
    /// 递归循环内部异常最大次数
    /// </summary>
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
            if (ex!.InnerException != null &&
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                ex.Message.Contains(see_inner_exception, StringComparison.OrdinalIgnoreCase))
#else
                ex.Message.ToLowerInvariant().Contains(see_inner_exception))
#endif
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
    /// 获取异常中所有错误信息
    /// </summary>
    /// <param name="e">当前捕获的异常</param>
    /// <param name="msg">可选的消息，将写在第一行</param>
    /// <param name="args">可选的消息参数</param>
    /// <returns></returns>
    public static string GetAllMessage(this Exception e, string? msg = null, params object?[] args)
    {
        var has_msg = !string.IsNullOrWhiteSpace(msg);
        var has_args = args != null && args.Length != 0;
        return GetAllMessageCore(e, has_msg, has_args, msg, args!);
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
                sb.AppendLine(exception.ToString());
            }
            exception = exception.InnerException;
        }

        var text = sb.ToString().Trim();
        return text;
    }
}