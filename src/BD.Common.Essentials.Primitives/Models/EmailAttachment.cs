namespace BD.Common.Models;

public sealed class EmailAttachment : FileBase, IEmailAttachment
{
    public EmailAttachment(string fullPath) : base(fullPath)
    {

    }

    public EmailAttachment(string fullPath, string contentType) : base(fullPath, contentType)
    {

    }

    public EmailAttachment(FileBase file) : base(file)
    {
    }
}