static class ProjPathHelper
{
    static readonly string projPath;

    static ProjPathHelper()
    {
        projPath = Utils.GetProjectPath();
    }

    public static string GetProjPath(string? _) => projPath;
}
