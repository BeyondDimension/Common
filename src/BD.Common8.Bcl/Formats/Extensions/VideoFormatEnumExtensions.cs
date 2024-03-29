namespace System.Formats;

/// <summary>
/// Enum 扩展 <see cref="VideoFormat"/>
/// </summary>
public static partial class VideoFormatEnumExtensions
{
    static readonly Lazy<IReadOnlyDictionary<VideoFormat, string>> mEnumMime = new(() =>
    {
        var value = new Dictionary<VideoFormat, string>()
        {
            { VideoFormat.WebM, MediaTypeNames.WEBM },
            { VideoFormat.MP4, MediaTypeNames.MP4 },
        };
        return value;
    });

    static readonly Lazy<IReadOnlyDictionary<string, VideoFormat>> mMimeEnum = new(() =>
    {
        var value = EnumMime.ToDictionary(x => x.Value, x => x.Key, StringComparer.OrdinalIgnoreCase);
        return value;
    });

    static IReadOnlyDictionary<VideoFormat, string> EnumMime => mEnumMime.Value;

    static IReadOnlyDictionary<string, VideoFormat> MimeEnum => mMimeEnum.Value;

    /// <summary>
    /// 获取 MIME
    /// </summary>
    /// <param name="videoFormat"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string GetMIME(this VideoFormat videoFormat)
    {
        if (EnumMime.ContainsKey(videoFormat)) return EnumMime[videoFormat];
        throw new ArgumentOutOfRangeException(nameof(videoFormat), videoFormat, null);
    }

    /// <summary>
    /// 根据 MIME 获取视频格式
    /// </summary>
    /// <param name="mime"></param>
    /// <returns></returns>
    public static VideoFormat? GetFormat(string? mime)
    {
        if (mime == null) return null;
        if (MimeEnum.ContainsKey(mime)) return MimeEnum[mime];
        return null;
    }
}
