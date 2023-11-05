namespace BD.Common8.Essentials.Models.Abstractions;

/// <inheritdoc cref="IFileBase"/>
public abstract class FileBase : IFileBase
{
    /// <summary>
    /// 根据文件路径实例化
    /// </summary>
    public FileBase(string fullPath)
    {
        FullPath = fullPath;
    }

    /// <summary>
    /// 根据文件路径和内容类型实例化
    /// </summary>
    public FileBase(string fullPath, string contentType) : this(fullPath)
    {
        ContentType = contentType;
    }

    /// <summary>
    /// 根据文件基类实例化
    /// </summary>
    /// <param name="file"></param>
    public FileBase(FileBase file)
    {
        FullPath = file.FullPath;
        ContentType = file.ContentType;
        FileName = file.FileName;
    }

    /// <inheritdoc/>
    public string FullPath { get; }

    /// <inheritdoc/>
    public string? ContentType { get; set; }

    /// <inheritdoc/>
    public virtual Task<Stream> OpenReadAsync()
    {
        var fileStream = IOPath.OpenRead(FullPath);
        return Task.FromResult<Stream>(fileStream.ThrowIsNull());
    }

    string? fileName;

    /// <inheritdoc/>
    public string FileName
    {
        get => GetFileName();
        set => fileName = value;
    }

    /// <summary>
    /// 获取文件的名称，如果文件名未提供，则从路径获取文件名
    /// </summary>
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