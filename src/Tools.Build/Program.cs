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
    return exitCode;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 500;
}