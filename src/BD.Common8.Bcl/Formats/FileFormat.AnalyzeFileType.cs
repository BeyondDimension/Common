#pragma warning disable SA1600 // Elements should be documented

namespace System.Formats;

partial class FileFormat
{
    public readonly record struct AnalyzeFileTypeResult
    {
        AnalyzeFileTypeResult(string fileEx)
        {
            FileEx = fileEx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AnalyzeFileTypeResult(string fileEx)
            => new(fileEx);

        AnalyzeFileTypeResult(ImageFormat imageFormat)
        {
            ImageFormat = imageFormat;
            FileEx = imageFormat.GetExtension();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AnalyzeFileTypeResult(ImageFormat imageFormat)
            => new(imageFormat);

        public string FileEx { get; init; }

        public ImageFormat? ImageFormat { get; init; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AnalyzeFileTypeResult AnalyzeFileType(ReadOnlyMemory<byte> buffer)
    {
#if NET6_0_OR_GREATER
        if (IsImage(buffer, out var imageFormat))
        {
            return imageFormat;
        }

        if (IsSQLite3(buffer))
        {
            return FileEx.SQLite;
        }

        Utf8StringComparerOrdinalIgnoreCase comparer = new();
        // 根据文件头识别一些文件类型使用正确的文件扩展名
        var magicNumber = "<html"u8;
        if (magicNumber.SequenceEqual(buffer.Span[..magicNumber.Length], comparer))
        {
            return FileEx.HTML;
        }
        magicNumber = "<body"u8;
        if (magicNumber.SequenceEqual(buffer.Span[..magicNumber.Length], comparer))
        {
            return FileEx.HTML;
        }
        magicNumber = "<!DOCTYPE"u8;
        if (magicNumber.SequenceEqual(buffer.Span[..magicNumber.Length], comparer))
        {
            return FileEx.HTML;
        }
        magicNumber = "<?xml"u8;
        if (magicNumber.SequenceEqual(buffer.Span[..magicNumber.Length], comparer))
        {
            return FileEx.XML;
        }
        magicNumber = "{"u8;
        if (magicNumber.SequenceEqual(buffer.Span[..magicNumber.Length], comparer))
        {
            return FileEx.JSON;
        }

        return FileEx.BIN;
#else
        throw new NotSupportedException("Only supports runtime versions of. NET 6 or higher.");
#endif
    }
}