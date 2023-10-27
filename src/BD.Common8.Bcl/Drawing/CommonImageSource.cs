namespace System.Drawing;

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

    /// <summary>
    /// 克隆当前实例的副本
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 获取图片流
    /// </summary>
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

    /// <summary>
    /// 原始的图片流
    /// </summary>
    public Stream OriginStream { get; }

    /// <summary>
    /// 图片格式
    /// </summary>
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

    /// <inheritdoc/>
    public override string? ToString()
    {
        if (OriginStream is IFileStreamWrapper wrapper)
            return wrapper.Name;
        else if (OriginStream is FileStream file)
            return file.Name;
        return base.ToString();
    }

    /// <summary>
    /// 上边距
    /// </summary>
    public float Top { get; set; }

    /// <summary>
    /// 左边距
    /// </summary>
    public float Left { get; set; }

    /// <summary>
    /// 右边距
    /// </summary>
    public float Right { get; set; }

    /// <summary>
    /// 下边距
    /// </summary>
    public float Bottom { get; set; }

    /// <summary>
    /// 设置上下边距相同
    /// </summary>
    public float TopBottom
    {
        set
        {
            Top = value;
            Bottom = value;
        }
    }

    /// <summary>
    /// 设置左右边距相同
    /// </summary>
    public float LeftRight
    {
        set
        {
            Left = value;
            Right = value;
        }
    }

    /// <summary>
    /// 设置上下左右边距相同
    /// </summary>
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

    /// <summary>
    /// X 轴圆角半径
    /// </summary>
    public float Radius_X { get; set; }

    /// <summary>
    /// Y 轴圆角半径
    /// </summary>
    public float Radius_Y { get; set; }

    /// <summary>
    /// 设置半径相同
    /// </summary>
    public float Radius
    {
        set
        {
            Radius_X = value;
            Radius_Y = value;
        }
    }

    /// <summary>
    /// 是否为圆形图片
    /// </summary>
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
    /// 尝试将文件路径或资源 URI 转换为 <see cref="CommonImageSource"/>，如果转换失败返回值将为 <see langword="null"/>
    /// </summary>
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
    /// 尝试将图片的输入流转换为 <see cref="CommonImageSource"/>，如果转换失败返回值将为 <see langword="null"/>
    /// </summary>
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
