// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    public const string DirName_AppData = "AppData";
    public const string DirName_Cache = "Cache";

    static Func<string>? getAppDataDirectory;
    static Func<string>? getCacheDirectory;

    /// <summary>
    /// 必须在 main 函数中初始化文件夹目录，否则将在使用时抛出此异常
    /// </summary>
    static Exception MustCallFileSystemInitException =>
        new NullReferenceException("msut call FileSystemXXX.InitFileSystem(..");

    /// <summary>
    /// 获取可存储应用程序数据的位置
    /// </summary>
    public static string AppDataDirectory
    {
        get
        {
            if (getAppDataDirectory != null)
                return getAppDataDirectory();
            throw MustCallFileSystemInitException;
        }
    }

    /// <summary>
    /// 获取可以存储临时数据的位置
    /// </summary>
    public static string CacheDirectory
    {
        get
        {
            if (getCacheDirectory != null)
                return getCacheDirectory();
            throw MustCallFileSystemInitException;
        }
    }

    static readonly object lock_GetCacheFilePath = new();

    /// <summary>
    /// 根据缓存子文件夹名称与文件扩展名获取一个缓存文件路径
    /// </summary>
    /// <param name="dirName">缓存子文件夹名称</param>
    /// <param name="fileNamePrefix">文件名前缀</param>
    /// <param name="fileEx">文件扩展名</param>
    public static string GetCacheFilePath(string dirName, string fileNamePrefix, string fileEx)
    {
        lock (lock_GetCacheFilePath)
        {
            var cacheDirPath = Path.Combine(CacheDirectory, dirName);
            if (!Directory.Exists(cacheDirPath))
            {
                Directory.CreateDirectory(cacheDirPath);
                return GetCacheFilePath();
            }
            else
            {
                string cacheFilePath;
                do
                {
                    cacheFilePath = GetCacheFilePath();
                } while (File.Exists(cacheFilePath));
                return cacheFilePath;
            }

            string GetCacheFilePath() => Path.Combine(cacheDirPath, GetCacheFileName());
            string GetCacheFileName() => $"{fileNamePrefix}_{DateTimeOffset.Now.Ticks}{Random2.GenerateRandomString(4)}{fileEx}";
        }
    }

    /// <summary>
    /// 尝试延时一段时间后删除文件
    /// </summary>
    /// <param name="filePath">要删除的文件路径</param>
    /// <param name="millisecondsDelay">延时等待的毫秒数</param>
    public static async void TryDeleteInDelay(string filePath, int millisecondsDelay = 9000)
    {
        await Task.Delay(millisecondsDelay);
        FileTryDelete(filePath);
    }

    /// <summary>
    /// 启动进程后尝试延时一段时间后删除文件
    /// </summary>
    /// <param name="process">启动的进程</param>
    /// <param name="filePath">要删除的文件路径</param>
    /// <param name="millisecondsDelay">延时等待的毫秒数</param>
    /// <param name="processWaitMillisecondsDelay">启动的进程等待退出的毫秒数</param>
    public static void TryDeleteInDelay(Process? process, string filePath, int millisecondsDelay = 9000, int processWaitMillisecondsDelay = 9000)
    {
        if (process != null)
        {
            var waitForExitResult = process.TryWaitForExit(processWaitMillisecondsDelay);
            if (!waitForExitResult)
            {
                try
                {
                    process.KillEntireProcessTree();
                }
                catch
                {

                }
                TryDeleteInDelay(filePath, millisecondsDelay);
                return;
            }
        }
        FileTryDelete(filePath);
    }

    /// <summary>
    /// 尝试根据缓存子文件夹名称删除整个缓存子文件夹
    /// </summary>
    /// <param name="dirName">缓存子文件夹名称</param>
    public static void TryDeleteCacheSubDir(string dirName)
    {
        var cacheDirPath = Path.Combine(CacheDirectory, dirName);
        DirTryDelete(cacheDirPath);
    }

    public abstract class FileSystemBase
    {
        protected FileSystemBase()
        {
        }

        /// <summary>
        /// 初始化文件系统
        /// </summary>
        /// <param name="getAppDataDirectory">获取应用目录文件夹</param>
        /// <param name="getCacheDirectory">获取缓存目录文件夹</param>
        protected static void InitFileSystem(Func<string> getAppDataDirectory, Func<string> getCacheDirectory)
        {
            IOPath.getAppDataDirectory = getAppDataDirectory;
            IOPath.getCacheDirectory = getCacheDirectory;
        }

        /// <summary>
        /// 带迁移的初始化文件系统，使用 <see cref="Directory.Move(string, string)"/> 或 xcopy 进行移动，如果迁移失败则回退源目录
        /// </summary>
        /// <param name="destAppDataPath">新的 AppData 文件夹路径</param>
        /// <param name="destCachePath">新的 Cache 文件夹路径</param>
        /// <param name="sourceAppDataPath">旧的 AppData 文件夹路径</param>
        /// <param name="sourceCachePath">旧的 Cache 文件夹路径</param>
        protected static void InitFileSystemWithMigrations(
            string destAppDataPath, string destCachePath,
            string sourceAppDataPath, string sourceCachePath)
        {
            bool ExistsNotEmptyDir(string path)
            {
                var exists = Directory.Exists(path);
                if (DesktopBridge.IsRunningAsUwp)
                {
                    if (path == destCachePath || path == sourceCachePath)
                    {
                        return false;
                    }
                }
                return exists && Directory.EnumerateFileSystemEntries(path).Any(); // 文件夹存在且不为空文件夹
            }

            var paths = new[] { destAppDataPath, destCachePath, };
            var dict_paths = paths.ToDictionary(x => x, x => ExistsNotEmptyDir(x));

            if (dict_paths.Values.All(x => !x))
            {
                var old_paths = new[] { sourceAppDataPath, sourceCachePath, };
                if (old_paths.All(x => Directory.Exists(x) && Directory.EnumerateFileSystemEntries(x).Any())) // 迁移之前根目录上的文件夹
                {
                    var isNotFirst = false;
                    for (int i = 0; i < old_paths.Length; i++)
                    {
                        var path = paths[i];
                        var old_path = old_paths[i];
                        try
                        {
                            if (!isNotFirst)
                            {
                                try
                                {
                                    // 尝试搜索之前版本的进程将其结束
                                    var currentProcess = Process.GetCurrentProcess();
                                    var query = from x in Process.GetProcessesByName(currentProcess.ProcessName)
                                                where x != currentProcess
                                                let m = x.TryGetMainModule()
                                                where m != null && m.FileName != currentProcess.TryGetMainModule()?.FileName
                                                select x;
                                    var process = query.ToArray();
                                    foreach (var proces in process)
                                    {
                                        try
                                        {
#if NETCOREAPP3_0_OR_GREATER
                                            proces.Kill(true);
#else
                                            proces.Kill();
#endif
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                isNotFirst = true;
                            }
                            MoveDirectory(old_path, path);
                            dict_paths[path] = true;
                        }
                        catch
                        {
                            if (!DesktopBridge.IsRunningAsUwp)
                            {
                                // 跨卷移动失败或其他原因失败，使用旧的目录，并尝试删除创建的空文件夹
                                DirTryDelete(path);
                            }
                            paths[i] = old_path;
                        }
                    }
                }
            }

            foreach (var item in dict_paths)
            {
                if (!item.Value)
                {
                    Directory.CreateDirectory(item.Key);
                }
            }

            InitFileSystem(GetAppDataDirectory, GetCacheDirectory);
            string GetAppDataDirectory() => paths[0];
            string GetCacheDirectory() => paths[1];
        }

        /// <summary>
        /// 初始化文件系统，但优先使用旧目录上的文件夹，如果存在的话(允许空文件夹)，不会进行文件迁移
        /// </summary>
        /// <param name="destAppDataPath">新的 AppData 文件夹路径</param>
        /// <param name="destCachePath">新的 Cache 文件夹路径</param>
        /// <param name="sourceAppDataPath">旧的 AppData 文件夹路径</param>
        /// <param name="sourceCachePath">旧的 Cache 文件夹路径</param>
        protected static void InitFileSystemUseDestFirst(
            string destAppDataPath, string destCachePath,
            string sourceAppDataPath, string sourceCachePath)
        {
            var paths = new[] { destAppDataPath, destCachePath, };
            var old_paths = new[] { sourceAppDataPath, sourceCachePath, };

            for (int i = 0; i < old_paths.Length; i++)
            {
                var item = old_paths[i];
                if (Directory.Exists(item))
                {
                    paths[i] = item;
                }
                else
                {
                    DirCreateByNotExists(paths[i]);
                }
            }

            InitFileSystem(GetAppDataDirectory, GetCacheDirectory);
            string GetAppDataDirectory() => paths[0];
            string GetCacheDirectory() => paths[1];
        }
    }

    /// <summary>
    /// (可选)初始化文件系统
    /// </summary>
    /// <param name="getAppDataDirectory">获取应用目录文件夹</param>
    /// <param name="getCacheDirectory">获取缓存目录文件夹</param>
    public static void InitFileSystem(Func<string> getAppDataDirectory, Func<string> getCacheDirectory)
    {
        IOPath.getAppDataDirectory = getAppDataDirectory;
        IOPath.getCacheDirectory = getCacheDirectory;
    }
}