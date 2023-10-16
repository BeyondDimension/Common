namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="ICommand"/> 类型的扩展函数
/// </summary>
public static partial class CommandExtensions
{
    /// <summary>
    /// 执行调用 <see cref="ICommand"/>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="parameter"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invoke(
        this ICommand command,
        object? parameter = null)
    {
        if (command.CanExecute(parameter))
            command.Execute(parameter);
    }
}