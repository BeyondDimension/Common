#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
namespace BD.Common8.SourceGenerator.Repositories.Helpers;

/// <summary>
/// 项目路径助手类
/// </summary>
static class ProjPathHelper
{
    static readonly object @lock = new();

    static string? projPath;

    /// <summary>
    /// 获取当前项目绝对路径(.sln|.slnx文件所在目录)
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

    static IEnumerable<string> EnumerateSlnFiles(string path)
    {
        foreach (var it in Directory.EnumerateFiles(path))
        {
            if (it.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
            {
                yield return it;
            }
            else if (it.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase))
            {
                yield return it;
            }
        }
    }

    /// <summary>
    /// 获取当前项目绝对路径(.sln|.slnx文件所在目录)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static string GetProjectPath(string? path = null)
    {
        path ??= AppContext.BaseDirectory;
        if (!EnumerateSlnFiles(path).Any())
        {
            var parent = Directory.GetParent(path);
            if (parent == null) return string.Empty;
            return GetProjectPath(parent.FullName);
        }
        return path;
    }
}
