#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // Using 指令是不需要的。
global using static BD.Common8.ProjectUtils;
#pragma warning restore IDE0005 // Using 指令是不需要的。
#pragma warning restore IDE0079 // 请删除不必要的忽略

namespace BD.Common8;

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

    static ProjectUtils()
    {
        ProjPath = GetProjectPath();
        // https://docs.github.com/en/actions/learn-github-actions/variables#default-environment-variables
        isCI = bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var result) && result;
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

#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
    /// <summary>
    /// 当前目标框架 TFM
    /// </summary>
    public static readonly string tfm =
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
        $"net{Environment.Version.Major}.{Environment.Version.Minor}{tfm_}";

    /// <summary>
    /// 当前目标框架 TFM 后缀
    /// </summary>
    public const string tfm_ =
#if WINDOWS
    "-windows10.0.19041.0";
#elif LINUX
    "";
#elif MACCATALYST
    "-maccatalyst";
#elif MACOS
    "-macos";
#else
    "";
#endif

    static readonly bool isCI;

    /// <summary>
    /// 判断当前是否在 CI 中运行
    /// </summary>
    /// <returns></returns>
    public static bool IsCI() => isCI;
}