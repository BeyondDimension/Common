#pragma warning disable SA1600 // Elements should be documented

namespace System.Formats.Internals;

partial class ImageFileFormat
{
    /// <summary>
    /// 高效率图像文件格式（HEIF）
    /// <para>https://en.wikipedia.org/wiki/High_Efficiency_Image_File_Format</para>
    /// <para>https://www.jianshu.com/p/b016d10a087d</para>
    /// <para>https://www.freesion.com/article/5585164728/</para>
    /// </summary>
    public static class HEIF_HEIC
    {
        public static class HEIF
        {
            /// <summary>
            /// 图像格式为 <see cref="ImageFormat.HEIF"/>
            /// </summary>
            public const ImageFormat Format = ImageFormat.HEIF;

            /// <summary>
            /// 默认文件扩展名为 <see cref="FileEx.HEIF"/>
            /// </summary>
            public const string DefaultFileExtension = FileEx.HEIF;

            /// <summary>
            /// 默认的 MIME 类型为 <see cref="MediaTypeNames.HEIF"/>
            /// </summary>
            public const string DefaultMIME = MediaTypeNames.HEIF;

            /// <summary>
            /// ftypmif1
            /// </summary>
            public static readonly byte?[] MagicNumber;

            static HEIF()
            {
                MagicNumber = [null, null, null, null, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'m', (byte)'i', (byte)'f', (byte)'1'];
            }
        }

        public static class HEIFSequence
        {
            /// <summary>
            /// 图像格式为 <see cref=" ImageFormat.HEIFSequence"/>
            /// </summary>
            public const ImageFormat Format = ImageFormat.HEIFSequence;

            /// <summary>
            /// 默认文件扩展名为 <see cref="MediaTypeNames.HEIFSequence"/>
            /// </summary>
            public const string DefaultMIME = MediaTypeNames.HEIFSequence;

            /// <summary>
            /// ftypmsf1
            /// </summary>
            public static readonly byte?[] MagicNumber;

            static HEIFSequence()
            {
                MagicNumber = [null, null, null, null, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'m', (byte)'s', (byte)'f', (byte)'1'];
            }
        }

        public static class HEIC
        {
            /// <summary>
            /// 图像格式为 <see cref=" ImageFormat.HEIC"/>
            /// </summary>
            public const ImageFormat Format = ImageFormat.HEIC;

            /// <summary>
            /// 默认文件扩展名为 <see cref="MediaTypeNames.HEIC"/>
            /// </summary>
            public const string DefaultFileExtension = FileEx.HEIC;

            /// <summary>
            /// 默认的 MIME 类型为 <see cref="MediaTypeNames.HEIC"/>
            /// </summary>
            public const string DefaultMIME = MediaTypeNames.HEIC;

            public static readonly byte?[]?[] MagicNumber;

            /// <summary>
            /// ftypheic
            /// </summary>
            public static readonly byte?[] MagicNumber1;

            /// <summary>
            /// ftypheix
            /// </summary>
            public static readonly byte?[] MagicNumber2;

            static HEIC()
            {
                MagicNumber1 = [null, null, null, null, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'h', (byte)'e', (byte)'i', (byte)'c'];
                MagicNumber2 = [null, null, null, null, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'h', (byte)'e', (byte)'i', (byte)'x'];
                MagicNumber = new[] { MagicNumber1, MagicNumber2 };
            }
        }

        public static class HEICSequence
        {
            /// <summary>
            /// 图像格式为 <see cref=" ImageFormat.HEICSequence"/>
            /// </summary>
            public const ImageFormat Format = ImageFormat.HEICSequence;

            /// <summary>
            /// 默认文件扩展名为  <see cref=" ImageFormat.HEICSequence"/>
            /// </summary>
            public const string DefaultMIME = MediaTypeNames.HEICSequence;

            public static readonly byte?[]?[] MagicNumber;

            /// <summary>
            /// ftyphevc
            /// </summary>
            public static readonly byte?[] MagicNumber1;

            /// <summary>
            /// ftyphevx
            /// </summary>
            public static readonly byte?[] MagicNumber2;

            static HEICSequence()
            {
                MagicNumber1 = [null, null, null, null, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'h', (byte)'e', (byte)'v', (byte)'c'];
                MagicNumber2 = [null, null, null, null, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'h', (byte)'e', (byte)'v', (byte)'x'];
                MagicNumber = new[] { MagicNumber1, MagicNumber2 };
            }
        }
    }
}