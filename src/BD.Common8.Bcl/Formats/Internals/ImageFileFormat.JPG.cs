#pragma warning disable SA1600 // Elements should be documented

namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// Joint Photographic Experts Group
    /// <para>https://en.wikipedia.org/wiki/JPEG</para>
    /// </summary>
    public static class JPG
    {
        public const ImageFormat Format = ImageFormat.JPEG;

        public const string DefaultFileExtension = FileEx.JPG;

        public const string DefaultMIME = MediaTypeNames.JPG;

#pragma warning disable IDE0300 // 简化集合初始化
        public static readonly string[] FileExtensions = { DefaultFileExtension, ".jpeg" };
#pragma warning restore IDE0300 // 简化集合初始化

        public static readonly byte[] MagicNumber;

        static JPG()
        {
            MagicNumber = [0xFF, 0xD8, 0xFF];
        }
    }
}