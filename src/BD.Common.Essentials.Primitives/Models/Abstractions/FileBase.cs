namespace BD.Common.Models.Abstractions;

public abstract class FileBase : IFileBase
{
    public FileBase(string fullPath)
    {
        FullPath = fullPath;
    }

    public FileBase(string fullPath, string contentType) : this(fullPath)
    {
        ContentType = contentType;
    }

    public FileBase(FileBase file)
    {
        FullPath = file.FullPath;
        ContentType = file.ContentType;
        FileName = file.FileName;
    }

    public string FullPath { get; }

    public string? ContentType { get; set; }

    public virtual Task<Stream> OpenReadAsync()
    {
        var fileStream = IOPath.OpenRead(FullPath);
        return Task.FromResult<Stream>(fileStream.ThrowIsNull());
    }

    string? fileName;

    public string FileName
    {
        get => GetFileName();
        set => fileName = value;
    }

    internal string GetFileName()
    {
        // try the provided file name
        if (!string.IsNullOrWhiteSpace(fileName))
            return fileName;

        // try get from the path
        if (!string.IsNullOrWhiteSpace(FullPath))
            return Path.GetFileName(FullPath);

        // this should never happen as the path is validated in the constructor
        throw new InvalidOperationException($"Unable to determine the file name from '{FullPath}'.");
    }
}