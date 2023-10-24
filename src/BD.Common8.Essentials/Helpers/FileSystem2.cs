namespace BD.Common8.Essentials.Helpers;

partial class FileSystem2
{
    /// <summary>
    /// 初始化文件系统
    /// </summary>
    public static void InitFileSystem()
    {
        var appDataPath = BaseDirectory.AppDataDirectory;
        var cachePath = BaseDirectory.CacheDirectory;
        IOPath.DirCreateByNotExists(appDataPath);
        IOPath.DirCreateByNotExists(cachePath);
        InitFileSystem(GetAppDataDirectory, GetCacheDirectory);
        string GetAppDataDirectory() => appDataPath;
        string GetCacheDirectory() => cachePath;
    }

#if DEBUG
    /// <inheritdoc cref="IOPath.AppDataDirectory"/>
    [Obsolete("use IOPath.AppDataDirectory", true)]
    public static string AppDataDirectory => IOPath.AppDataDirectory;

    /// <inheritdoc cref="IOPath.CacheDirectory"/>
    [Obsolete("use IOPath.CacheDirectory", true)]
    public static string CacheDirectory => IOPath.CacheDirectory;
#endif
}
