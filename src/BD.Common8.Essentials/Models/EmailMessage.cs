namespace BD.Common8.Essentials.Models;

/// <summary>
/// 电子邮件
/// </summary>
public sealed class EmailMessage
{
    /// <summary>
    /// 用于创建空的邮件消息对象
    /// </summary>
    public EmailMessage()
    {
    }

    /// <summary>
    /// 用于创建具有指定主题、正文和收件人的邮件消息对象
    /// </summary>
    public EmailMessage(string subject, string body, params string[] to)
    {
        Subject = subject;
        Body = body;
        To = to?.ToList() ?? [];
    }

    /// <summary>
    /// 邮件主题
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 邮件正文
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// 邮件正文格式
    /// </summary>
    public EmailBodyFormat BodyFormat { get; set; }

    /// <summary>
    /// 收件人列表
    /// </summary>
    public List<string> To { get; set; } = [];

    /// <summary>
    /// 抄送列表
    /// </summary>
    public List<string> Cc { get; set; } = [];

    /// <summary>
    /// 密送列表
    /// </summary>
    public List<string> Bcc { get; set; } = [];

    /// <summary>
    /// 附件列表
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = [];
}