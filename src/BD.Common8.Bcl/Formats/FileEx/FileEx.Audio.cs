namespace System.Formats;

/// <summary>
/// 文件扩展名 .xyz
/// </summary>
public static partial class FileEx
{
    /// <summary>
    /// .m4a
    /// </summary>
    public const string M4a = ".m4a";

    /// <summary>
    /// .amr
    /// </summary>
    public const string Amr = ".amr";

    /// <summary>
    /// .aac
    /// </summary>
    public const string Aac = ".aac";

    /// <summary>
    /// .wav
    /// </summary>
    public const string Wav = ".wav";

    /// <summary>
    /// .pcm
    /// </summary>
    public const string Pcm = ".pcm";

    /// <summary>
    /// .mp3
    /// </summary>
    public const string Mp3 = ".mp3";

    static IEnumerable<string>? _Audios;

    /// <summary>
    /// 音频
    /// </summary>
    public static IEnumerable<string> Audios
    {
        get
        {
            if (_Audios == null) return GetAudios();
            return _Audios;
        }

        set
        {
            _Audios = value;
        }
    }

    static IEnumerable<string> GetAudios()
    {
        yield return M4a;
        yield return Amr;
        yield return Aac;
        yield return Wav;
        yield return Pcm;
        yield return Mp3;
    }

    /// <summary>
    /// 判断文件扩展名是否为音频
    /// </summary>
    /// <param name="fileEx"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAudio(string fileEx)
        => Audios.Any(x
            => string.Equals(x, fileEx, StringComparison.OrdinalIgnoreCase));
}