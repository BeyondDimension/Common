try
{
    string? GetArgument(int index)
    {
        try
        {
            return args[index];
        }
        catch
        {
            return default;
        }
    }

    ProcessStartInfo psi;
    Console.WriteLine(string.Join(' ', Environment.GetCommandLineArgs()));

    // https://docs.github.com/en/actions/learn-github-actions/contexts#env-context
    var github_workspace = GetArgument(0);
    if (string.IsNullOrWhiteSpace(github_workspace))
        throw new ArgumentOutOfRangeException(nameof(github_workspace));
    if (!Directory.Exists(github_workspace))
        Directory.CreateDirectory(github_workspace);
    var github_sha = GetArgument(1);
    if (string.IsNullOrWhiteSpace(github_sha))
        throw new ArgumentOutOfRangeException(nameof(github_sha));
    var github_repositoryUrl = GetArgument(2);
    if (string.IsNullOrWhiteSpace(github_repositoryUrl))
        throw new ArgumentOutOfRangeException(nameof(github_repositoryUrl));
    github_repositoryUrl = github_repositoryUrl.Replace(
        "git://",
        "https://",
        StringComparison.OrdinalIgnoreCase);

    try
    {
        var proxy = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath!)!, "checkout.proxy.txt"));
        if (!string.IsNullOrWhiteSpace(proxy))
        {
            Console.WriteLine($"proxy: {proxy}");
            psi = new()
            {
                FileName = "git",
                Arguments = $"config --global http.proxy {proxy}",
                WorkingDirectory = github_workspace,
            };
            Process.Start(psi)!.WaitForExit();
            psi = new()
            {
                FileName = "git",
                Arguments = $"config --global https.proxy {proxy}",
                WorkingDirectory = github_workspace,
            };
            Process.Start(psi)!.WaitForExit();
        }
        Console.WriteLine("mark: proxyed");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    if (!Directory.Exists(Path.Combine(github_workspace, ".git")))
    {
        psi = new()
        {
            FileName = "git",
            Arguments = $"clone {github_repositoryUrl}",
            WorkingDirectory = Directory.GetParent(github_workspace)!.FullName,
        };
        Process.Start(psi)!.WaitForExit();
    }
    Console.WriteLine("mark: cloneed");

    if (!Directory.Exists(github_workspace))
    {
        Directory.CreateDirectory(github_workspace);
    }

    psi = new()
    {
        FileName = "git",
        Arguments = $"config --global --add safe.directory {github_workspace}",
        WorkingDirectory = github_workspace,
    };
    Process.Start(psi)!.WaitForExit();
    Console.WriteLine("mark: safeed");

    psi = new()
    {
        FileName = "git",
        Arguments = "fetch origin",
        WorkingDirectory = github_workspace,
    };
    Console.WriteLine("mark: fetched");
    Process.Start(psi)!.WaitForExit();
    psi = new()
    {
        FileName = "git",
        Arguments = $"checkout {github_sha}",
        WorkingDirectory = github_workspace,
    };
    Console.WriteLine("mark: checkouted");
    Process.Start(psi)!.WaitForExit();
    psi = new()
    {
        FileName = "git",
        Arguments = $"submodule update --init --recursive",
        WorkingDirectory = github_workspace,
    };
    Console.WriteLine("mark: submoduleed");
    Process.Start(psi)!.WaitForExit();
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 500;
}