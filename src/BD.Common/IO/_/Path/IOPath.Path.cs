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

    public static partial class EnvVarNames
    {
        public const string MyDocuments = "%Documents%";
        public const string MyMusic = "%Music%";
        public const string MyPictures = "%Pictures%";
        public const string MyVideos = "%Videos%";
        public const string StartMenu = "%StartMenu%";
        public const string StartMenuProgramData = "%StartMenuProgramData%";
        public const string StartMenuAppData = "%StartMenuAppData%";
        public const string AppData = "%SPP_AppData%";
        public const string Cache = "%SPP_Cache%";
        public const string PlatformFolder = "%Platform_Folder%";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ExpandEnvironmentVariables(
        string? path,
        string? platformFolder = null,
        string? appDataDirectory = null,
        string? cacheDirectory = null,
        bool addAppDataDirectory = true,
        bool addCacheDirectory = false,
        Action<Dictionary<string, string>>? action = null)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        var variables = new Dictionary<string, string>
        {
            { EnvVarNames.MyDocuments, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
            { EnvVarNames.MyMusic, Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) },
            { EnvVarNames.MyPictures, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) },
            { EnvVarNames.MyVideos, Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) },
            { EnvVarNames.StartMenu, Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) },
            { EnvVarNames.StartMenuProgramData, Environment.ExpandEnvironmentVariables(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs")) },
            { EnvVarNames.StartMenuAppData, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs") },
        };

        if (addAppDataDirectory)
        {
            if (string.IsNullOrEmpty(appDataDirectory))
                appDataDirectory = AppDataDirectory;
            variables.Add(EnvVarNames.AppData, appDataDirectory);
        }

        if (addCacheDirectory)
        {
            if (string.IsNullOrEmpty(cacheDirectory))
                cacheDirectory = CacheDirectory;
            variables.Add(EnvVarNames.Cache, cacheDirectory);
        }

        action?.Invoke(variables);

        foreach (var (k, v) in variables)
            path = path.Replace(k, v);

        if (!string.IsNullOrEmpty(platformFolder))
            path = path.Replace(EnvVarNames.PlatformFolder, platformFolder);

        return Environment.ExpandEnvironmentVariables(path);
    }
}