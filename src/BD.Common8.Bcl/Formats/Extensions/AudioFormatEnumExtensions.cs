#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Formats;

/// <summary>
/// Enum 扩展 <see cref="AudioFormat"/>
/// </summary>
public static partial class AudioFormatEnumExtensions
{
    /// <summary>
    /// 获取扩展名
    /// </summary>
    /// <param name="audioFormat"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string GetExtension(this AudioFormat audioFormat)
    {
        return audioFormat switch
        {
            AudioFormat.AMR => FileEx.AMR,
            AudioFormat.WAV => FileEx.WAV,
            AudioFormat.CAF => FileEx.CAF,
            AudioFormat.MP3 => FileEx.MP3,
            _ => throw new ArgumentOutOfRangeException(nameof(audioFormat), audioFormat, null),
        };
    }

    /// <summary>
    /// 获取 MIME
    /// </summary>
    /// <param name="audioFormat"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string GetMIME(this AudioFormat audioFormat)
    {
        return audioFormat switch
        {
            AudioFormat.AMR => MediaTypeNames.AMR,
            AudioFormat.WAV => MediaTypeNames.WAV,
            AudioFormat.CAF => MediaTypeNames.CAF,
            AudioFormat.MP3 => MediaTypeNames.MP3,
            _ => throw new ArgumentOutOfRangeException(nameof(audioFormat), audioFormat, null),
        };
    }
}
