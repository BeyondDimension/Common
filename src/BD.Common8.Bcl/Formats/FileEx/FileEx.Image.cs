namespace System.Formats;

public static partial class FileEx
{
#pragma warning disable SA1600 // Elements should be documented
    // https://developer.mozilla.org/zh-CN/docs/Web/Media/Formats/Image_types#%E5%B8%B8%E8%A7%81%5%9B%BE%E5%83%8F%E6%96%87%E4%BB%B6%E7%B1%BB%E5%9E%8B

    public const string WebP = ".webp";
    public const string Png = ".png";
    public const string Jpg = ".jpg";
    public const string Jpeg = ".jpeg";
    public const string Gif = ".gif";
    public const string Svg = ".svg";
    public const string Jfif = ".jfif";
    public const string Pjg = ".pjg";
    public const string Pjpeg = ".pjpeg";
    public const string Ico = ".ico";
    public const string Cur = ".cur";
    public const string Bmp = ".bmp";
    public const string Avif = ".avif";
    public const string Apng = ".apng";
    public const string Heic = ".heic";
    public const string Heif = ".heif";
    public const string Tif = ".tif";
    public const string Tiff = ".tiff";

#pragma warning restore SA1600 // Elements should be documented

    static IEnumerable<string>? _Images;

    /// <summary>
    /// 图片
    /// </summary>
    public static IEnumerable<string> Images
    {
        get
        {
            if (_Images == null) return GetImages();
            return _Images;
        }

        set
        {
            _Images = value;
        }
    }

    static IEnumerable<string> GetImages()
    {
        yield return WebP;
        yield return Png;
        yield return Jpg;
        yield return Jpeg;
        yield return Gif;
        yield return Svg;
        yield return Jfif;
        yield return Pjg;
        yield return Pjpeg;
        yield return Ico;
        yield return Cur;
        yield return Bmp;
        yield return Avif;
        yield return Apng;
        yield return Heic;
        yield return Heif;
        yield return Tif;
        yield return Tiff;
    }

    /// <summary>
    /// 判断文件扩展名是否为图片
    /// </summary>
    /// <param name="fileEx"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsImage(string fileEx)
        => Images.Any(x
            => string.Equals(x, fileEx, StringComparison.OrdinalIgnoreCase));
}