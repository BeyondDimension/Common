static partial class Utils
{
#if !SOURCE_GENERATOR
    public static readonly string ProjPath;
#endif

    static Utils()
    {
#if !SOURCE_GENERATOR
        ProjPath = GetProjectPath();
#endif
    }

    /// <summary>
    /// 获取当前项目绝对路径(.sln文件所在目录)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetProjectPath(string? path = null)
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