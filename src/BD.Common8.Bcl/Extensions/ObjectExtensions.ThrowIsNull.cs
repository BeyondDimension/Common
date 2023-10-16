namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="object"/> 类型的扩展函数
/// </summary>
public static partial class ObjectExtensions
{
    /// <summary>
    /// 判断变量参数为 <see langword="null"/> 时引发 <see cref="ArgumentNullException"/> 异常
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argument"></param>
    /// <param name="paramName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIsNull<T>([NotNull] this T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) where T : class
    {
        // https://github.com/dotnet/runtime/blob/v6.0.5/src/libraries/System.Private.CoreLib/src/System/ArgumentNullException.cs#L59
        // https://github.com/CommunityToolkit/dotnet/blob/v8.0.0-preview3/CommunityToolkit.Diagnostics/Guard.cs#L63
        if (argument is null)
            Throw(paramName);
        return argument;
    }

    [DoesNotReturn]
    static void Throw(string? paramName)
        => throw new ArgumentNullException(paramName);
}