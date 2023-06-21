namespace BD.Common.Repositories.SourceGenerator.Helpers;

/// <summary>
/// 项目路径助手类
/// </summary>
static class ProjPathHelper
{
    static readonly object @lock = new();

    static string? projPath;

    /// <summary>
    /// 获取当前项目绝对路径(.sln文件所在目录)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetProjPath(string? path)
    {
        if (projPath != null)
            return projPath;

        if (path == null)
            throw new ArgumentNullException(nameof(path));

        lock (@lock)
        {
            projPath ??= Utils.GetProjectPath(path);
            return projPath;
        }
    }
}
