namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// Tagged Image File Format 标签图像文件格式
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/Media/Formats/Image_types#tiff_tagged_image_file_format</para>
    /// </summary>
    public static class TIFF
    {
        /// <summary>
        /// 图像格式为 <see cref="ImageFormat.TIFF"/>
        /// </summary>
        public const ImageFormat Format = ImageFormat.TIFF;

        /// <summary>
        /// 默认文件扩展名为 <see cref="FileEx.TIFF"/>
        /// </summary>
        public const string DefaultFileExtension = FileEx.TIFF;

        /// <summary>
        /// 默认的 MIME 类型为 <see cref="MediaTypeNames.TIFF"/>
        /// </summary>
        public const string DefaultMIME = MediaTypeNames.TIFF;

        /// <summary>
        /// 文件扩展名集合
        /// </summary>
        public static readonly string[] FileExtensions = [DefaultFileExtension, ".tif"];

        /// <summary>
        /// II*
        /// </summary>
        public static readonly byte[] MagicNumber1;

        /// <summary>
        /// MM*
        /// </summary>
        public static readonly byte[] MagicNumber2;

        /// <summary>
        /// TIFF 文件的幻数
        /// </summary>
        public static readonly byte[]?[] MagicNumber;

        static TIFF()
        {
            MagicNumber1 = "II*"u8.ToArray();
            MagicNumber2 = "MM*"u8.ToArray();
            MagicNumber = [MagicNumber1, MagicNumber2];
        }
    }
}
