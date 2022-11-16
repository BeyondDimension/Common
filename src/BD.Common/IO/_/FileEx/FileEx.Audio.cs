// ReSharper disable once CheckNamespace
namespace System.IO;

public static partial class FileEx
{
    public const string M4a = ".m4a";
    public const string Amr = ".amr";
    public const string Aac = ".aac";
    public const string Wav = ".wav";
    public const string Pcm = ".pcm";
    public const string Mp3 = ".mp3";

    static IEnumerable<string>? _Audios;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAudio(string value)
        => Audios.Any(x
            => string.Equals(x, value, StringComparison.OrdinalIgnoreCase));
}