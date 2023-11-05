namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// 可移植网络图形 PNG
    /// <para>https://en.wikipedia.org/wiki/Portable_Network_Graphics</para>
    /// </summary>
    public static class PNG
    {
        /// <summary>
        /// 图像格式为 <see cref=" ImageFormat.PNG"/>
        /// </summary>
        public const ImageFormat Format = ImageFormat.PNG;

        /// <summary>
        /// 默认文件扩展名为 <see cref="FileEx.PNG"/>
        /// </summary>
        public const string DefaultFileExtension = FileEx.PNG;

        /// <summary>
        /// 默认的 MIME 类型为 <see cref="MediaTypeNames.PNG"/>
        /// </summary>
        public const string DefaultMIME = MediaTypeNames.PNG;

        /// <summary>
        /// PNG 文件的幻数
        /// </summary>
        public static readonly byte[] MagicNumber;

        static PNG()
        {
            MagicNumber = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        }
    }
}