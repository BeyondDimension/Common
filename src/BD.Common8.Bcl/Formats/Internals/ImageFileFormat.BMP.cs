namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// Windows 位图
    /// <para>https://en.wikipedia.org/wiki/BMP_file_format</para>
    /// </summary>
    public static class BMP
    {
        /// <summary>
        /// 图像格式为 <see cref="ImageFormat.BMP"/>
        /// </summary>
        public const ImageFormat Format = ImageFormat.BMP;

        /// <summary>
        /// 默认文件扩展名为 <see cref="FileEx.BMP"/>
        /// </summary>
        public const string DefaultFileExtension = FileEx.BMP;

        /// <summary>
        /// 默认的 MIME 类型为 <see cref="MediaTypeNames.BMP"/>
        /// </summary>
        public const string DefaultMIME = MediaTypeNames.BMP;

        /// <summary>
        /// 文件扩展名集合
        /// </summary>
        public static readonly string[] FileExtensions = [DefaultFileExtension, ".dib"];

        /// <summary>
        /// MIME 类型集合
        /// </summary>
        public static readonly string[] MIME = [DefaultMIME, "image/x-bmp"];

        /// <summary>
        /// BMP 文件的幻数
        /// </summary>
        public static readonly byte[] MagicNumber;

        static BMP()
        {
            MagicNumber = "BM"u8.ToArray();
        }
    }
}