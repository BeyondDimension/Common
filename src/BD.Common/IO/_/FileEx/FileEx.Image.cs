// ReSharper disable once CheckNamespace
namespace System.IO;

public static partial class FileEx
{
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

    static IEnumerable<string>? _Images;

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

    [Obsolete("use Images/IsImage", true)]
    public static string[] ImageFileExtensions => Images.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsImage(string value)
        => Images.Any(x
            => string.Equals(x, value, StringComparison.OrdinalIgnoreCase));
}