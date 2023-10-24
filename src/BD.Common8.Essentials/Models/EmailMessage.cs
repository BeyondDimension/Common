namespace BD.Common8.Essentials.Models;

#pragma warning disable SA1600 // Elements should be documented

public sealed class EmailMessage
{
    public EmailMessage()
    {
    }

    public EmailMessage(string subject, string body, params string[] to)
    {
        Subject = subject;
        Body = body;
        To = to?.ToList() ?? [];
    }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public EmailBodyFormat BodyFormat { get; set; }

    public List<string> To { get; set; } = new List<string>();

    public List<string> Cc { get; set; } = new List<string>();

    public List<string> Bcc { get; set; } = new List<string>();

    public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
}