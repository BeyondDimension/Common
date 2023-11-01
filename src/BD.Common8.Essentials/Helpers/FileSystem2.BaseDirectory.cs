#if NET35
using WinFormsApplication = System.Windows.Forms.Application;
#endif

namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供了访问设备文件夹位置的简便方法。
/// </summary>
public sealed partial class FileSystem2 : IOPath.FileSystemBase
{
    private FileSystem2() => throw new NotSupportedException();

    /// <summary>
    /// 文件系统的基础文件夹路径
    /// </summary>
    public static class BaseDirectory
    {
        /// <summary>
        /// 获取应用程序的启动路径
        /// </summary>
        static string StartupPath
        {
            get
            {
                var appDataRootPath =
#if NET35
                WinFormsApplication.StartupPath;
#else
                IOPath.BaseDirectory;
#endif
                return appDataRootPath;
            }
        }

        /// <summary>
        /// 获取应用程序的 AppData 文件夹路径
        /// </summary>
        public static string AppDataDirectory
        {
            get
            {
                var appDataPath = Path.Combine(StartupPath, IOPath.DirName_AppData);
                return appDataPath;
            }
        }

        /// <summary>
        /// 获取应用程序的 Cache 文件夹路径
        /// </summary>
        public static string CacheDirectory
        {
            get
            {
                var cachePath = Path.Combine(StartupPath, IOPath.DirName_Cache);
                return cachePath;
            }
        }
    }
}