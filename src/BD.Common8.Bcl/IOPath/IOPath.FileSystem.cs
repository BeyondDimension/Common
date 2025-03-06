namespace System;

public static partial class IOPath
{
    /// <summary>
    /// AppData
    /// </summary>
    public const string DirName_AppData = "AppData";

    /// <summary>
    /// Cache
    /// </summary>
    public const string DirName_Cache = "Cache";

    static Func<string>? getAppDataDirectory;
    static Func<string>? getCacheDirectory;

    /// <summary>
    /// 必须在 main 函数中初始化文件夹目录，否则将在使用时抛出此异常
    /// </summary>
    static Exception MustCallFileSystemInitException =>
        new NullReferenceException("must call FileSystemXXX.InitFileSystem(..");

    /// <summary>
    /// 获取应用程序数据的位置
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
    /// 获取缓存数据的位置
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

#if !NETFRAMEWORK

    static readonly System.Threading.Lock lock_GetCacheFilePath = new();

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
    public static async Task TryDeleteInDelayAsync(string filePath, int millisecondsDelay = 9000)
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
    public static async Task TryDeleteInDelayAsync(Process? process, string filePath, int millisecondsDelay = 9000, int processWaitMillisecondsDelay = 9000)
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
                await TryDeleteInDelayAsync(filePath, millisecondsDelay);
                return;
            }
        }
        FileTryDelete(filePath);
    }

#endif

    /// <summary>
    /// 尝试根据缓存子文件夹名称删除整个缓存子文件夹
    /// </summary>
    /// <param name="dirName">缓存子文件夹名称</param>
    public static void TryDeleteCacheSubDir(string dirName)
    {
        var cacheDirPath = Path.Combine(CacheDirectory, dirName);
        DirTryDelete(cacheDirPath);
    }

    /// <summary>
    /// 初始化文件系统抽象类
    /// </summary>
    public abstract class FileSystemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemBase"/> class.
        /// </summary>
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