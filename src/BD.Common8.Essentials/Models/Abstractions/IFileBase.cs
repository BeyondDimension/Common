namespace BD.Common8.Essentials.Models.Abstractions;

#pragma warning disable SA1600 // Elements should be documented
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
    /// 异步打开文件并返回文件内容的流
    /// </summary>
    Task<Stream> OpenReadAsync();
}