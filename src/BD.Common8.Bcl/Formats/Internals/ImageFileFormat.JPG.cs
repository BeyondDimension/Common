namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// Joint Photographic Experts Group
    /// <para>https://en.wikipedia.org/wiki/JPEG</para>
    /// </summary>
    public static class JPG
    {
        /// <summary>
        /// 图像格式为 <see cref=" ImageFormat.JPEG"/>
        /// </summary>
        public const ImageFormat Format = ImageFormat.JPEG;

        /// <summary>
        /// 默认文件扩展名为 <see cref=" FileEx.JPG"/>
        /// </summary>
        public const string DefaultFileExtension = FileEx.JPG;

        /// <summary>
        /// 默认的 MIME 类型为 <see cref=" FileEx.JPG"/>
        /// </summary>
        public const string DefaultMIME = MediaTypeNames.JPG;

        /// <summary>
        /// 文件扩展名集合
        /// </summary>
        public static readonly string[] FileExtensions = [DefaultFileExtension, ".jpeg"];

        /// <summary>
        /// JPG 文件的幻数
        /// </summary>
        public static readonly byte[] MagicNumber;

        static JPG()
        {
            MagicNumber = [0xFF, 0xD8, 0xFF];
        }
    }
}