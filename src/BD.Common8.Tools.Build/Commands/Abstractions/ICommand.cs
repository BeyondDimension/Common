namespace BD.Common8.Tools.Build.Commands.Abstractions;

/// <summary>
/// 命令行业务接口
/// </summary>
public interface ICommand
{
    /// <summary>
    /// 获取当前业务的命令行实例
    /// </summary>
    /// <returns></returns>
    internal static abstract Command GetCommand();

    /// <summary>
    /// 添加当前业务命令行到 <see cref="RootCommand"/>
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="rootCommand"></param>
    static void AddCommand<TCommand>(RootCommand rootCommand) where TCommand : ICommand
    {
        var command = TCommand.GetCommand();
        rootCommand.AddCommand(command);
    }
}