// ReSharper disable once CheckNamespace
namespace System.IO;

public static partial class FileEx
{
    public const string Mp4 = ".mp4";
    public const string Mpg = ".mpg";
    public const string Mpeg = ".mpeg";
    public const string Dat = ".dat";
    public const string Asf = ".asf";
    public const string Avi = ".avi";
    public const string Rm = ".rm";
    public const string Rmvb = ".rmvb";
    public const string Mov = ".mov";
    public const string Wmv = ".wmv";
    public const string Flv = ".flv";
    public const string Mkv = ".mkv";

    static readonly string[] Videos = new[] { Mp4, Mpg, Mpeg, Dat, Asf, Avi, Rm, Rmvb, Mov, Wmv, Flv, Mkv };

    public static bool IsVideo(string value)
    {
        foreach (var item in Videos)
        {
            if (string.Equals(item, value, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}