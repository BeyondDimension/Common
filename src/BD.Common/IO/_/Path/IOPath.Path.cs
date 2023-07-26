// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    /// <summary>
    /// 删除路径中的非法字符
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? CleanPathIlegalCharacter(string? f)
    {
        if (string.IsNullOrEmpty(f))
            return f;

        var invalidFileNameChars = Path.GetInvalidFileNameChars();
        var invalidPathChars = Path.GetInvalidPathChars();
        var regexSearch = new string(invalidFileNameChars.Concat(invalidPathChars).ToArray());

        var r = new Regex($"[{Regex.Escape(regexSearch)}]");
        return r.Replace(f, "");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ExpandEnvironmentVariables(
        string? path,
        string? platformFolder = null)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        var variables = new Dictionary<string, string>
            {
                //{ "%SPP_UserData%", IOPath.CacheDirectory },
                { "%SPP_AppData%", IOPath.AppDataDirectory },
                { "%Documents%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
                { "%Music%", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) },
                { "%Pictures%", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) },
                { "%Videos%", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) },
                { "%StartMenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) },
                { "%StartMenuProgramData%", Environment.ExpandEnvironmentVariables(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs")) },
                { "%StartMenuAppData%", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs") }
            };

        foreach (var (k, v) in variables)
            path = path.Replace(k, v);

        if (!string.IsNullOrEmpty(platformFolder))
            path = path.Replace("%Platform_Folder%", platformFolder);

        return Environment.ExpandEnvironmentVariables(path);
    }
}