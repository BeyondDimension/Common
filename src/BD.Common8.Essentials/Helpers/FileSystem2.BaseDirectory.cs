#if NET35
using WinFormsApplication = System.Windows.Forms.Application;
#endif

namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 提供了访问设备文件夹位置的简便方法。
/// </summary>
public sealed partial class FileSystem2 : IOPath.FileSystemBase
{
    private FileSystem2() => throw new NotSupportedException();

    public static class BaseDirectory
    {
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

        public static string AppDataDirectory
        {
            get
            {
                var appDataPath = Path.Combine(StartupPath, IOPath.DirName_AppData);
                return appDataPath;
            }
        }

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