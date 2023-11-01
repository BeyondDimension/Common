namespace BD.Common8.Essentials.Models;

/// <summary>
/// 提供保存文件操作的结果
/// </summary>
public sealed class SaveFileResult : IDisposable
{
    /// <summary>
    /// 文件的完整路径
    /// </summary>
    readonly string? fullPath;

    /// <summary>
    /// 文件的流对象
    /// </summary>
    Stream? stream;

    /// <summary>
    /// 文件的文本内容
    /// </summary>
    readonly string? @string;

    /// <summary>
    /// 表示资源是否已经被释放的标志
    /// </summary>
    bool disposedValue;

    /// <summary>
    /// 通过指定文件的完整路径创建 SaveFileResult 实例
    /// </summary>
    public SaveFileResult(string fullPath)
    {
        this.fullPath = fullPath;
    }

    /// <summary>
    /// 通过指定文件的流对象和可选的文本内容创建 SaveFileResult 实例
    /// </summary>
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

    /// <inheritdoc/>
    public override string? ToString()
    {
        if (fullPath != null)
            return fullPath;
        if (@string != null)
            return @string;
        return base.ToString();
    }

    /// <summary>
    /// 释放资源，指定是否释放托管资源
    /// </summary>
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

    /// <summary>
    /// 释放资源，始终会释放托管资源
    /// </summary>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}