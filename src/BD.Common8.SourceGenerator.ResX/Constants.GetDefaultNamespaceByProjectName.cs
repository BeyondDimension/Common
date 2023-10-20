namespace BD.Common8.SourceGenerator.ResX;

#pragma warning disable SA1600 // Elements should be documented

static partial class Constants
{
    public static string GetDefaultNamespaceByProjectName(string projectName)
    {
        projectName = projectName switch
        {
            "BD.Common8.Bcl" => "BD.Common8",
            _ => projectName,
        };
        return projectName.EndsWith(".Resources") ?
            projectName : $"{projectName}.Resources";
    }
}