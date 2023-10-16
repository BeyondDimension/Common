#pragma warning disable SA1600 // Elements should be documented

namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// The ICO file format is an image file format for computer icons in Microsoft Windows.
    /// <para>https://en.wikipedia.org/wiki/ICO_(file_format)</para>
    /// </summary>
    public static class ICO
    {
        public const ImageFormat Format = ImageFormat.ICO;

        public const string DefaultFileExtension = FileEx.ICO;

        public const string DefaultMIME = MediaTypeNames.ICO;

        internal static readonly byte[] MagicNumber;

        static ICO()
        {
            MagicNumber = [0x00, 0x00, 0x01, 0x00];
        }
    }
}