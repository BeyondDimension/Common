namespace Tools.Build.Commands;

/// <summary>
/// 构建当前仓库源代码命令
/// </summary>
interface IBuildCommand : ICommand
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

        return slnFileName switch
        {
            "BD.Common8" => [
                // 3.SourceGenerator
                "BD.Common8.SourceGenerator.ResX",
                //"BD.Common8.SourceGenerator.ResX.Test",
                "BD.Common8.SourceGenerator.Bcl",
                "BD.Common8.SourceGenerator.Bcl.Test",
                "BD.Common8.SourceGenerator.Ipc.Client",
                "BD.Common8.SourceGenerator.Ipc.Client.Test",
                "BD.Common8.SourceGenerator.Ipc.Server",
                "BD.Common8.SourceGenerator.Ipc.Server.Test",
                "BD.Common8.SourceGenerator.Shared",

                // AspNetCore
                "BD.Common8.AspNetCore",
                "BD.Common8.AspNetCore.Identity",
                "BD.Common8.AspNetCore.Identity.BackManage",

                // Bcl
                "BD.Common8.Bcl",
                "BD.Common8.Bcl.Compat",

                // Crawler
                "BD.Common8.Crawler",

                // Essentials
                "BD.Common8.Essentials",
                "BD.Common8.Essentials.Implementation",
                "BD.Common8.Essentials.Implementation.Avalonia",

                // Http
                "BD.Common8.Http.ClientFactory",
                "BD.Common8.Http.ClientFactory.Server",

                // HuaweiCloud

                // Ipc
                "BD.Common8.Ipc",
                "BD.Common8.Ipc.Client",
                "BD.Common8.Ipc.Server",

                // Orm
                "BD.Common8.Orm.EFCore",

                // Pinyin
                "BD.Common8.Pinyin",
                "BD.Common8.Pinyin.ChnCharInfo",
                "BD.Common8.Pinyin.CoreFoundation",

                // Primitives
                "BD.Common8.Primitives",
                "BD.Common8.Primitives.District",
                "BD.Common8.Primitives.PersonalData.BirthDate",
                "BD.Common8.Primitives.PersonalData.PhoneNumber",
                "BD.Common8.Primitives.Essentials",
                "BD.Common8.Primitives.Toast",
                "BD.Common8.Primitives.ApiRsp",
                "BD.Common8.Primitives.ApiResponse",

                // Repositories
                "BD.Common8.Repositories",
                "BD.Common8.Repositories.EFCore",
                "BD.Common8.Repositories.SQLitePCL",

                // Security
                "BD.Common8.Security",

                // Settings
                "BD.Common8.Settings5",

                // Sms
                "BD.Common8.SmsSender",

                // Toast
                "BD.Common8.Toast",

                // UserInput
                "BD.Common8.UserInput.ModelValidator",
            ],
            "BD.Avalonia8" => [
                "BD.Avalonia8.Image2",
            ],
            "BD.SteamClient8" => [
                "BD.SteamClient8",
                "BD.SteamClient8.Impl",
                "BD.SteamClient8.Models",
                "BD.SteamClient8.Primitives",
            ],
            _ => [],
        };
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
    }

    /// <summary>
    /// 命令的逻辑实现
    /// </summary>
    internal static async Task<int> Handler(bool test, bool no_err)
    {
        bool hasError = false;
        var repoPath = ProjPath;

        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMinutes(11.5D)); // 设置超时时间

        var pkgPath = Path.Combine(repoPath, "pkg");
        IOPath.DirTryDelete(pkgPath, true);

        var projectNames = GetProjectNames();

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

        if (hasError)
        {
            if (no_err)
            {
                return 0;
            }
            return 500;
        }
        Console.WriteLine("🆗");
        Console.WriteLine("OK");
        return 0;

        async ValueTask Handler(string projectName, string config, bool test, CancellationToken cancellationToken)
        {
            var projPath = Path.Combine(repoPath, "src", projectName);
            Clean(projPath);

            ProcessStartInfo psi = new()
            {
                FileName = "dotnet",
                Arguments = $"build -c {config} {projectName}.csproj --nologo -v q /property:WarningLevel=0 -p:AnalysisLevel=none{(test ? " -p:GeneratePackageOnBuild=false" : "")} /nowarn:MSB4011,NU5048,NU5104",
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
                    if (hasError != true)
                        hasError = true;
                    Console.WriteLine($"构建失败({config})：{projectName}, exitCode:{exitCode}");
                }
            }
            finally
            {
                KillProcess();
            }
        }
    }
}
