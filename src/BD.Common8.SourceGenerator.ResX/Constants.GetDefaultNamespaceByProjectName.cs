namespace BD.Common8.SourceGenerator.ResX;

static partial class Constants
{
    /// <summary>
    /// 根据项目名称获取默认的命名空间
    /// </summary>
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