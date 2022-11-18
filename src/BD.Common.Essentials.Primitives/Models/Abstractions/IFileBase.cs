namespace BD.Common.Models.Abstractions;

public interface IFileBase
{
    string FullPath { get; }

    string? ContentType { get; set; }

    string FileName { get; set; }

    Task<Stream> OpenReadAsync();
}