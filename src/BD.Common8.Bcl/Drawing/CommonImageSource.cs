namespace System.Drawing;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 通用图片源
/// </summary>
public sealed class CommonImageSource : IDisposable, IFileStreamWrapper
{
    Stream? stream;
    bool disposedValue;

    CommonImageSource(Stream stream)
    {
        OriginStream = this.stream = stream;
    }

    public CommonImageSource Clone()
    {
        CommonImageSource clipStream = new(OriginStream)
        {
            Top = Top,
            Bottom = Bottom,
            Left = Left,
            Right = Right,
            Circle = Circle,
            Radius_X = Radius_X,
            Radius_Y = Radius_Y,
        };
        return clipStream;
    }

    public Stream? Stream
    {
        get
        {
            if (stream is IFileStreamWrapper wrapper)
            {
                stream = wrapper.FileStream;
            }
            return stream;
        }
    }

    public Stream OriginStream { get; }

    public ImageFormat Format { get; init; }

    /// <inheritdoc cref="FileStream.Name"/>
    public string Name
    {
        get
        {
            if (OriginStream is IFileStreamWrapper wrapper)
                return wrapper.Name;
            else if (OriginStream is FileStream file)
                return file.Name;
            return string.Empty;
        }
    }

    public override string? ToString()
    {
        if (OriginStream is IFileStreamWrapper wrapper)
            return wrapper.Name;
        else if (OriginStream is FileStream file)
            return file.Name;
        return base.ToString();
    }

    public float Top { get; set; }

    public float Left { get; set; }

    public float Right { get; set; }

    public float Bottom { get; set; }

    public float TopBottom
    {
        set
        {
            Top = value;
            Bottom = value;
        }
    }

    public float LeftRight
    {
        set
        {
            Left = value;
            Right = value;
        }
    }

    public float TopBottomLeftRight
    {
        set
        {
            Top = value;
            Bottom = value;
            Left = value;
            Right = value;
        }
    }

    public float Radius_X { get; set; }

    public float Radius_Y { get; set; }

    public float Radius
    {
        set
        {
            Radius_X = value;
            Radius_Y = value;
        }
    }

    public bool Circle { get; set; }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                stream?.Dispose();
                OriginStream.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            stream = null;
            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public static implicit operator CommonImageSource?(Stream? stream)
    {
        if (stream == null)
            return null;
        if (stream.CanSeek)
        {
            if (FileFormat.IsImage(stream, out var imageFormat))
            {
                return new(stream)
                {
                    Format = imageFormat,
                };
            }
            return null;
        }
        return new(stream);
    }

    public static implicit operator CommonImageSource?(string? filePath)
    {
        FileStreamWrapper? wrapper = filePath;
        if (wrapper == null)
            return null;

        try
        {
            using var fs = new FileStream(filePath!,
                FileMode.Open,
                FileAccess.Read,
                IOPath.FileShareReadWriteDelete);
            if (FileFormat.IsImage(fs, out var imageFormat))
            {
                return new(wrapper)
                {
                    Format = imageFormat,
                };
            }
        }
        catch
        {
        }
        return null;
    }

    /// <summary>
    /// 将图片文件本地路径或资源路径转为图像源
    /// </summary>
    /// <param name="filePathOrResUri"></param>
    /// <param name="isCircle">是否为圆形</param>
    /// <param name="config">图像可配置选项</param>
    /// <returns></returns>
    public static object? TryParse(string? filePathOrResUri, bool isCircle = false, Action<CommonImageSource>? config = null)
    {
        if (filePathOrResUri == null)
            return null;
        if (filePathOrResUri.StartsWith("avares:"))
            return filePathOrResUri;
        CommonImageSource? commonImageSource = filePathOrResUri;
        if (commonImageSource != null)
        {
            commonImageSource.Circle = isCircle;
            config?.Invoke(commonImageSource);
        }
        return commonImageSource;
    }

    /// <summary>
    /// 将图片文件本地路径或资源路径转为图像源
    /// </summary>
    /// <param name="imageStream"></param>
    /// <param name="isCircle">是否为圆形</param>
    /// <param name="config">图像可配置选项</param>
    /// <returns></returns>
    public static CommonImageSource? TryParse(Stream? imageStream, bool isCircle = false, Action<CommonImageSource>? config = null)
    {
        if (imageStream == null)
            return null;

        CommonImageSource? commonImageSource = imageStream;
        if (commonImageSource != null)
        {
            commonImageSource.Circle = isCircle;
            config?.Invoke(commonImageSource);
        }
        return commonImageSource;
    }
}
