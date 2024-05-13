namespace BD.Common8.Essentials.Models;

/// <summary>
/// 表示电子邮件附件的类
/// </summary>
public sealed class EmailAttachment : FileBase, IEmailAttachment
{
    /// <summary>
    /// 使用指定的完整路径初始化 <see cref="EmailAttachment"/> 类的新实例
    /// </summary>
    [SystemTextJsonConstructor]
    public EmailAttachment(string fullPath) : base(fullPath)
    {
    }

    /// <summary>
    /// 使用指定的完整路径和内容类型初始化 <see cref="EmailAttachment"/> 类的新实例
    /// </summary>
    public EmailAttachment(string fullPath, string contentType) : base(fullPath, contentType)
    {
    }

    /// <summary>
    /// 使用指定的 <see cref="FileBase"/> 初始化 <see cref="EmailAttachment"/> 类的新实例
    /// </summary>
    public EmailAttachment(FileBase file) : base(file)
    {
    }
}