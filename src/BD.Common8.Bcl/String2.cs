namespace System;

/// <inheritdoc cref="string"/>
public static partial class String2
{
    /// <summary>
    /// 数字
    /// </summary>
    public const string Digits = "0123456789";

    /// <summary>
    /// 大写字母
    /// </summary>
    public const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// 小写字母
    /// </summary>
    public const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// 字母
    /// </summary>
    public const string Letters = LowerCaseLetters + UpperCaseLetters;

    /// <summary>
    /// 数字与字母
    /// </summary>
    public const string DigitsLetters = Digits + Letters;

    /// <summary>
    /// utf-8
    /// </summary>
    public const string UTF8 = "utf-8";

    /// <summary>
    /// #
    /// </summary>
    public const string Sharp = "#";

    /// <summary>
    /// .
    /// </summary>
    public const char DOT = '.';

    /// <summary>
    /// N
    /// </summary>
    public const string N = "N";

    /// <summary>
    /// https://
    /// </summary>
    public const string Prefix_HTTPS = "https://";

    /// <summary>
    /// http://
    /// </summary>
    public const string Prefix_HTTP = "http://";

    /// <summary>
    /// ms-windows-store://
    /// </summary>
    public const string Prefix_MSStore = "ms-windows-store://";

    /// <summary>
    /// mailto:
    /// </summary>
    public const string Prefix_Email = "mailto:";

    /// <summary>
    /// file:///
    /// </summary>
    public const string Prefix_File = "file:///";

    /// <summary>
    /// 判断字符串是否为 Http Url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="httpsOnly">是否仅Https</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHttpUrl([NotNullWhen(true)] string? url, bool httpsOnly = false) => url != null &&
        (url.StartsWith(Prefix_HTTPS, StringComparison.OrdinalIgnoreCase) ||
              (!httpsOnly && url.StartsWith(Prefix_HTTP, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    /// 判断字符串是否为 Store Url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStoreUrl([NotNullWhen(true)] string? url)
        => url != null && url.StartsWith(Prefix_MSStore, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 判断字符串是否为 Email Url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmailUrl([NotNullWhen(true)] string? url)
        => url != null && url.StartsWith(Prefix_Email, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 判断字符串是否为 File Url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFileUrl([NotNullWhen(true)] string? url)
        => url != null && url.StartsWith(Prefix_File, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 将字符串尝试转换为 <see cref="Version"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseVersion(string? value, [NotNullWhen(true)] out Version? version)
    {
        version = default;
        if (value != null)
        {
            if (Version.TryParse(value, out version))
            {
                return true;
            }
            if (int.TryParse(value, out var major))
            {
                version = new Version(major, 0);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 0.0.0.0
    /// </summary>
    public const string VersionZero = "0.0.0.0";

    /// <summary>
    /// 将 <see cref="Version"/> 转换为 4 位完整长度补 0 的字符串
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ToString(Version? version)
    {
        if (version == null)
            return null;
        if (version.Revision >= 0)
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        if (version.Build >= 0)
            return $"{version.Major}.{version.Minor}.{version.Build}.0";
        if (version.Minor >= 0)
            return $"{version.Major}.{version.Minor}.0.0";
        if (version.Major >= 0)
            return $"{version.Major}.0.0.0";
        return VersionZero;
    }
}
