namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// WebP 是一种同时采用有损和无损压缩的图像格式
    /// <para>https://en.wikipedia.org/wiki/WebP</para>
    /// </summary>
    public static class WebP
    {
        /// <summary>
        /// 图像格式为 WebP
        /// </summary>
        public const ImageFormat Format = ImageFormat.WebP;

        /// <summary>
        /// WebP 默认文件扩展名
        /// </summary>
        public const string DefaultFileExtension = FileEx.WEBP;

        /// <summary>
        /// WebP 默认 MIME 类型
        /// </summary>
        public const string DefaultMIME = MediaTypeNames.WEBP;

        /// <summary>
        /// 用于识别 WebP 格式的幻数
        /// </summary>
        public static readonly byte?[] MagicNumber;

        static WebP()
        {
            MagicNumber = [0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x57, 0x45, 0x42, 0x50];
        }
    }
}