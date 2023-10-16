using SR = BD.Common8.Resources.SR;

namespace System;

/// <summary>
/// ThrowHelper 类是可用于高效引发异常的帮助程序类型。 它旨在支持 Guard API，并且应主要用于开发人员需要对引发的异常类型进行精细控制的情况，或者需要对要包含的确切异常消息进行精细控制。
/// </summary>
public static partial class ThrowHelper
{
    /// <summary>
    /// 抛出异常 <see cref="ArgumentOutOfRangeException(string?, object?, string?)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="actualValue"></param>
    /// <param name="paramName"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException<T>(T actualValue, [CallerArgumentExpression(nameof(actualValue))] string? paramName = null) =>
          throw new ArgumentOutOfRangeException(SR.Arg_ArgumentOutOfRangeException__.Format(paramName, actualValue));
}
