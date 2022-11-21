using System.Runtime;

// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    /// <summary>
    /// 获取当前应用程序的基目录文件夹路径
    /// </summary>
    public static string BaseDirectory => BaseDirectory_._BaseDirectory();

    /// <summary>
    /// 设置当前应用程序目录结构是否有 bin 文件夹的目录
    /// </summary>
    public static bool UseBinBaseDirectory
    {
        set => BaseDirectory_._BaseDirectory = value ?
            BinBaseDirectory_.DefaultBaseDirectory :
            BaseDirectory_.DefaultBaseDirectory;
    }

    public static void SetBaseDirectory(Func<string> value)
        => BaseDirectory_._BaseDirectory = value;

    static class BinBaseDirectory_
    {
        static readonly Lazy<string> _BaseDirectory = new(() =>
        {
            var value = AppContext.BaseDirectory;
            // 启用将发布 Host 入口点重定向到 Bin 目录中时重定向基目录
            if (OperatingSystem.IsWindows() && !DesktopBridge.IsRunningAsUwp)
            {
                var value2 = new DirectoryInfo(value);
                if (value2.Parent != null && string.Equals(value2.Name, "Bin", StringComparison.OrdinalIgnoreCase))
                {
                    value = value2.Parent.FullName;
                }
            }
            return value;
        });

        internal static readonly Func<string> DefaultBaseDirectory = () => _BaseDirectory.Value;
    }

    static class BaseDirectory_
    {
        internal static Func<string> _BaseDirectory;
        internal static readonly Func<string> DefaultBaseDirectory;

        static BaseDirectory_()
        {
            DefaultBaseDirectory = () => AppContext.BaseDirectory;
            _BaseDirectory = DefaultBaseDirectory;
        }
    }
}