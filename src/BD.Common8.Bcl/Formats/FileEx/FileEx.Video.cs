namespace System.Formats;

public static partial class FileEx
{
    /// <summary>
    /// .mp4
    /// </summary>
    public const string Mp4 = ".mp4";

    /// <summary>
    /// .m4v
    /// </summary>
    public const string M4v = ".m4v";

    /// <summary>
    /// .mpg
    /// </summary>
    public const string Mpg = ".mpg";

    /// <summary>
    /// .mpeg
    /// </summary>
    public const string Mpeg = ".mpeg";

    /// <summary>
    /// .mp2
    /// </summary>
    public const string Mp2 = ".mp2";

    /// <summary>
    /// .dat
    /// </summary>
    public const string Dat = ".dat";

    /// <summary>
    /// .asf
    /// </summary>
    public const string Asf = ".asf";

    /// <summary>
    /// .avi
    /// </summary>
    public const string Avi = ".avi";

    /// <summary>
    /// .rm
    /// </summary>
    public const string Rm = ".rm";

    /// <summary>
    /// .rmvb
    /// </summary>
    public const string Rmvb = ".rmvb";

    /// <summary>
    /// .mov
    /// </summary>
    public const string Mov = ".mov";

    /// <summary>
    /// .wmv
    /// </summary>
    public const string Wmv = ".wmv";

    /// <summary>
    /// .flv
    /// </summary>
    public const string Flv = ".flv";

    /// <summary>
    /// .gifv
    /// </summary>
    public const string Gifv = ".gifv";

    /// <summary>
    /// .mkv
    /// </summary>
    public const string Mkv = ".mkv";

    /// <summary>
    /// .qt
    /// </summary>
    public const string Qt = ".qt";

    static IEnumerable<string>? _Videos;

    /// <summary>
    /// 视频
    /// </summary>
    public static IEnumerable<string> Videos
    {
        get
        {
            if (_Videos == null) return GetVideos();
            return _Videos;
        }

        set
        {
            _Videos = value;
        }
    }

    static IEnumerable<string> GetVideos()
    {
        yield return Mp4;
        yield return M4v;
        yield return Mpg;
        yield return Mpeg;
        yield return Mp2;
        yield return Dat;
        yield return Asf;
        yield return Avi;
        yield return Rm;
        yield return Rmvb;
        yield return Mov;
        yield return Wmv;
        yield return Flv;
        yield return Gifv;
        yield return Mkv;
        yield return Qt;
    }

    /// <summary>
    /// 判断文件扩展名是否为视频
    /// </summary>
    /// <param name="fileEx"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVideo(string fileEx)
        => Videos.Any(x
            => string.Equals(x, fileEx, StringComparison.OrdinalIgnoreCase));
}