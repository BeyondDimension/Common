namespace System;

public static partial class IOPath
{
    /// <summary>
    /// 文件大小单位倍数 1024
    /// </summary>
    public const double unit_multiple_double_1024 = 1024D;

    /// <summary>
    /// 文件大小单位倍数 1000
    /// </summary>
    public const double unit_multiple_double_1000 = 1000D;

#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
    /// <summary>
    /// 文件大小单位组，KB/MB/GB
    /// </summary>
    public static readonly string[] unit_group = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB",];

    /// <summary>
    /// 文件大小单位组，KiB/MiB/GiB
    /// </summary>
    public static readonly string[] unit_group_i = ["B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB", "BiB",];
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter

    /// <summary>
    /// 获取用于显示的文件大小
    /// </summary>
    /// <param name="length"></param>
    /// <param name="unit_multiple"></param>
    /// <param name="unit_group"></param>
    /// <returns></returns>
    public static (double length, string unit) GetDisplayFileSize(double length, double unit_multiple = unit_multiple_double_1024, string[]? unit_group = null)
    {
        unit_group ??= IOPath.unit_group;
        if (length > 0D)
        {
            for (int i = 0; i < unit_group.Length; i++)
            {
                if (i > 0) length /= unit_multiple;
                if (length < unit_multiple) return (length, unit_group[i]);
            }
            return (length, unit_group.Last());
        }
        else
        {
            return (0D, unit_group.First());
        }
    }

    /// <summary>
    /// 获取用于显示文件大小的字符串
    /// </summary>
    /// <param name="length"></param>
    /// <param name="unit_multiple"></param>
    /// <param name="unit_group"></param>
    /// <returns></returns>
    public static string GetDisplayFileSizeString(double length, double unit_multiple = unit_multiple_double_1024, string[]? unit_group = null)
    {
        // https://github.com/CommunityToolkit/dotnet/blob/v8.0.0-preview3/CommunityToolkit.Common/Converters.cs#L17
        (length, string unitstr) = GetDisplayFileSize(length, unit_multiple, unit_group);
        return $"{length:0.00} {unitstr}";
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