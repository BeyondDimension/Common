namespace Tools.Build.Commands;

/// <summary>
/// 构建当前仓库源代码命令
/// </summary>
partial interface IBuildCommand : ICommand
{
    /// <summary>
    /// 命令名
    /// </summary>
    const string CommandName = "build";

    /// <inheritdoc cref="ICommand.GetCommand"/>
    static Command ICommand.GetCommand()
    {
        var test = new Option<bool>("--test");
        var no_err = new Option<bool>("--no_err");
        var command = new Command(CommandName, "构建当前仓库源代码命令")
        {
            test, no_err,
        };
        command.AddAlias("b"); // 单个字母的命令名简写
        command.SetHandler(Handler, test, no_err);
        return command;
    }

    /// <summary>
    /// 获取要构建的项目名称
    /// </summary>
    /// <returns></returns>
    static string[] GetProjectNames()
    {
        var slnFileName = Directory.GetFiles(ProjPath, "*.sln").FirstOrDefault();
        slnFileName.ThrowIsNull();
        slnFileName = Path.GetFileNameWithoutExtension(slnFileName);

        return GetProjectNamesCore(slnFileName);
    }

    /// <summary>
    /// 根据项目路径清理 bin/obj 文件夹
    /// </summary>
    /// <param name="projPath"></param>
    private static void Clean(string projPath)
    {
        // 如果用 < .NET 8 SDK 的 VS 打开过项目将导致在项目目录下产生 bin/obj 文件会影响编译
        // 先尝试清理目录再执行构建
        Array.ForEach(["bin", "obj"], x =>
        {
            var path = Path.Combine(projPath, x);
            IOPath.DirTryDelete(path, true);
        });

        // FodyWeavers.xsd 文件会导致编译错误
        // MSBUILD : error : Fody: Failed to update schema for (src\BD.Common8.Essentials.Implementation.Avalonia\FodyWeavers.xml).
        // Exception message: The process cannot access the file 'src\BD.Common8.Essentials.Implementation.Avalonia\FodyWeavers.xsd' because it is being used by another process.
        var xsdFilePath = Path.Combine(projPath, "FodyWeavers.xsd");
        if (File.Exists(xsdFilePath))
        {
            File.Delete(xsdFilePath);
        }
    }

    /// <summary>
    /// 命令的逻辑实现
    /// </summary>
    internal static async Task<int> Handler(bool test, bool no_err)
    {
        try
        {
            HashSet<string> errors = new();
            var repoPath = ProjPath;

            var projectNames = GetProjectNames();

            using CancellationTokenSource cts = new();
            cts.CancelAfter(TimeSpan.FromMinutes(11.5D * projectNames.Length)); // 设置超时时间

            var pkgPath = Path.Combine(repoPath, "pkg");
            IOPath.DirTryDelete(pkgPath, true);

            //await Handler("BD.Common8.SourceGenerator.ResX", cts.Token);
            //await Parallel.ForEachAsync(projectNames, cts.Token, Handler); // 并行化构建相关项目

            string[] configs = test ? ["Release", "Debug"] : ["Release"];
            foreach (var config in configs)
            {
                foreach (var projectName in projectNames)
                {
                    // 顺序循环构建相关项目，并行化会导致文件占用冲突
                    await Handler(projectName, config, test, cts.Token);
                }
            }

            if (errors.Count != 0)
            {
                Console.WriteLine("❌");
                Console.WriteLine("HasError");
                foreach (var err in errors)
                {
                    Console.Error.WriteLine(err);
                }
                if (no_err)
                {
                    return (int)ExitCode.Ok;
                }
                return (int)ExitCode.Exception;
            }
            else
            {
                Console.WriteLine("🆗");
                Console.WriteLine("OK");
                return (int)ExitCode.Ok;
            }

            async ValueTask Handler(string projectName, string config, bool test, CancellationToken cancellationToken)
            {
                var projPath = Path.Combine(repoPath, "src", projectName);
                Clean(projPath);

                ProcessStartInfo psi = new()
                {
                    FileName = "dotnet",
                    Arguments = $"build -c {config} {projectName}.csproj --nologo -v q /property:WarningLevel=0 -p:AnalysisLevel=none{(test ? " -p:GeneratePackageOnBuild=false" : "")} /nowarn:MSB4011,NU5048,NU5104,NU1506 -maxcpucount",
                    WorkingDirectory = projPath,
                };
                var process = Process.Start(psi);
                if (process == null)
                    return;

                Console.WriteLine($"开始构建({config})：{projectName}");

                bool isKillProcess = false;
                void KillProcess()
                {
                    if (isKillProcess)
                        return;

                    isKillProcess = true;

                    try
                    {
                        process.Kill(true); // 避免进程残留未结束
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                try
                {
                    cancellationToken.Register(KillProcess);

                    await process.WaitForExitAsync(cancellationToken);

                    int exitCode = -1;
                    try
                    {
                        exitCode = process.ExitCode;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    if (exitCode == 0)
                    {
                        Console.WriteLine($"构建成功({config})：{projectName}");
                    }
                    else
                    {
                        var err = $"构建失败({config})：{projectName}, exitCode:{exitCode}";
                        Console.WriteLine(err);
                        errors.Add(err);
                    }
                }
                finally
                {
                    KillProcess();
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return (int)ExitCode.Exception;
        }
    }
}
