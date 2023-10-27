namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// Windows位图（Bitmap）类
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
#pragma warning disable IDE0300 // 简化集合初始化
        public static readonly string[] FileExtensions = { DefaultFileExtension, ".dib" };
#pragma warning restore IDE0300 // 简化集合初始化
        /// <summary>
        /// MIME 类型集合
        /// </summary>
#pragma warning disable IDE0300 // 简化集合初始化
        public static readonly string[] MIME = { DefaultMIME, "image/x-bmp" };
#pragma warning restore IDE0300 // 简化集合初始化

        /// <summary>
        /// 用于识别 BMP 文件的幻数
        /// </summary>
        public static readonly byte[] MagicNumber;

        static BMP()
        {
            MagicNumber = "BM"u8.ToArray();
        }
    }
}