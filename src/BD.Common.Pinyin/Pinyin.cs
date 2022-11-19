namespace BD.Common;

/// <inheritdoc cref="IPinyin"/>
public static class Pinyin
{
    const string TAG = "Pinyin";

    /// <summary>
    /// #
    /// </summary>
    const string Sharp = "#";

    /// <summary>
    /// 竖线分隔符
    /// </summary>
    public const char SeparatorVerticalBar = '|';

    /// <summary>
    /// 拼音分隔符(单引号)
    /// </summary>
    public const char Separator = '\'';

    /// <summary>
    /// 中文拼音分隔符(单引号)
    /// </summary>
    public const char SeparatorZH = '’';

    /// <inheritdoc cref="IPinyin.IsChinese(char)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsChinese(char c) => IPinyin.Instance.IsChinese(c);

    /// <inheritdoc cref="IPinyin.GetPinyin(string, PinyinFormat)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetPinyin(string s, PinyinFormat format)
        => IPinyin.Instance.GetPinyin(s, format);

    /// <inheritdoc cref="IPinyin.GetPinyinArray(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] GetPinyinArray(string s)
        => IPinyin.Instance.GetPinyinArray(s);

    /// <summary>
    /// 获取字母表排序值
    /// </summary>
    /// <param name="s"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    public static string GetAlphabetSort(string s, string def = Sharp)
    {
        if (!string.IsNullOrWhiteSpace(s))
        {
            #region V1

            //var firstChar = s[0]; // 第一个字符
            //var index = Constants.UpperCaseLetters.IndexOf(firstChar); // 第一个字符是大写字母则返回大写字母。
            //if (index >= 0) return Constants.UpperCaseLetters[index].ToString();
            //index = Constants.LowerCaseLetters.IndexOf(firstChar); // 查找小写字母
            //if (index >= 0) return Constants.UpperCaseLetters[index].ToString(); // 找到了则返回大写字母。

            //var pinyin = IPinyin.Instance;
            //if (pinyin.IsChinese(firstChar))
            //{
            //    return pinyin.GetPinyin(s, PinyinFormat.AlphabetSort);
            //}
            //else
            //{
            //    return def + s;
            //}

            #endregion

            #region V2

            s = GetPinyin(s, PinyinFormat.AlphabetSort).ToUpper();
            var firstChar = s[0]; // 第一个字符
            var index = String2.UpperCaseLetters.IndexOf(firstChar);
            return index >= 0 ? s : def + s;

            #endregion
        }
        return def;
    }

    /// <summary>
    /// 搜索比较，返回是否匹配
    /// </summary>
    /// <param name="inputText">输入的搜索文本内容</param>
    /// <param name="name">名称</param>
    /// <param name="pinyinArray">名称对应的拼音数组，可使用 <see cref="GetPinyinArray(string)"/> 获取，最好在模型类中定义一个属性存放 <see cref="string[]"/> 拼音数组，且当 Name 发生改变时重新赋值</param>
    /// <param name="ignoreOtherChar">比较时是否忽略其他字符</param>
    /// <param name="comparisonType"></param>
    /// <returns></returns>
    public static bool SearchCompare_Nullable(string? inputText, string name, string[] pinyinArray, bool ignoreOtherChar = false, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        if (string.IsNullOrEmpty(inputText)) return true; // 空值全部匹配。
        return SearchCompare(inputText, name, pinyinArray, ignoreOtherChar, comparisonType);
    }

    /// <inheritdoc cref="SearchCompare_Nullable(string?, string, string[], bool, StringComparison)"/>
    public static bool SearchCompare(string inputText, string name, string[] pinyinArray, bool ignoreOtherChar = false, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        if (name.Contains(inputText, comparisonType)) return true;
        if (!pinyinArray.Any_Nullable()) return false;

        #region 汉字拼音混合比较算法

        try
        {
            var index = 0; // 输入分词数组的比较下标
            for (int i = 0; i < pinyinArray!.Length; i++) // 循环所有字的拼音
            {
                var item_pinyin = pinyinArray[i]; // 当前字的拼音
                if (index >= inputText.Length) return true; // 下标溢出即搜索完毕，返回比较成功。
                int cache_index = 0;
                var break_next = false;
                var ignore_separator = true; // 本次循环忽略分隔符。
                for (; index < inputText.Length; index++) // 循环输入内容
                {
                    if (break_next) break;
                    var input_item = inputText[index]; // 当前输入内容字符
                    if (input_item == Separator || input_item == SeparatorZH) // 拼音分隔分号字符无视
                    {
                        if (index == 0) return false; // 第一个字符不能是分隔分号开头
                        if (i < pinyinArray.Length - 1) // 前面的可以输入分隔符跳过
                        {
                            if (ignore_separator)
                            {
                                ignore_separator = !ignore_separator;
                            }
                            else
                            {
                                break_next = true;
                            }
                        }
                        continue;
                    }
                    var isChinese = IsChinese(input_item); // 当前字符是否为中文字符。
                    if (isChinese)
                    {
                        if (name[i] != input_item) return false;
                        else continue;
                    }
                    // ↑ 当前字符为中文字符比较内容不正确返回比较失败。
                    char letter = default;

                    var indexOfUpperCase = String2.UpperCaseLetters.IndexOf(input_item); // 大写字母搜索
                    if (indexOfUpperCase >= 0) letter = String2.UpperCaseLetters[indexOfUpperCase];
                    var indexOfLowerCase = String2.LowerCaseLetters.IndexOf(input_item); // 小写字母搜索
                    if (indexOfLowerCase >= 0) letter = String2.UpperCaseLetters[indexOfLowerCase];

                    var letterHasValue = letter != default;

                    if (!letterHasValue && !(ignoreOtherChar && index != 0)) return false; // 无效字符是否允许

                    var item_pinyin_letter_upper = item_pinyin[cache_index];
                    indexOfLowerCase = String2.LowerCaseLetters.IndexOf(item_pinyin_letter_upper);
                    if (indexOfLowerCase >= 0) item_pinyin_letter_upper = String2.UpperCaseLetters[indexOfLowerCase];

                    if (letterHasValue && item_pinyin_letter_upper != letter) return false;
                    else cache_index += 1;
                    if (cache_index >= item_pinyin.Length)
                    {
                        ignore_separator = true;
                        break_next = true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(TAG, e, "SearchCompare catch, name: {0}, inputText: {1}", name, inputText);
            return false;
        }

        return true;

        #endregion
    }

    public static string[] GetPinyin(string s, IDictionary<string, string[]> dict)
    {
        string[] pinyinArray;
        if (!dict.ContainsKey(s))
        {
            dict[s] = pinyinArray = GetPinyinArray(s);
        }
        else
        {
            pinyinArray = dict[s];
        }
        return pinyinArray;
    }
}