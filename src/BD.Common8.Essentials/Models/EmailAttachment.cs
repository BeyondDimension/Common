namespace BD.Common8.Essentials.Models;

#pragma warning disable SA1600 // Elements should be documented

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