namespace BD.Common.Models;

public sealed class FileResult : FileBase, IFileResult
{
    public FileResult(string fullPath) : base(fullPath)
    {

    }

    public FileResult(string fullPath, string contentType) : base(fullPath, contentType)
    {

    }

    public FileResult(FileBase file) : base(file)
    {
    }
}