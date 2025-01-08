namespace BD.Common8.Pinyin.Services.Implementation;

/// <summary>
/// 使用 Microsoft Visual Studio International Pack 1.0 中的 Simplified Chinese Pin-Yin Conversion Library（简体中文拼音转换类库）实现的拼音功能
/// </summary>
sealed class ChnCharInfoPinyinImpl : IPinyin
{
    /// <inheritdoc/>
    bool IPinyin.IsChinese(char c)
    {
        return ChineseChar.IsValidChar(c);
    }

    static IEnumerable<string> GetPinyins(string s)
    {
        foreach (var c in s)
        {
            string? r = null;
            try
            {
                if (ChineseChar.IsValidChar(c))
                {
                    var cc = new ChineseChar(c);
                    var py = cc.Pinyins.FirstOrDefault();
                    if (py != null)
                        if (py.Length > 1)
                            r = py[0..^1];
                        else
                            r = py;
                }
            }
            catch
            {
            }
            yield return r ?? c.ToString();
        }
    }

    /// <inheritdoc/>
    string IPinyin.GetPinyin(string s, PinyinFormat format)
    {
        var r = GetPinyins(s);
        return format switch
        {
            PinyinFormat.UpperVerticalBar => string.Join(PinyinHelper.SeparatorVerticalBar, r),
            PinyinFormat.AlphabetSort => string.Join(null, r),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
        };
    }

    /// <inheritdoc/>
    string[] IPinyin.GetPinyinArray(string s) => GetPinyins(s).ToArray();
}