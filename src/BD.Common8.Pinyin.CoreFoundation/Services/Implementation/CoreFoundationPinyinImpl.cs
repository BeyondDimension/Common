namespace BD.Common8.Pinyin.Services.Implementation;

/// <summary>
/// 使用 <see cref="CFStringTransform"/> 实现的拼音功能
/// </summary>
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("maccatalyst")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("tvos")]
sealed class CoreFoundationPinyinImpl : IPinyin
{
    /// <summary>
    /// 将指定的字符串转换拉丁字母表示形式
    /// </summary>
    /// <returns></returns>
    public static bool TransformMandarinLatin(string s, out CFMutableString str)
    {
        str = new CFMutableString(s);
        return str.Transform(CFStringTransform.MandarinLatin, false);
    }

    /// <summary>
    /// 获取指定字符串的拼音表示形式
    /// </summary>
    public static bool GetPinyin(string s, out CFMutableString str)
        => TransformMandarinLatin(s, out str) &&
        str.Transform(CFStringTransform.StripDiacritics, false);

    /// <inheritdoc/>
    string IPinyin.GetPinyin(string s, PinyinFormat format)
    {
        if (GetPinyin(s, out var str))
        {
            s = str!;
            s = format switch
            {
                PinyinFormat.UpperVerticalBar => s.Replace(' ', PinyinHelper.SeparatorVerticalBar).ToUpper(),
                PinyinFormat.AlphabetSort => s,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
        }
        return s;
    }

    /// <inheritdoc/>
    string[] IPinyin.GetPinyinArray(string s)
    {
        if (GetPinyin(s, out var str))
        {
            s = str!;
            return s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
        return [];
    }

    /// <inheritdoc/>
    bool IPinyin.IsChinese(char c) => TransformMandarinLatin(c.ToString(), out var _);
}