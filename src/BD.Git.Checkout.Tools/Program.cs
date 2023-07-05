using BD.Git.Checkout.Tools.Properties;

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

    static string Unprotect(byte[] encryptedData)
    {
#if WINDOWS
        var bytes = ProtectedData.Unprotect(encryptedData, Resources.hash2, DataProtectionScope.LocalMachine);
        var str = Encoding.UTF8.GetString(bytes);
        return str;
#else
        var str = Encoding.UTF8.GetString(encryptedData);
        return str;
#endif
    }

    ProcessStartInfo psi;
    Console.WriteLine(string.Join(' ', Environment.GetCommandLineArgs()));

    // https://docs.github.com/en/actions/learn-github-actions/contexts#env-context
    var github_workspace = GetArgument(0);
    if (string.IsNullOrWhiteSpace(github_workspace))
        throw new ArgumentOutOfRangeException(nameof(github_workspace));
    if (!Directory.Exists(github_workspace))
        Directory.CreateDirectory(github_workspace);
    var github_workspace_parent = Directory.GetParent(github_workspace)?.FullName;
    if (string.IsNullOrWhiteSpace(github_workspace_parent))
        throw new ArgumentOutOfRangeException(nameof(github_workspace_parent));
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

    string? proxy = default, github_auth_token = default;
    try
    {
        proxy = File.ReadAllText(Path.Combine(
            Path.GetDirectoryName(Environment.ProcessPath!)!,
            "checkout.proxy.txt"));
        Console.WriteLine("mark: proxyed");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
    try
    {
        github_auth_token = Unprotect(File.ReadAllBytes(Path.Combine(
            Path.GetDirectoryName(Environment.ProcessPath!)!,
            "checkout.t.txt")));
        Console.WriteLine("mark: ted");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    if (string.IsNullOrWhiteSpace(github_auth_token))
    {
        github_auth_token = Unprotect(Resources.value2);
    }

    Lazy<Dictionary<string, string>> environment = new(() =>
    {
        // file:///C:/Program%20Files/Git/mingw64/share/doc/git-doc/git-config.html
        var env = new Dictionary<string, string>
        {
            { "GIT_CONFIG_NOSYSTEM", "true" },
        };
        int count = 0, count_a = 0;
        if (!string.IsNullOrWhiteSpace(proxy))
        {
            count_a = count++;
            env.Add($"GIT_CONFIG_KEY_{count_a}", "http.proxy");
            env.Add($"GIT_CONFIG_VALUE_{count_a}", proxy);
            count_a = count++;
            env.Add($"GIT_CONFIG_KEY_{count_a}", "https.proxy");
            env.Add($"GIT_CONFIG_VALUE_{count_a}", proxy);
        }
        if (!string.IsNullOrWhiteSpace(github_auth_token))
        {
            // https://github.com/cirruslabs/cirrus-ci-docs/issues/407#issuecomment-530366488
            count_a = count++;
            var token_key = $"""
                url.https://{github_auth_token}@github.insteadOf
                """;
            env.Add($"GIT_CONFIG_KEY_{count_a}", token_key);
            env.Add($"GIT_CONFIG_VALUE_{count_a}", "https://github");
        }
        if (count > 0)
        {
            env.Add("GIT_CONFIG_COUNT", count.ToString());
        }
        return env;
    });

    ProcessStartInfo Git(ProcessStartInfo psi)
    {
        psi.FileName = "git";
        if (environment.Value.Any())
        {
            foreach (var env in environment.Value)
            {
                psi.Environment.Add(env!);
            }
        }
        return psi;
    }

    if (!Directory.Exists(Path.Combine(github_workspace, ".git")))
    {
        psi = Git(new()
        {
            Arguments = $"clone {github_repositoryUrl}",
            WorkingDirectory = github_workspace_parent,
        });
        Process.Start(psi)!.WaitForExit();
    }
    Console.WriteLine("mark: cloneed");

    if (!Directory.Exists(github_workspace))
    {
        Directory.CreateDirectory(github_workspace);
    }

    psi = Git(new()
    {
        Arguments = $"config --global --add safe.directory {github_workspace}",
        WorkingDirectory = github_workspace,
    });
    Process.Start(psi)!.WaitForExit();
    Console.WriteLine("mark: safeed");

    psi = Git(new()
    {
        Arguments = "fetch origin",
        WorkingDirectory = github_workspace,
    });
    Console.WriteLine("mark: fetched");
    Process.Start(psi)!.WaitForExit();
    psi = Git(new()
    {
        Arguments = $"checkout {github_sha}",
        WorkingDirectory = github_workspace,
    });
    Console.WriteLine("mark: checkouted");
    Process.Start(psi)!.WaitForExit();
    psi = Git(new()
    {
        Arguments = $"submodule update --init --recursive",
        WorkingDirectory = github_workspace,
    });
    Console.WriteLine("mark: submoduleed");
    Process.Start(psi)!.WaitForExit();
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 500;
}