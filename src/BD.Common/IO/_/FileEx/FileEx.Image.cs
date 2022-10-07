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

    static readonly string[] Images = new[] { WebP, Png, Jpg, Jpeg, Gif, Svg, Jfif, Pjg, Pjpeg, Ico, Cur, Bmp, Avif, Apng, Heic, Heif, Tif, Tiff };

    public static bool IsImage(string value)
    {
        foreach (var item in Images)
        {
            if (string.Equals(item, value, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}