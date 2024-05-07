namespace Tools.Build.Commands;

/// <summary>
/// 服务端发布命令
/// </summary>
public interface IServerPublishCommand : ICommand
{
    /// <summary>
    /// 命令名
    /// </summary>
    const string CommandName = "spub";

    /// <inheritdoc cref="ICommand.GetCommand"/>
    static Command ICommand.GetCommand()
    {
        var no_err = new Option<bool>("--no_err");
        var push_name = new Option<string>("--push_name");
        var input = new Option<string>("--input", "发布的项目组，输入 a、all 发布全部，使用逗号分隔符");
        var push_domain = new Option<string>("--push_domain", "推送的域名");
        var push_only = new Option<bool>("--push_only");
        var tag_ver = new Option<string>("--tag_ver");
        var command = new Command(CommandName, "服务端发布命令")
        {
            no_err, push_name, input, push_domain, push_only, tag_ver,
        };
        command.SetHandler(Handler, no_err, push_name, input, push_domain, push_only, tag_ver);
        return command;
    }

    private const string fileNameServerPublishConfig = "server-publish.json";

    private static bool hasError = false;

    static async Task<int> Handler(bool no_err, string push_name, string input, string push_domain, bool push_only, string tag_ver)
    {
        int exitCode = 0;
        try
        {
            using CancellationTokenSource cts = new();
            cts.CancelAfter(TimeSpan.FromHours(2)); // 设置超时时间

            Console.CancelKeyPress += (_, _) =>
            {
                cts.Cancel();
            };

            await HandlerCore(push_name, input, push_domain, push_only, tag_ver, cts.Token);
        }
        finally
        {
            if (hasError)
            {
                if (!no_err)
                {
                    exitCode = 500;
                }
            }
            Console.WriteLine("🆗");
            Console.WriteLine("OK");
        }
        return exitCode;
    }

    private static readonly string projPath = ROOT_ProjPath;
    //private static readonly string projPath = @"C:\Repos\WTTS";

    private static async Task HandlerCore(string push_name, string input, string push_domain, bool push_only, string tag_ver, CancellationToken cancellationToken)
    {
        ServerPublishConfig? config = null;
        MemoryStream? configMemoryStream = null;
        try
        {
            using var configStream = new FileStream(Path.Combine(projPath, "src", fileNameServerPublishConfig), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            try
            {
                config = await SystemTextJsonSerializer.DeserializeAsync(configStream, AppJsonSerializerContext.Default.ServerPublishConfig, cancellationToken);
                configStream.Position = 0;
                await configStream.CopyToAsync(configMemoryStream = new(), cancellationToken);
            }
            catch
            {
            }
            config ??= new();

            try
            {
                await Publish(projPath, config, push_name, input, push_domain, push_only, tag_ver, cancellationToken);
            }
            finally
            {
                Stream? serializeStream = null;
                try
                {
                    if (configMemoryStream != null)
                    {
                        serializeStream = new MemoryStream();
                    }
                    else
                    {
                        configStream.Position = 0;
                        serializeStream = configStream;
                    }
                    await SystemTextJsonSerializer.SerializeAsync(serializeStream, config, AppJsonSerializerContext.Default.ServerPublishConfig, CancellationToken.None);
                    await serializeStream.FlushAsync(CancellationToken.None);
                    if (configMemoryStream != null)
                    {
                        // 与读取时的流内容对比，如果不一致则写入
                        if (!SequenceEqual(configMemoryStream, serializeStream))
                        {
                            serializeStream.Position = 0;
                            configStream.Position = 0;
                            await serializeStream.CopyToAsync(configStream, CancellationToken.None);
                            await configStream.FlushAsync(CancellationToken.None);
                        }
                    }
                }
                finally
                {
                    try
                    {
                        configStream?.SetLength(configStream.Position);
                    }
                    catch
                    {
                    }
                    serializeStream?.Dispose();
                }
            }

            static bool SequenceEqual(MemoryStream left, Stream right)
            {
                if (left.Length != right.Length)
                {
                    return false;
                }

                left.Position = 0;
                right.Position = 0;

                while (true)
                {
                    var l = left.ReadByte();
                    if (l == -1)
                    {
                        break;
                    }
                    if (l != right.ReadByte())
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        finally
        {
            configMemoryStream?.Dispose();
        }
    }

    private static readonly char[] separator = [',', '，', '\\', '、', '|'];

    private static async Task Publish(string projPath, ServerPublishConfig config, string push_name, string? input, string push_domain, bool push_only, string tag_ver, CancellationToken cancellationToken)
    {
        var projects = config.Projects;
        if (projects == null || projects.Length == 0)
        {
            Console.WriteLine("错误：未配置发布项目");
            return;
        }

        if (!string.IsNullOrWhiteSpace(input))
        {
            var array = input?.Split(separator, StringSplitOptions.RemoveEmptyEntries)?.Select(x => x.Trim())?.ToArray();
            if (array != null && array.Length != 0)
            {
                switch (array.FirstOrDefault()?.ToLowerInvariant())
                {
                    case "p":
                    case "push":
                        push_only = true;
                        break;
                }

                if (!array.Any(x => x.ToLowerInvariant() switch
                {
                    "a" or "all" => true,
                    _ => false,
                }))
                {
                    List<ServerPublishProject> list = new();
                    foreach (var item in array)
                    {
                        if (int.TryParse(item, out var index) && index >= 0 && index < projects.Length)
                        {
                            list.Add(projects[index]);
                            continue;
                        }
                        else
                        {
                            var eq_item = projects.FirstOrDefault(x =>
                            {
                                if (string.Equals(item, x.ProjectName, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }
                                if (string.Equals(item, x.DockerfileTag, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }
                                return false;
                            });
                            if (eq_item != null)
                            {
                                list.Add(eq_item);
                            }
                        }
                    }
                    if (list.Count != 0)
                    {
                        projects = list.ToArray();
                    }
                }
            }
        }

        var proj_datas = projects.ToDictionary(static x => x, y => y.GetData(projPath));

        if (!push_only)
        {
            foreach (var item in proj_datas)
            {
                var proj = item.Key;
                (var csprojPath, var publishPath, _, _) = item.Value;
                IOPath.DirTryDelete(publishPath);

                #region dotnet publish

                ProcessStartInfo psi = new()
                {
                    FileName = "dotnet",
                    WorkingDirectory = projPath,
                };

                psi.ArgumentList.Add("publish");

                var configuration = config.GetConfig();
                psi.ArgumentList.Add("-c");
                psi.ArgumentList.Add(configuration);

                psi.ArgumentList.Add(csprojPath);
                psi.ArgumentList.Add("--nologo");
                psi.ArgumentList.Add("-v");
                psi.ArgumentList.Add("q");
                psi.ArgumentList.Add("/property:WarningLevel=0");
                psi.ArgumentList.Add("-p:AnalysisLevel=none");
                psi.ArgumentList.Add("-p:GeneratePackageOnBuild=false");
                psi.ArgumentList.Add("-p:DebugType=none");
                psi.ArgumentList.Add("-p:DebugSymbols=false");
                psi.ArgumentList.Add("-p:IsPackable=false");
                psi.ArgumentList.Add("-p:GenerateDocumentationFile=false");
                psi.ArgumentList.Add("/nowarn:MSB4011,NU5048,NU5104,NU1506");
                psi.ArgumentList.Add("-maxcpucount");

                psi.ArgumentList.Add("-o");
                psi.ArgumentList.Add(publishPath);

                psi.ArgumentList.Add("-f");
                psi.ArgumentList.Add($"net{Environment.Version.Major}.{Environment.Version.Minor}");

                psi.ArgumentList.Add($"-p:SelfContained={config.SelfContained.ToLowerString()}");

                if (!config.PublishSingleFile)
                {
                    psi.ArgumentList.Add("-p:UseAppHost=false");
                }

                psi.ArgumentList.Add($"-p:PublishSingleFile={config.PublishSingleFile.ToLowerString()}");

                psi.ArgumentList.Add($"-p:PublishReadyToRun={config.PublishReadyToRun.ToLowerString()}");

                psi.ArgumentList.Add($"-p:RuntimeIdentifier={config.GetRuntimeIdentifier()}");

                //psi.ArgumentList.Add($"-p:AssemblyName={AssemblyName_ENTRYPOINT}");

#if DEBUG
                var publish_args = psi.FileName + ' ' + string.Join(' ', psi.ArgumentList);
                Debug.WriteLine(publish_args);
#endif

                var process = Process.Start(psi);
                if (process == null)
                    return;

                Console.WriteLine($"dotnet publish start({configuration})：{proj.ProjectName}");

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
                        Console.WriteLine($"dotnet publish end({configuration})：{proj.ProjectName}");
                    }
                    else
                    {
                        if (hasError != true)
                            hasError = true;
                        Console.WriteLine($"dotnet publish end({configuration})：{proj.ProjectName}, exitCode:{exitCode}");
                    }
                }
                finally
                {
                    KillProcess();
                }

                #endregion
            }

            #region docker build

            await ForEachAsync(proj_datas, cancellationToken, async (item, cancellationToken) =>
            {
                var proj = item.Key;
                (var csprojPath, _, var dockerfileTag, var dockerfileDirPath) = item.Value;

                var dockerfilePath = Path.Combine(dockerfileDirPath, "Dockerfile");
                using (var stream = new FileStream(dockerfilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await WriteDockerfile(stream, config, proj, csprojPath, cancellationToken);
                }

                ProcessStartInfo psi = new()
                {
                    FileName = "docker",
                    WorkingDirectory = projPath,
                    Arguments = $"build -t {dockerfileTag} -f {dockerfilePath} .",
                };

                var process = Process.Start(psi);
                if (process == null)
                    return;

                var configuration = config.GetConfig();
                Console.WriteLine($"docker build start({configuration})：{proj.ProjectName}");

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
                        Console.WriteLine($"docker build end({configuration})：{proj.ProjectName}");
                    }
                    else
                    {
                        if (hasError != true)
                            hasError = true;
                        Console.WriteLine($"docker build end({configuration})：{proj.ProjectName}, exitCode:{exitCode}");
                    }
                }
                finally
                {
                    KillProcess();
                }
            });

            #endregion

        }
        #region docker tag/push

        // 并行化推送镜像

        if (string.IsNullOrWhiteSpace(push_name))
        {
            push_name = "wtts";
        }

        string[]? domains = null;
        if (!string.IsNullOrWhiteSpace(push_domain))
        {
            var push_domain_split = push_domain.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (push_domain_split.Length != 0)
            {
                domains = push_domain_split;
            }
        }

        if (domains != null && domains.Length != 0)
        {
            if (string.IsNullOrWhiteSpace(tag_ver))
            {
                tag_ver = "latest";
            }

            var tag_with_push_array = domains.SelectMany(domain => proj_datas.ToArray().Select(y => KeyValuePair.Create(y.Key, (y.Value, domain)))).ToArray();

            await ForEachAsync(
                tag_with_push_array,
                cancellationToken,
                async (item, cancellationToken) =>
            {
                var proj = item.Key;
                (_, _, var dockerfileTag, _) = item.Value.Value;
                var domain = item.Value.domain;

                var tagProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"tag {dockerfileTag} {domain}/{push_name}/{dockerfileTag}:{tag_ver}",
                });
                tagProcess.ThrowIsNull();
                try
                {
                    await tagProcess.WaitForExitAsync(cancellationToken);
                }
                finally
                {
                    tagProcess.KillEntireProcessTree();
                }

                var pushProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"push {domain}/{push_name}/{dockerfileTag}:{tag_ver}",
                });
                pushProcess.ThrowIsNull();
                try
                {
                    await pushProcess.WaitForExitAsync(cancellationToken);
                }
                finally
                {
                    pushProcess.KillEntireProcessTree();
                }
            });
        }

        #endregion
    }

    private static async Task WriteDockerfile(Stream stream, ServerPublishConfig config, ServerPublishProject project, string csprojPath, CancellationToken cancellationToken)
    {
        var entryPoint = Path.GetFileName(csprojPath);
        entryPoint.ThrowIsNull();
        entryPoint = entryPoint.TrimEnd(".csproj", StringComparison.OrdinalIgnoreCase);
        stream.Write(
"""
# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
"""u8);

        if (project.InstallGoogleChrome)
        {
            stream.Write(
"""

RUN apt-get update
"""u8);
            stream.Write(
"""

COPY ["./ref/google-chrome-stable/google-chrome-stable_*_amd64.deb", "."]
"""u8);
            //            stream.WriteUtf16StrToUtf8OrCustom(
            //$"""

            //COPY ["{Path.Combine(projPath, "ref", "google-chrome-stable", "google-chrome-stable_*_amd64.deb").Replace(@"\", "/")}", "."]
            //""");
            stream.Write(
"""

RUN apt-get install -f -y ./google-chrome-stable_*_amd64.deb
RUN rm google-chrome-stable_*_amd64.deb

ENV PLAYWRIGHT_BROWSERS_PATH "/usr/bin/google-chrome-stable"
"""u8);
        }

        stream.Write(
"""

EXPOSE 80
EXPOSE 443
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(
$"""
        
COPY ["./src/artifacts/publish/{project.ProjectName}/output*", "/app"]
""");

        if (config.PublishSingleFile)
        {
            stream.WriteUtf16StrToUtf8OrCustom(
$"""

ENTRYPOINT ["{entryPoint}"]
""");
        }
        else
        {
            stream.WriteUtf16StrToUtf8OrCustom(
$"""

ENTRYPOINT ["dotnet", "{entryPoint}.dll"]
""");
        }
        await stream.FlushAsync(cancellationToken);
        try
        {
            stream.SetLength(stream.Position);
        }
        catch
        {
        }
    }

    private static async Task ForEachAsync<TSource>(IEnumerable<TSource> source, CancellationToken cancellationToken, Func<TSource, CancellationToken, ValueTask> body)
    {
        foreach (var item in source)
        {
            await body(item, cancellationToken);
        }
    }
}

sealed record class ServerPublishProject
{
    public string? ProjectName { get; set; }

    public string? DirName { get; set; }

    public string? DockerfileTag { get; set; }

    public bool InstallGoogleChrome { get; set; }

    internal (string csprojPath, string publishPath, string dockerfileTag, string dockerfileDirPath) GetData(string projPath)
    {
        var projDirName = DirName ?? ProjectName;
        projDirName.ThrowIsNull();

        var projName = ProjectName.ThrowIsNull();
        if (!projName.StartsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            projName += ".csproj";

        var csprojPath = Path.Combine(projPath, "src", projDirName, projName);
        var dockerfileDirPath = Path.Combine(projPath, "src", "artifacts", "publish", projDirName);
        var publishPath = Path.Combine(dockerfileDirPath, "output");

        var dockerfileTag = DockerfileTag;
        if (string.IsNullOrWhiteSpace(dockerfileTag))
        {
            using var fs = new FileStream(csprojPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            string? line;
            do
            {
                line = sr.ReadLine()?.Trim();
                if (line != null && line.StartsWith("<DockerfileTag>", StringComparison.OrdinalIgnoreCase))
                {
                    DockerfileTag = dockerfileTag = line
                        .TrimStart("<DockerfileTag>", StringComparison.OrdinalIgnoreCase)
                        .TrimEnd("</DockerfileTag>", StringComparison.OrdinalIgnoreCase)
                        .Trim();
                }
            } while (line != null);
        }

        return (csprojPath, publishPath, dockerfileTag.ThrowIsNull(), dockerfileDirPath);
    }
}

sealed record class ServerPublishConfig
{
    public ServerPublishProject[]? Projects { get; set; }

    public bool SelfContained { get; set; }

    public string? RuntimeIdentifier { get; set; }

    public bool PublishSingleFile { get; set; } = true;

    public bool PublishReadyToRun { get; set; } = true;

    public string? Config { get; set; }

    internal string GetConfig() => string.IsNullOrWhiteSpace(Config) ? "Release" : Config;

    internal string GetRuntimeIdentifier() => string.IsNullOrWhiteSpace(RuntimeIdentifier) ? "linux-x64" : RuntimeIdentifier;
}

[JsonSerializable(typeof(ServerPublishConfig))]
[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true)]
sealed partial class AppJsonSerializerContext : SystemTextJsonSerializerContext
{
}