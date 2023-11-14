using static BD.Common8.Tools.Build.Commands.IBuildCommand;

namespace BD.Common8.Tools.Build.Commands;

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
        var command = new Command(CommandName, "推送 NuGet 包命令")
        {
            // 之后可以添加一些参数，例如单独推送 NuGet 源和 GitHub 源或者其他的源
            token_github, token_nuget,
        };
        command.AddAlias("p"); // 单个字母的命令名简写
        command.SetHandler(Handler, token_github, token_nuget);
        return command;
    }

    /// <summary>
    /// 推送源
    /// </summary>
    enum PushSource : byte
    {
        NuGet,
        GitHub,
    }

    private record struct JobItem(PushSource PushSource, string PushFileName);

    /// <summary>
    /// 命令的逻辑实现
    /// </summary>
    /// <param name="token_github"></param>
    /// <param name="token_nuget"></param>
    /// <returns></returns>
    internal static async Task<int> Handler(string token_github, string token_nuget)
    {
        bool hasError = false;
        var repoPath = ProjPath;

        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMinutes(9.5D)); // 设置超时时间

        var pkgPath = Path.Combine(repoPath, "pkg");
        IEnumerable<string> GetPushFileNames(string projectName)
        {
            //// 版本号不确定，使用通配符匹配，在 build 命令时会删除 pkg 文件夹，所以不用考虑一个包有多个版本的情况
            var nupkg = $"{projectName}*.nupkg";
            var snupkg = $"{projectName}*.snupkg";
            nupkg = Directory.GetFiles(pkgPath, nupkg).FirstOrDefault();
            if (nupkg != null)
            {
                snupkg = Directory.GetFiles(pkgPath, snupkg).FirstOrDefault();
                if (snupkg == null) // snupkg 符号包可选，有就推，没有就忽略
                {
                    yield return nupkg;
                }
                else
                {
                    yield return nupkg;
                    yield return snupkg;
                }
            }
        }
        var jobs = projectNames.Select(GetPushFileNames).SelectMany(static x => x).ToArray();
        await Parallel.ForEachAsync(
            jobs.Select(static x => new JobItem(PushSource.NuGet, x))
            .Concat(jobs.Select(static x => new JobItem(PushSource.GitHub, x))),
            cts.Token, Handler); // 并行化推送相关项目

        if (hasError)
            return 500;
        Console.WriteLine("🆗");
        Console.WriteLine("OK");
        return 0;

        async ValueTask Handler(JobItem jobItem, CancellationToken cancellationToken)
        {
            const string url_nuget_push_github = "https://nuget.pkg.github.com/BeyondDimension";
            const string url_nuget_push_nuget = "https://api.nuget.org/v3/index.json";

            string url_nuget_push, token;
            switch (jobItem.PushSource)
            {
                case PushSource.GitHub:
                    url_nuget_push = url_nuget_push_github;
                    token = token_github;
                    break;
                case PushSource.NuGet:
                    url_nuget_push = url_nuget_push_nuget;
                    token = token_nuget;
                    break;
                default:
                    return;
            }
            if (string.IsNullOrWhiteSpace(token))
                return;

            ProcessStartInfo psi = new()
            {
                FileName = "dotnet",
                Arguments = $"nuget push {jobItem.PushFileName} -s {url_nuget_push} -k {token} --skip-duplicate",
                WorkingDirectory = pkgPath,
            };
            var process = Process.Start(psi);
            if (process == null)
                return;

            Console.WriteLine($"开始推送({jobItem.PushSource})：{jobItem.PushFileName}");

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
                    Console.WriteLine($"推送成功({jobItem.PushSource})：{jobItem.PushFileName}");
                }
                else
                {
                    if (hasError != true)
                        hasError = true;
                    Console.WriteLine($"推送失败({jobItem.PushSource})：{jobItem.PushFileName}");
                }
            }
            finally
            {
                KillProcess();
            }
        }
    }
}
