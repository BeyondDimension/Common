try
{
    Console.OutputEncoding = Encoding.UTF8;
    const string rootCommandDesc = "BeyondDimension Build Tools";
    var rootCommand = new RootCommand(rootCommandDesc);
    // 根据命令行业务接口反射当前程序集查找所有实现循环添加
    var interfaceType = typeof(ICommand);
    var addMethod = interfaceType.GetMethod(nameof(ICommand.AddCommand),
        BindingFlags.Static | BindingFlags.Public);
    var commands = interfaceType.Assembly.GetTypes().
        Where(x => x != interfaceType && x.IsInterface && interfaceType.IsAssignableFrom(x)).
        Select(x => addMethod!.MakeGenericMethod(x)).
        ToArray();
    Array.ForEach(commands, m => m.Invoke(null, [rootCommand,]));
    var exitCode = await rootCommand.InvokeAsync(args);
    Console.Write("exitCode: ");
    Console.WriteLine(exitCode);
    return exitCode;
}
catch (Exception ex)
{
    Console.WriteLine("catch: ");
    Console.WriteLine(ex);
    return (int)ExitCode.Exception;
}

/// <summary>
/// 进程退出状态码
/// </summary>
enum ExitCode
{
    // https://tldp.org/LDP/abs/html/exitcodes.html

    Ok = 0,

    /// <summary>
    /// 出现 <see cref="System.Exception"/>
    /// </summary>
    Exception = 64,
}