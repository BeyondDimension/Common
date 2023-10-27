namespace System.Formats;

public static partial class FileEx
{
    // https://developer.mozilla.org/zh-CN/docs/Web/Media/Formats/Image_types#%E5%B8%B8%E8%A7%81%5%9B%BE%E5%83%8F%E6%96%87%E4%BB%B6%E7%B1%BB%E5%9E%8B

    /// <summary>
    /// .webp
    /// </summary>
    public const string WebP = ".webp";

    /// <summary>
    /// .png
    /// </summary>
    public const string Png = ".png";

    /// <summary>
    /// .jpg
    /// </summary>
    public const string Jpg = ".jpg";

    /// <summary>
    /// .jpeg
    /// </summary>
    public const string Jpeg = ".jpeg";

    /// <summary>
    /// .gif
    /// </summary>
    public const string Gif = ".gif";

    /// <summary>
    /// .svg
    /// </summary>
    public const string Svg = ".svg";

    /// <summary>
    /// .jfif
    /// </summary>
    public const string Jfif = ".jfif";

    /// <summary>
    /// .pjg
    /// </summary>
    public const string Pjg = ".pjg";

    /// <summary>
    /// .pjpeg
    /// </summary>
    public const string Pjpeg = ".pjpeg";

    /// <summary>
    /// .ico
    /// </summary>
    public const string Ico = ".ico";

    /// <summary>
    /// .cur
    /// </summary>
    public const string Cur = ".cur";

    /// <summary>
    /// .bmp
    /// </summary>
    public const string Bmp = ".bmp";

    /// <summary>
    /// .avif
    /// </summary>
    public const string Avif = ".avif";

    /// <summary>
    /// .apng
    /// </summary>
    public const string Apng = ".apng";

    /// <summary>
    /// .heic
    /// </summary>
    public const string Heic = ".heic";

    /// <summary>
    /// .heif
    /// </summary>
    public const string Heif = ".heif";

    /// <summary>
    /// .tif
    /// </summary>
    public const string Tif = ".tif";

    /// <summary>
    /// .tiff
    /// </summary>
    public const string Tiff = ".tiff";

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