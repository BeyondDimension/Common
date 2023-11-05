namespace BD.Common8.Essentials.Models.Abstractions;

/// <summary>
/// 提供文件操作的基本属性和方法
/// </summary>
public interface IFileBase
{
    /// <summary>
    /// 获取文件的完整路径
    /// </summary>
    string FullPath { get; }

    /// <summary>
    /// 获取或设置文件的内容类型
    /// </summary>
    string? ContentType { get; set; }

    /// <summary>
    /// 获取或设置文件的名称
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// 打开文件并以异步方式读取文件流
    /// </summary>
    Task<Stream> OpenReadAsync();
}