// ReSharper disable once CheckNamespace
namespace System;

public static class ExceptionExtensions
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
}
