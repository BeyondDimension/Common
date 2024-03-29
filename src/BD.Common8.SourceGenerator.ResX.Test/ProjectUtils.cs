#pragma warning disable CA1050 // 在命名空间中声明类型

global using static ProjectUtils;

/// <summary>
/// 项目工具类
/// </summary>
public static partial class ProjectUtils
{
#if !SOURCE_GENERATOR
    /// <summary>
    /// 当前项目绝对路径
    /// </summary>
    public static readonly string ProjPath;

#if (NETFRAMEWORK && NET40_OR_GREATER) || !NETFRAMEWORK
    /// <summary>
    /// 当前项目的顶级绝对路径（通常作为子模块返回仓库的项目路径）
    /// </summary>
    public static readonly string ROOT_ProjPath;

    /// <summary>
    /// 用于测试的数据存储的路径
    /// </summary>
    public static readonly string DataPath = "";
#endif

    static ProjectUtils()
    {
        ProjPath = GetProjectPath();
#if (NETFRAMEWORK && NET40_OR_GREATER) || !NETFRAMEWORK
        ROOT_ProjPath = ProjPath;
        if (!string.IsNullOrWhiteSpace(ROOT_ProjPath))
        {
            var mROOT_ProjPath = ROOT_ProjPath;
            var mROOT_ProjPath2 = mROOT_ProjPath;
            while (true)
            {
                mROOT_ProjPath = Path.Combine(mROOT_ProjPath, "..");
                mROOT_ProjPath = GetProjectPath(mROOT_ProjPath);
                if (string.IsNullOrWhiteSpace(mROOT_ProjPath))
                {
                    ROOT_ProjPath = mROOT_ProjPath2;
                    break;
                }
                mROOT_ProjPath2 = mROOT_ProjPath;
            }
        }

        bool contains_actions_runner = false;
        if (!string.IsNullOrWhiteSpace(ROOT_ProjPath))
        {
            contains_actions_runner = ROOT_ProjPath.Contains("actions-runner");
            DataPath = contains_actions_runner ? Path.Combine(ROOT_ProjPath, "..", "..") : Path.Combine(ROOT_ProjPath, "..");
            DataPath = Path.GetFullPath(DataPath);
        }

        // https://docs.github.com/en/actions/learn-github-actions/variables#default-environment-variables
        //isCI = contains_actions_runner || (bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var result) && result);
#endif
    }
#endif

    /// <summary>
    /// 获取当前项目绝对路径(.sln文件所在目录)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetProjectPath(string? path = null)
    {
        path ??=
#if NET46_OR_GREATER || NETCOREAPP
        AppContext.BaseDirectory;
#else
        AppDomain.CurrentDomain.BaseDirectory;
#endif
        try
        {
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
#pragma warning disable SA1003 // Symbols should be spaced correctly
#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly
#pragma warning disable SA1110 // Opening parenthesis or bracket should be on declaration line
            if (!
#pragma warning restore SA1003 // Symbols should be spaced correctly
#if NET35
            Directory.GetFiles
#else
                Directory.EnumerateFiles
#endif
                (path, "*.sln").Any())
            {
                var parent = Directory.GetParent(path);
                if (parent == null)
                    return string.Empty;
                return GetProjectPath(parent.FullName);
            }
#pragma warning restore SA1110 // Opening parenthesis or bracket should be on declaration line
#pragma warning restore SA1008 // Opening parenthesis should be spaced correctly
#pragma warning restore RS1035 // 不要使用禁用于分析器的 API
#pragma warning restore IDE0079 // 请删除不必要的忽略
        }
        catch
        {
            return string.Empty;
        }
        return path;
    }
}