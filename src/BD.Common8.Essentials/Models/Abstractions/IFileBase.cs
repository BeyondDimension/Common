namespace BD.Common8.Essentials.Models.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public interface IFileBase
{
    string FullPath { get; }

    string? ContentType { get; set; }

    string FileName { get; set; }

    Task<Stream> OpenReadAsync();
}