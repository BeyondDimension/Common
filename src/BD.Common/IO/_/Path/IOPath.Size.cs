// ReSharper disable once CheckNamespace
namespace System;

partial class IOPath
{
    const double unit_double = 1024D;

    static readonly string[] units = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB", };

    /// <summary>
    /// 获取用于显示的文件大小
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static (double length, string unit) GetDisplayFileSize(double length)
    {
        if (length > 0D)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (i > 0) length /= unit_double;
                if (length < unit_double) return (length, units[i]);
            }
            return (length, units.Last());
        }
        else
        {
            return (0D, units.First());
        }
    }

    /// <summary>
    /// 获取用于显示文件大小的字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GetDisplayFileSizeString(double length)
    {
        // https://github.com/CommunityToolkit/dotnet/blob/v8.0.0-preview3/CommunityToolkit.Common/Converters.cs#L17
        (length, string unit) = GetDisplayFileSize(length);
        return $"{length:0.00} {unit}";
    }

    /// <summary>
    /// 获取文件的大小
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns>单位 字节</returns>
    public static long GetFileSize(FileInfo fileInfo) => fileInfo.Exists ? fileInfo.Length : 0L;

    /// <summary>
    /// 获取指定路径的大小
    /// </summary>
    /// <param name="dirPath">路径</param>
    /// <returns>单位 字节</returns>
    public static long GetDirectorySize(string dirPath)
    {
        var len = 0L;
        // 判断该路径是否存在（是否为文件夹）
        var isDirectory = IsDirectory(dirPath, out var fileInfo, out var directoryInfo);
        if (isDirectory.HasValue)
        {
            if (!isDirectory.Value)
            {
                //查询文件的大小
                len = GetFileSize(fileInfo!);
            }
            else
            {
                // 通过GetFiles方法，获取di目录中的所有文件的大小
                len += directoryInfo!.GetFiles().Sum(x => x.Length);
                // 获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                len += directoryInfo.GetDirectories().Sum(x => GetDirectorySize(x.FullName));
            }
        }
        return len;
    }
}