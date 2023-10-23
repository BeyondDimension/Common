try
{
    Console.WriteLine($"Environment.Version: {Environment.Version}");
    // 从低版本的运行时中加载高版本的程序集
    var libPath = string.Join(Path.DirectorySeparatorChar.ToString(),
        new string[]
        {
            ProjPath.TrimEnd(Path.DirectorySeparatorChar),
            "src",
            "artifacts",
            "bin",
            "BD.Common8.AssemblyLoad.Sample.Library",
            "debug",
            "BD.Common8.AssemblyLoad.Sample.Library.dll",
        });
    var assembly = Assembly.LoadFile(libPath);
    var program = assembly.GetType("BD.Common8.AssemblyLoad.Sample.Library.Program", true);
    var main = program.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);
    var exitCode = Convert.ToInt32(main.Invoke(null, default, default, new object?[] { args }, null));
    Console.WriteLine($"ExitCode: {exitCode}");
}
catch (Exception e)
{
    // System.BadImageFormatException: 生成此程序集的运行时比当前加载的运行时新，无法加载此程序集。 (异常来自 HRESULT:0x8013101B)
    // 当前运行时不支持或者 app.config 没有正确配置使用 4.x 运行时
    Console.WriteLine(e);
}
finally
{
    Console.ReadLine();
}