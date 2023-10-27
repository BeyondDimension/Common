#if !NETFRAMEWORK
namespace System.IO;

/// <summary>
/// 文件流包装类
/// </summary>
public sealed class FileStreamWrapper : Stream, IFileStreamWrapper
{
    FileStreamWrapper(string path)
    {
        Name = path;
    }

    /// <summary>
    /// 获取或设置文件名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否可读取，始终返回 <see langword="false"/>
    /// </summary>
    public override bool CanRead => false;

    /// <summary>
    /// 是否可查找，始终返回 <see langword="false"/>
    /// </summary>
    public override bool CanSeek => false;

    /// <summary>
    /// 是否可写入，始终返回 <see langword="false"/>
    /// </summary>
    public override bool CanWrite => false;

    /// <summary>
    /// 不支持获取流的长度，抛 <see cref="NotSupportedException"/> 异常
    /// </summary>
    public override long Length => throw new NotSupportedException();

    /// <summary>
    /// 不支持获取或设置当前流中的位置，抛 <see cref="NotSupportedException"/> 异常
    /// </summary>
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    /// <inheritdoc/>
    public override void Flush()
    {
    }

    /// <summary>
    /// 不支持读取
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 不支持查找
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 不支持设置长度
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 不支持写入
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public static implicit operator FileStreamWrapper?(string? filePath) => string.IsNullOrEmpty(filePath) ? null : new(filePath);
}
#endif