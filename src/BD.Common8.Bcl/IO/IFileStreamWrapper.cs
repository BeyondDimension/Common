#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
namespace System.IO;

/// <summary>
/// 文件流包装器接口
/// </summary>
public interface IFileStreamWrapper
{
    /// <inheritdoc cref="FileStream.Name"/>
    string Name { get; }

    /// <inheritdoc cref="IOPath.OpenRead"/>
    FileStream? FileStream => IOPath.OpenRead(Name);

    /// <inheritdoc cref="IO.FileStream(string, FileMode)"/>
    FileStream GetFileStream(FileMode mode) => new(Name, mode);

    /// <inheritdoc cref="IO.FileStream(string, FileMode, FileAccess)"/>
    FileStream GetFileStream(FileMode mode, FileAccess access) => new(Name, mode, access);

    /// <inheritdoc cref="IO.FileStream(string, FileMode, FileAccess, FileShare)"/>
    FileStream GetFileStream(FileMode mode, FileAccess access, FileShare share) => new(Name, mode, access, share);

    /// <inheritdoc cref="IO.FileStream(string, FileMode, FileAccess, FileShare, int)"/>
    FileStream GetFileStream(FileMode mode, FileAccess access, FileShare share, int bufferSize) => new(Name, mode, access, share, bufferSize);

    /// <inheritdoc cref="IO.FileStream(string, FileMode, FileAccess, FileShare, int, FileOptions)"/>
    FileStream GetFileStream(FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) => new(Name, mode, access, share, bufferSize, options);
}
#endif