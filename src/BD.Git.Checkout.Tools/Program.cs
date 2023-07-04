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

    // https://docs.github.com/en/actions/learn-github-actions/contexts#env-context
    var github_workspace = GetArgument(0);
    if (string.IsNullOrWhiteSpace(github_workspace))
        throw new ArgumentOutOfRangeException(nameof(github_workspace));
    var github_sha = GetArgument(1);
    if (string.IsNullOrWhiteSpace(github_sha))
        throw new ArgumentOutOfRangeException(nameof(github_sha));
    var github_repositoryUrl = GetArgument(2);
    if (string.IsNullOrWhiteSpace(github_repositoryUrl))
        throw new ArgumentOutOfRangeException(nameof(github_repositoryUrl));
    ProcessStartInfo psi;
    if (!Directory.Exists(Path.Combine(github_workspace, ".git")))
    {
        psi = new()
        {
            FileName = "git",
            Arguments = $"clone {github_repositoryUrl}",
            WorkingDirectory = github_workspace,
        };
        Process.Start(psi)!.WaitForExit();
    }
    psi = new()
    {
        FileName = "git",
        Arguments = "fetch origin",
        WorkingDirectory = github_workspace,
    };
    Process.Start(psi)!.WaitForExit();
    psi = new()
    {
        FileName = "git",
        Arguments = $"checkout {github_sha}",
        WorkingDirectory = github_workspace,
    };
    Process.Start(psi)!.WaitForExit();
    psi = new()
    {
        FileName = "git",
        Arguments = $"submodule update --init --recursive",
        WorkingDirectory = github_workspace,
    };
    Process.Start(psi)!.WaitForExit();
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 500;
}