namespace BD.Common8.Essentials.Models;

/// <summary>
/// 表示文件结果的类，继承自 <see cref="FileBase"/>，并实现了 <see cref="IFileResult"/> 接口
/// </summary>
public sealed class FileResult : FileBase, IFileResult
{
    /// <summary>
    /// 使用指定的完整路径初始化 <see cref="FileResult"/> 类的新实例
    /// </summary>
    public FileResult(string fullPath) : base(fullPath)
    {
    }

    /// <summary>
    /// 使用指定的完整路径和内容类型初始化 <see cref="FileResult"/> 类的新实例
    /// </summary>
    public FileResult(string fullPath, string contentType) : base(fullPath, contentType)
    {
    }

    /// <summary>
    /// 使用 <see cref="FileBase"/> 对象初始化 <see cref="FileResult"/> 类的新实例
    /// </summary>
    public FileResult(FileBase file) : base(file)
    {
    }
}