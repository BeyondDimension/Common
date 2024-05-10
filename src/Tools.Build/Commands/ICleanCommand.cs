namespace Tools.Build.Commands;

/// <summary>
/// 清理 bin、obj 文件夹命令
/// </summary>
partial interface ICleanCommand : ICommand
{
    /// <summary>
    /// 命令名
    /// </summary>
    const string CommandName = "clean";

    /// <inheritdoc cref="ICommand.GetCommand"/>
    static Command ICommand.GetCommand()
    {
        var command = new Command(CommandName, "清理 bin、obj 文件夹命令")
        {
        };
        command.SetHandler(Handler);
        return command;
    }

    /// <summary>
    /// 命令的逻辑实现
    /// </summary>
    internal static void Handler()
    {
        try
        {
            var srcPath = Path.Combine(ROOT_ProjPath, "src");
            if (Directory.Exists(srcPath))
            {
                Directory.GetDirectories(srcPath).ForEach(x =>
                {
                    Array.ForEach(["bin", "obj"], y =>
                    {
                        var path = Path.Combine(x, y);
                        if (Directory.Exists(path))
                        {
                            Console.WriteLine($"del: {path}");
                            IOPath.DirTryDelete(path, true);
                        }
                    });
                });
            }
        }
        finally
        {
            Console.WriteLine("Clean completed.");
        }
    }
}
