using SR = BD.Common8.Resources.SR;

namespace System;

/// <summary>
/// ThrowHelper 类是可用于高效引发异常的帮助程序类型。 它旨在支持 Guard API，并且应主要用于开发人员需要对引发的异常类型进行精细控制的情况，或者需要对要包含的确切异常消息进行精细控制。
/// </summary>
public static partial class ThrowHelper
{
    /// <summary>
    /// 抛出 <see cref="ArgumentNullException"/>
    /// </summary>
    /// <param name="paramName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [DoesNotReturn]
    public static void ThrowArgumentNullException(string? paramName)
        => throw new ArgumentNullException(paramName);

    /// <summary>
    /// 抛出异常 <see cref="ArgumentOutOfRangeException(string?, object?, string?)"/>，使用通用 message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="actualValue"></param>
    /// <param name="paramName"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException<T>(T actualValue, [CallerArgumentExpression(nameof(actualValue))] string? paramName = null) =>
          throw new ArgumentOutOfRangeException(SR.Arg_ArgumentOutOfRangeException__.Format(paramName, actualValue));

    /// <summary>
    /// 创建异常 <see cref="ArgumentOutOfRangeException(string?, object?, string?)"/>，使用通用 message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="actualValue"></param>
    /// <param name="paramName"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentOutOfRangeException GetArgumentOutOfRangeException<T>(T actualValue, [CallerArgumentExpression(nameof(actualValue))] string? paramName = null) =>
        new(SR.Arg_ArgumentOutOfRangeException__.Format(paramName, actualValue));

    /// <summary>
    /// 创建异常 <see cref="ArgumentOutOfRangeException(string?, object?, string?)"/>，使用自定义 message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="actualValue"></param>
    /// <param name="message"></param>
    /// <param name="paramName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentOutOfRangeException GetArgumentOutOfRangeWithMessageException<T>(T actualValue, string message, [CallerArgumentExpression(nameof(actualValue))] string? paramName = null) =>
        new(SR.Arg_ArgumentOutOfRangeException___.Format(paramName, actualValue, message));
}
