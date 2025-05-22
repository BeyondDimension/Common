using System.Diagnostics;
using static Tools.Build.Commands.IBuildCommand;

namespace Tools.Build.Commands;

/// <summary>
/// 推送 NuGet 包命令
/// </summary>
interface INuGetPushCommand : ICommand
{
    /// <summary>
    /// 命令名
    /// </summary>
    const string CommandName = "push";

    /// <inheritdoc cref="ICommand.GetCommand"/>
    static Command ICommand.GetCommand()
    {
        var token_github = new Option<string>("--token_github", "secrets.RMBADMIN_TOKEN");
        var token_nuget = new Option<string>("--token_nuget", "secrets.NUGET_API_KEY");
        var token_local = new Option<string>("--token_local", "secrets.LOCAL_NUGET_API_KEY");
        var url_nuget_push_local = new Option<string>("--url_nuget_push_local", "secrets.LOCAL_NUGET_PUSH_URL");
        var command = new Command(CommandName, "推送 NuGet 包命令")
        {
            // 之后可以添加一些参数，例如单独推送 NuGet 源和 GitHub 源或者其他的源
            token_github, token_nuget, token_local, url_nuget_push_local,
        };
        command.AddAlias("p"); // 单个字母的命令名简写
        command.SetHandler(Handler, token_github, token_nuget, token_local, url_nuget_push_local);
        return command;
    }

    /// <summary>
    /// 推送源
    /// </summary>
    enum PushSource : byte
    {
        NuGet,
        GitHub,
        Local,
    }

    private record struct JobItem(PushSource PushSource, (string nupkg, string? snupkg) PushFileName);

    /// <summary>
    /// 命令的逻辑实现
    /// </summary>
    internal static async Task<int> Handler(string token_github, string token_nuget, string token_local, string url_nuget_push_local)
    {
        using CommandStopwatch sw = new(CommandName);

        HashSet<string> errors = new();
        var repoPath = ProjPath;

        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMinutes(9.5D)); // 设置超时时间

        var pkgPath = Path.Combine(repoPath, "pkg");
        IEnumerable<(string nupkg, string? snupkg)> GetPushFileNames(string projectName)
        {
            //// 版本号不确定，使用通配符匹配，在 build 命令时会删除 pkg 文件夹，所以不用考虑一个包有多个版本的情况
            var nupkg = $"{projectName}*.nupkg";
            var snupkg = $"{projectName}*.snupkg";
            nupkg = Directory.GetFiles(pkgPath, nupkg).FirstOrDefault();
            if (nupkg != null)
            {
                if (projectName.Contains("BD.Common8.SourceGenerator"))
                {
                    // 源生成器项目不需要符号包
                    yield return (nupkg, null);
                }
                else
                {
                    snupkg = Directory.GetFiles(pkgPath, snupkg).FirstOrDefault();
                    yield return (nupkg, snupkg);
                }
            }
        }
        var projectNames = GetProjectNames();
        var jobs = projectNames.Select(GetPushFileNames).SelectMany(static x => x).ToArray();
        var tasks = Enum.GetValues<PushSource>().Select(x => jobs.Select(y => new JobItem(x, y))).SelectMany(static x => x).ToArray();

        await Parallel.ForEachAsync(tasks,
            cts.Token, Handler); // 并行化推送相关项目

        if (errors.Count != 0)
        {
            Console.WriteLine("❌");
            Console.WriteLine("HasError");
            foreach (var err in errors)
            {
                Console.Error.WriteLine(err);
            }
            return (int)ExitCode.Exception;
        }
        else
        {
            sw.IsSuccess = true;
            Console.WriteLine("🆗");
            Console.WriteLine("OK");
            return (int)ExitCode.Ok;
        }

        static (string? url_nuget_push, string? token) GetPushUrlAndToken(PushSource pushSource, string token_github, string token_nuget, string token_local, string url_nuget_push_local)
        {
            string? url_nuget_push = null, token = null;
            switch (pushSource)
            {
                case PushSource.NuGet:
                    url_nuget_push = Constants.url_nuget_push_nuget;
                    token = token_github;
                    if (!string.IsNullOrWhiteSpace(Constants.token_nuget))
                    {
                        if (Constants.token_nuget_expire.HasValue)
                        {
                            if (DateTimeOffset.Now <= Constants.token_nuget_expire.Value)
                            {
                                // 使用常量值替换传参
                                token = Constants.token_nuget;
                            }
                        }
                    }
                    break;
                case PushSource.GitHub:
                    url_nuget_push = Constants.url_nuget_push_github;
                    token = token_github;
                    if (!string.IsNullOrWhiteSpace(Constants.token_github))
                    {
                        if (Constants.token_github_expire.HasValue)
                        {
                            if (DateTimeOffset.Now <= Constants.token_github_expire.Value)
                            {
                                // 使用常量值替换传参
                                token = Constants.token_github;
                            }
                        }
                    }
                    break;
                case PushSource.Local:
                    url_nuget_push = Constants.url_nuget_push_local;
                    if (string.IsNullOrWhiteSpace(url_nuget_push))
                    {
                        url_nuget_push = url_nuget_push_local;
                    }
                    token = token_local;
                    if (!string.IsNullOrWhiteSpace(Constants.token_local))
                    {
                        if (Constants.token_local_expire.HasValue)
                        {
                            if (DateTimeOffset.Now <= Constants.token_local_expire.Value)
                            {
                                // 使用常量值替换传参
                                token = Constants.token_local;
                            }
                        }
                    }
                    break;
            }
            return (url_nuget_push, token);
        }

        async ValueTask Handler(JobItem jobItem, CancellationToken cancellationToken)
        {
            (var url_nuget_push, var token) = GetPushUrlAndToken(jobItem.PushSource, token_github, token_nuget, token_local, url_nuget_push_local);
            if (string.IsNullOrWhiteSpace(url_nuget_push) || string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            string[] pushFileNames = string.IsNullOrWhiteSpace(jobItem.PushFileName.snupkg)
                ? [jobItem.PushFileName.nupkg]
                : [jobItem.PushFileName.nupkg, jobItem.PushFileName.snupkg];

            foreach (var pushFileName in pushFileNames)
            {
                ProcessStartInfo psi = new()
                {
                    FileName = "dotnet",
                    Arguments = $"nuget push {pushFileName} -s {url_nuget_push} -k {token} --skip-duplicate",
                    WorkingDirectory = pkgPath,
                };
                var process = Process.Start(psi);
                if (process == null)
                    return;

                Console.WriteLine($"开始推送({jobItem.PushSource})：{pushFileName}");

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
                        Console.WriteLine($"推送成功({jobItem.PushSource})：{pushFileName}");
                    }
                    else
                    {
                        var err = $"推送失败({jobItem.PushSource})：{pushFileName}";
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
    }
}
