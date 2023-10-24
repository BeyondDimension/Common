namespace BD.Common8.Essentials.Models;

#pragma warning disable SA1600 // Elements should be documented

public sealed class SaveFileResult : IDisposable
{
    readonly string? fullPath;
    Stream? stream;
    readonly string? @string;
    bool disposedValue;

    public SaveFileResult(string fullPath)
    {
        this.fullPath = fullPath;
    }

    public SaveFileResult(Stream stream, string? @string = null)
    {
        this.stream = stream;
        this.@string = @string;
    }

    /// <summary>
    /// 打开写入流，在桌面平台上为 <see cref="FileStream"/>，在 Android 上为 Java.IO.OutputStream
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Stream OpenWrite()
    {
        if (fullPath != null)
            return new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
        if (stream != null)
            return stream;
        throw new NotSupportedException();
    }

    public override string? ToString()
    {
        if (fullPath != null)
            return fullPath;
        if (@string != null)
            return @string;
        return base.ToString();
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                // 释放托管状态(托管对象)
                stream?.Dispose();

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            stream = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}