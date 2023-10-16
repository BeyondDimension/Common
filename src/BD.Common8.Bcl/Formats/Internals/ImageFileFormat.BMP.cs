#pragma warning disable SA1600 // Elements should be documented

namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// Windows Bitmap
    /// <para>https://en.wikipedia.org/wiki/BMP_file_format</para>
    /// </summary>
    public static class BMP
    {
        public const ImageFormat Format = ImageFormat.BMP;

        public const string DefaultFileExtension = FileEx.BMP;

        public const string DefaultMIME = MediaTypeNames.BMP;

#pragma warning disable IDE0300 // 简化集合初始化
        public static readonly string[] FileExtensions = { DefaultFileExtension, ".dib" };
#pragma warning restore IDE0300 // 简化集合初始化

#pragma warning disable IDE0300 // 简化集合初始化
        public static readonly string[] MIME = { DefaultMIME, "image/x-bmp" };
#pragma warning restore IDE0300 // 简化集合初始化

        public static readonly byte[] MagicNumber;

        static BMP()
        {
            MagicNumber = "BM"u8.ToArray();
        }
    }
}