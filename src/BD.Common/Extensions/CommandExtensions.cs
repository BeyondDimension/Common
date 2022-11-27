using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace System;

public static class CommandExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invoke(this ICommand command, object? parameter = null)
    {
        if (command.CanExecute(parameter))
        {
            command.Execute(parameter);
        }
    }
}