namespace System.IO;

public sealed class FileStreamWrapper : Stream, IFileStreamWrapper
{
    FileStreamWrapper(string path)
    {
        Name = path;
    }

    public string Name { get; set; }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public static implicit operator FileStreamWrapper?(string? filePath) => string.IsNullOrEmpty(filePath) ? null : new(filePath);
}
