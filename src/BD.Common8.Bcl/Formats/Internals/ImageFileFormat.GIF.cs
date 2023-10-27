#pragma warning disable SA1600 // Elements should be documented
namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// Graphics Interchange Format
    /// <para>https://en.wikipedia.org/wiki/GIF</para>
    /// </summary>
    public static class GIF
    {
        /// <summary>
        /// 图像格式为 <see cref="ImageFormat.GIF"/>
        /// </summary>
        public const ImageFormat Format = ImageFormat.GIF;

        /// <summary>
        /// 默认文件扩展名为 <see cref="FileEx.GIF"/>
        /// </summary>
        public const string DefaultFileExtension = FileEx.GIF;

        /// <summary>
        /// 默认的 MIME 类型为 <see cref="MediaTypeNames.GIF"/>
        /// </summary>
        public const string DefaultMIME = MediaTypeNames.GIF;

        public static readonly byte[] MagicNumber1;

        public static readonly byte[] MagicNumber2;

        public static readonly byte[]?[] MagicNumber;

        static GIF()
        {
            MagicNumber1 = "GIF87a"u8.ToArray();
            MagicNumber2 = "GIF89a"u8.ToArray();
            MagicNumber = [MagicNumber1, MagicNumber2];
        }
    }
}