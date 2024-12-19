namespace System;

public static partial class IOPath
{
    /// <summary>
    /// 删除路径中的非法字符
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? CleanPathIlegalCharacter(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        var chars = CleanPathIlegalCharacterCore(path!).ToArray();
        if (chars.Length > 0)
            return new string(chars);

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IEnumerable<char> CleanPathIlegalCharacterCore(string path)
    {
        var invalidFileNameChars = Path.GetInvalidFileNameChars();
        var invalidPathChars = Path.GetInvalidPathChars();
        var chars = invalidFileNameChars.Concat(invalidPathChars);
        foreach (var it in path)
        {
            if (!chars.Contains(it))
            {
                yield return it;
            }
        }
    }

    /// <summary>
    /// 环境变量名称
    /// </summary>
    public static partial class EnvVarNames
    {
        /// <summary>
        /// %Documents%
        /// </summary>
        public const string MyDocuments = "%Documents%";

        /// <summary>
        /// %Music%
        /// </summary>
        public const string MyMusic = "%Music%";

        /// <summary>
        /// %Pictures%
        /// </summary>
        public const string MyPictures = "%Pictures%";

        /// <summary>
        /// %Videos%
        /// </summary>
        public const string MyVideos = "%Videos%";

        /// <summary>
        /// %StartMenu%
        /// </summary>
        public const string StartMenu = "%StartMenu%";

        /// <summary>
        /// %StartMenuProgramData%
        /// </summary>
        public const string StartMenuProgramData = "%StartMenuProgramData%";

        /// <summary>
        /// %StartMenuAppData%
        /// </summary>
        public const string StartMenuAppData = "%StartMenuAppData%";

        /// <summary>
        /// %SPP_AppData%
        /// </summary>
        public const string AppData = "%SPP_AppData%";

        /// <summary>
        /// %SPP_Cache%
        /// </summary>
        public const string Cache = "%SPP_Cache%";

        /// <summary>
        /// %Platform_Folder%
        /// </summary>
        public const string PlatformFolder = "%Platform_Folder%";
    }

    /// <summary>
    /// 将嵌入到指定字符串中的每个环境变量的名称替换为该变量的值的等效字符串，然后返回结果字符串。
    /// </summary>
    /// <param name="path"></param>
    /// <param name="platformFolder"></param>
    /// <param name="appDataDirectory"></param>
    /// <param name="cacheDirectory"></param>
    /// <param name="addAppDataDirectory"></param>
    /// <param name="addCacheDirectory"></param>
    /// <param name="action"></param>
    /// <returns></returns>
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
            variables.Add(EnvVarNames.AppData, appDataDirectory!);
        }

        if (addCacheDirectory)
        {
            if (string.IsNullOrEmpty(cacheDirectory))
                cacheDirectory = CacheDirectory;
            variables.Add(EnvVarNames.Cache, cacheDirectory!);
        }

        action?.Invoke(variables);

#if NETFRAMEWORK
        foreach (var it in variables)
        {
            var k = it.Key;
            var v = it.Value;
#else
        foreach (var (k, v) in variables)
        {
#endif
            path = path!.Replace(k, v);
        }

        if (!string.IsNullOrEmpty(platformFolder))
            path = path!.Replace(EnvVarNames.PlatformFolder, platformFolder);

        return Environment.ExpandEnvironmentVariables(path);
    }
}