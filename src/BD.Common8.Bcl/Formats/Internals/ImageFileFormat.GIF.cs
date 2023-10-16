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
        public const ImageFormat Format = ImageFormat.GIF;

        public const string DefaultFileExtension = FileEx.GIF;

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