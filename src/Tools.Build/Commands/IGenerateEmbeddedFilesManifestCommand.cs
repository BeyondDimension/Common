using ManifestEmbeddedFileProvider = Microsoft.Extensions.FileProviders.ManifestEmbeddedFileProvider;

namespace Tools.Build.Commands;

/// <summary>
/// 生成清单嵌入式程序集
/// </summary>
interface IGenerateEmbeddedFilesManifestCommand : ICommand
{
    /// <summary>
    /// 命令名
    /// </summary>
    const string CommandName = "gefm";

    /// <inheritdoc cref="ICommand.GetCommand"/>
    static Command ICommand.GetCommand()
    {
        var path = new Option<string>("--path");
        var command = new Command(CommandName, "生成清单嵌入式程序集")
        {
            path,
        };
        command.SetHandler(Handler, path);
        return command;
    }

    private static async Task<int> Handler(string path)
    {
        HashSet<string> errors = new();
        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMinutes(11.5D)); // 设置超时时间
        var cancellationToken = cts.Token;

        // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/file-providers?view=aspnetcore-8.0#manifest-embedded-file-provider

        var fileNameWithoutEx = $"{CommandName}_{Hashs.String.Crc32(path)}_{Directory.GetParent(path)?.Name}";
        var csprojFilePath = Path.Combine(path, $"{fileNameWithoutEx}.csproj");

        using (var csproj = new FileStream(csprojFilePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            WriteCsproj(csproj, path);
            csproj.Flush();
            csproj.SetLength(csproj.Position);
        }

        var artifactsPath = Path.Combine(ROOT_ProjPath, "src", "artifacts");
        var directoryBuildFilePath = Path.Combine(path, "Directory.Build.props");
        using (var directoryBuild = new FileStream(directoryBuildFilePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            WriteDirectoryBuild(directoryBuild, path, artifactsPath);
            directoryBuild.Flush();
            directoryBuild.SetLength(directoryBuild.Position);
        }

        ProcessStartInfo psi = new()
        {
            FileName = "dotnet",
            Arguments = $"build -c Release {csprojFilePath} --nologo -v q /property:WarningLevel=0 -p:AnalysisLevel=none -p:GeneratePackageOnBuild=false /nowarn:MSB4011,NU5048,NU5104,NU1506 -maxcpucount",
            WorkingDirectory = ROOT_ProjPath,
        };
        var process = Process.Start(psi);
        process.ThrowIsNull();

        Console.WriteLine($"开始构建：{csprojFilePath}");

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
                Console.WriteLine($"构建成功{csprojFilePath}");
            }
            else
            {
                var err = $"构建失败{csprojFilePath}, exitCode:{exitCode}";
                Console.WriteLine(err);
                errors.Add(err);
            }
        }
        finally
        {
            KillProcess();
        }

        var dllPath = Path.Combine(artifactsPath, "bin", fileNameWithoutEx, "release", $"{fileNameWithoutEx}.dll");
        Console.WriteLine("dllPath:");
        Console.WriteLine(dllPath);

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
            Console.WriteLine("🆗");
            Console.WriteLine("OK");
            return (int)ExitCode.Ok;
        }
    }

    private static void WriteDirectoryBuild(Stream stream, string path, string artifactsPath)
    {
        stream.Write(
"""
<Project>
    <PropertyGroup>
        <UseArtifactsOutput>true</UseArtifactsOutput>
        <ArtifactsPath>
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(artifactsPath);
        stream.Write(
"""
</ArtifactsPath>
  </PropertyGroup>
</Project>
"""u8);
    }

    private static void WriteCsproj(Stream stream, string path)
    {
        stream.Write(
"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(Environment.Version.Major.ToString());
        stream.Write("."u8);
        stream.WriteUtf16StrToUtf8OrCustom(Environment.Version.Minor.ToString());
        stream.Write(
"""
</TargetFramework>
    <GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>
    <OutputType>Library</OutputType>
    <DebugType>none</DebugType>
    <Version>
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(DateTime.Now.ToString($"{Environment.Version.Major}.yy.1MMdd.HHmmss"));
        stream.Write(
"""
</Version>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.FileProviders.Embedded" Version="
"""u8);
        var version = typeof(ManifestEmbeddedFileProvider).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion.Split('+').First();
        stream.WriteUtf16StrToUtf8OrCustom(version);
        stream.Write(
"""
" />
  </ItemGroup>
  <ItemGroup>

"""u8);

        void WriteEmbeddedResource(string filePath)
        {
            var fileEx = Path.GetExtension(filePath).ToLowerInvariant();

            switch (fileEx)
            {
                case ".csproj" or ".props" or ".targets":
                    return;
            }

            stream.Write(
"""
    <EmbeddedResource Include="
"""u8);
            var include = Path.GetRelativePath(path, filePath);
            stream.WriteUtf16StrToUtf8OrCustom(include);
            stream.Write(
"""
" />

"""u8);
        }

        string[] files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            WriteEmbeddedResource(file);
        }
        string[] dirs = Directory.GetDirectories(path);
        while (dirs.Length != 0)
        {
            List<string[]> dirss = new();
            foreach (var dir in dirs)
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    WriteEmbeddedResource(file);
                }
                dirss.Add(Directory.GetDirectories(dir));
            }
            dirs = dirss.SelectMany(static x => x).ToArray();
        }

        stream.Write(
"""
  </ItemGroup>
</Project>
"""u8);
    }
}
