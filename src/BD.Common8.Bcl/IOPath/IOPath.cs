namespace System;

/// <summary>
/// 文件夹(Directory), 文件(File), 路径/目录(Path) 工具类
/// </summary>
public static partial class IOPath
{
    /// <summary>
    /// 允许文件共享 <see cref="FileShare.Read"/> OR <see cref="FileShare.Write"/> OR <see cref="FileShare.Delete"/>
    /// </summary>
    public const FileShare FileShareReadWriteDelete = FileShare.ReadWrite | FileShare.Delete;

#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    /// <summary>
    /// 返回从一个路径到另一个路径的相对路径
    /// </summary>
    /// <param name="relativeTo">相对于结果的源路径。 此路径始终被视为目录</param>
    /// <param name="path">目标路径</param>
    /// <returns>相对路径，如果路径不共享同一根，则为 path</returns>
    public static string GetRelativePath(string relativeTo, string path)
    {
        throw new NotImplementedException("TODO impl GetRelativePath");

        // https://learn.microsoft.com/zh-cn/dotnet/api/system.io.path.getrelativepath
        // 下面的代码演示如何调用 GetRelativePath 方法。
        //// "C:/Program Files/Microsoft" relative to "C:/Program Files/Common Files" is "../Microsoft"
        //Console.WriteLine(Path.GetRelativePath("C:/Program Files/Common Files", "C:/Program Files/Microsoft"));
        //// "C:/Program Files/Microsoft" relative to "C:/Program Files/" is "Microsoft"
        //Console.WriteLine(Path.GetRelativePath("C:/Program Files/", "C:/Program Files/Microsoft"));

        //// This code produces output similar to the following:
        ////
        //// ../Microsoft
        //// Microsoft
    }
#endif
}
