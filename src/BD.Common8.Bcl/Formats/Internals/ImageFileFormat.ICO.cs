namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// ICO 文件格式是 Microsoft Windows 中计算机图标的图像文件格式
    /// <para>https://en.wikipedia.org/wiki/ICO_(file_format)</para>
    /// </summary>
    public static class ICO
    {
        /// <summary>
        /// 图像格式为 <see cref=" ImageFormat.ICO"/>
        /// </summary>
        public const ImageFormat Format = ImageFormat.ICO;

        /// <summary>
        /// 默认文件扩展名为 <see cref=" ImageFormat.ICO"/>
        /// </summary>
        public const string DefaultFileExtension = FileEx.ICO;

        /// <summary>
        /// 默认的 MIME 类型为 <see cref=" ImageFormat.ICO"/>
        /// </summary>
        public const string DefaultMIME = MediaTypeNames.ICO;

        /// <summary>
        /// ICO 文件的幻数
        /// </summary>
        internal static readonly byte[] MagicNumber;

        static ICO()
        {
            MagicNumber = [0x00, 0x00, 0x01, 0x00];
        }
    }
}