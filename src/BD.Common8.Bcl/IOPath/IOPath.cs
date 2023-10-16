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
}
