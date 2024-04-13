namespace BD.Common8.SourceGenerator.Repositories.Helpers;

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
            projPath ??= GetProjectPath(path);
            return projPath;
        }
    }

    /// <summary>
    /// 获取当前项目绝对路径(.sln文件所在目录)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static string GetProjectPath(string? path = null)
    {
        path ??= AppContext.BaseDirectory;
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
        if (!Directory.EnumerateFiles(path, "*.sln").Any())
        {
            var parent = Directory.GetParent(path);
            if (parent == null) return string.Empty;
            return GetProjectPath(parent.FullName);
        }
#pragma warning restore RS1035 // 不要使用禁用于分析器的 API
        return path;
    }
}
