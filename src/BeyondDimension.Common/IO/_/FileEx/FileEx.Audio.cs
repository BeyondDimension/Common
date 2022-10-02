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

    static readonly string[] Audios = new[] { M4a, Amr, Aac, Wav, Pcm, Mp3 };

    public static bool IsAudio(string value)
    {
        foreach (var item in Audios)
        {
            if (string.Equals(item, value, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}