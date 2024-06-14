// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.ChineseChar
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>封装了简体中文的读音和笔画等基本信息。</summary>
[BinaryResource(
"""
[
  {
    "Path": "CharDictionary",
    "Name": "_CharDictionary"
  },
  {
    "Path": "HomophoneDictionary",
    "Name": "_HomophoneDictionary"
  },
  {
    "Path": "PinyinDictionary",
    "Name": "_PinyinDictionary"
  },
  {
    "Path": "StrokeDictionary",
    "Name": "_StrokeDictionary"
  }
]
""")]
public sealed partial class ChineseChar
{
    static readonly CharDictionary charDictionary;
    static readonly PinyinDictionary pinyinDictionary;
    static readonly StrokeDictionary strokeDictionary;
    static readonly HomophoneDictionary homophoneDictionary;
    readonly char chineseCharacter;
    readonly short strokeNumber;
    readonly bool isPolyphone;
    readonly short pinyinCount;
    readonly string[] pinyinList = new string[8];

    static ChineseChar()
    {
        unsafe
        {
            var bytes = _PinyinDictionary();
            fixed (byte* ptr = bytes)
            {
                using UnmanagedMemoryStream stream = new(ptr, bytes.Length);
                using BinaryReader binaryReader = new BinaryReader(stream);
                pinyinDictionary = PinyinDictionary.Deserialize(binaryReader);
                new Span<byte>(ptr, bytes.Length).Clear();
            }
        }
        unsafe
        {
            var bytes = _CharDictionary();
            fixed (byte* ptr = bytes)
            {
                using UnmanagedMemoryStream stream = new(ptr, bytes.Length);
                using BinaryReader binaryReader = new BinaryReader(stream);
                charDictionary = CharDictionary.Deserialize(binaryReader);
                new Span<byte>(ptr, bytes.Length).Clear();
            }
        }
        unsafe
        {
            var bytes = _HomophoneDictionary();
            fixed (byte* ptr = bytes)
            {
                using UnmanagedMemoryStream stream = new(ptr, bytes.Length);
                using BinaryReader binaryReader = new BinaryReader(stream);
                homophoneDictionary = HomophoneDictionary.Deserialize(binaryReader);
                new Span<byte>(ptr, bytes.Length).Clear();
            }
        }
        unsafe
        {
            var bytes = _StrokeDictionary();
            fixed (byte* ptr = bytes)
            {
                using UnmanagedMemoryStream stream = new(ptr, bytes.Length);
                using BinaryReader binaryReader = new BinaryReader(stream);
                strokeDictionary = StrokeDictionary.Deserialize(binaryReader);
                new Span<byte>(ptr, bytes.Length).Clear();
            }
        }
    }

    /// <summary>ChineseChar 类的构造函数</summary>
    /// <param name="ch">指定的汉字字符</param>
    /// <exception cref="T:System.NotSupportedException">
    ///   字符不在简体中文扩展字符集中
    /// </exception>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public ChineseChar(char ch)
    {
        chineseCharacter = IsValidChar(ch) ? ch : throw new NotSupportedException(AssemblyResource.CHARACTER_NOT_SUPPORTED);
        var charUnit = charDictionary.GetCharUnit(ch);
        strokeNumber = charUnit.StrokeNumber;
        pinyinCount = charUnit.PinyinCount;
        isPolyphone = charUnit.PinyinCount > 1;
        for (int index = 0; index < pinyinCount; ++index)
        {
            PinyinUnit pinYinUnitByIndex = pinyinDictionary.GetPinYinUnitByIndex(charUnit.PinyinIndexList[index]);
            pinyinList[index] = pinYinUnitByIndex.Pinyin;
        }
    }

    /// <summary>获取这个字符的拼音个数。</summary>
    /// <value>这个字符的拼音数。</value>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public short PinyinCount => pinyinCount;

    /// <summary>获取这个字符的笔画数。</summary>
    /// <value>这个字符的笔画数。</value>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public short StrokeNumber => strokeNumber;

    /// <summary>获取这个字符是否是多音字。</summary>
    /// <value>这个布尔型的字符是否是多音字。</value>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例。
    /// </remarks>
    public bool IsPolyphone => isPolyphone;

    /// <summary>获取这个字符的拼音。</summary>
    /// <value>这个字符的拼音。</value>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    ///  </remarks>
    public ReadOnlyCollection<string> Pinyins => new(pinyinList);

    /// <summary>获取这个汉字字符</summary>
    /// <value>汉字字符</value>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public char ChineseCharacter => chineseCharacter;

    /// <summary>识别字符是否有指定的读音</summary>
    /// <param name="pinyin">指定的需要被识别的拼音</param>
    /// <returns>如果指定的拼音字符串在实例字符的拼音集合中则返回 ture，否则返回 false</returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">拼音是一个空引用。</exception>
    public bool HasSound(string pinyin)
    {
        ArgumentNullException.ThrowIfNull(pinyin);
        for (int index = 0; index < PinyinCount; ++index)
        {
            if (string.Compare(Pinyins[index], pinyin, true, CultureInfo.CurrentCulture) == 0)
                return true;
        }
        return false;
    }

    /// <summary>识别给出的字符是否是实例字符的同音字</summary>
    /// <param name="ch">指出需要识别的字符</param>
    /// <returns>如果给出的实例字符是同音字则返回 ture，否则返回 false</returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例。
    /// </remarks>
    public bool IsHomophone(char ch) => IsHomophone(chineseCharacter, ch);

    /// <summary>识别给出的两个字符是否是同音字</summary>
    /// <param name="ch1">指出第一个字符</param>
    /// <param name="ch2">指出第二个字符</param>
    /// <returns>如果给出的字符是同音字返回 ture，否则返回 false</returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public static bool IsHomophone(char ch1, char ch2) => ExistSameElement(charDictionary.GetCharUnit(ch1).PinyinIndexList, charDictionary.GetCharUnit(ch2).PinyinIndexList);

    /// <summary>将给出的字符和实例字符的笔画数进行比较</summary>
    /// <param name="ch">显示给出的字符</param>
    /// <returns>
    /// <list>说明比较操作的结果
    /// <item>如果给出字符和实例字符的笔画数相同，返回值为 0</item>
    /// <item>如果实例字符比给出字符的笔画多，返回值为&gt; 0</item>
    /// <item>如果实例字符比给出字符的笔画少，返回值为&lt; 0</item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public int CompareStrokeNumber(char ch) => StrokeNumber - charDictionary.GetCharUnit(ch).StrokeNumber;

    /// <summary>
    /// 获取给定拼音的所有同音字
    /// </summary>
    /// <param name="pinyin">指出读音</param>
    /// <returns>
    ///  返回具有相同的指定拼音的字符串列表，如果拼音不是有效值则返回空
    /// </returns>
    /// <remarks>
    ///  请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">拼音是一个空引用</exception>
    public static char[]? GetChars(string pinyin)
    {
        ArgumentNullException.ThrowIfNull(pinyin);
        return !IsValidPinyin(pinyin) ? null : homophoneDictionary.GetHomophoneUnit(pinyinDictionary, pinyin).HomophoneList;
    }

    /// <summary>识别给出的拼音是否是一个有效的拼音字符串</summary>
    /// <param name="pinyin">指出需要识别的字符串</param>
    /// <returns>如果指定的字符串是一个有效的拼音字符串则返回 ture，否则返回 false</returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar
    ///  </remarks>
    /// <exception cref="T:System.ArgumentNullException">拼音是一个空引用</exception>
    public static bool IsValidPinyin(string pinyin)
    {
        ArgumentNullException.ThrowIfNull(pinyin);
        return pinyinDictionary.GetPinYinUnitIndex(pinyin) >= 0;
    }

    /// <summary>识别给出的字符串是否是一个有效的汉字字符</summary>
    /// <param name="ch">指出需要识别的字符</param>
    /// <returns>如果指定的字符是一个有效的汉字字符则返回 ture，否则返回 false</returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public static bool IsValidChar(char ch) => charDictionary.GetCharUnit(ch) != null;

    /// <summary>识别给出的笔画数是否是一个有效的笔画数</summary>
    /// <param name="strokeNumber">指出需要识别的笔画数</param>
    /// <returns>如果指定的笔画数是一个有效的笔画数则返回 ture，否则返回 false</returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" />来查看使用 ChineseChar 的实例
    /// </remarks>
    public static bool IsValidStrokeNumber(short strokeNumber) => strokeNumber >= 0 && strokeNumber <= 48 && strokeDictionary.GetStrokeUnit(strokeNumber) != null;

    /// <summary>检索具有指定拼音的字符数</summary>
    /// <param name="pinyin">显示需要被识别的拼音字符串</param>
    /// <returns>
    /// 返回具有指定拼音的字符数。
    /// 如果拼音不是有效值则返回-1。
    /// </returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">拼音是一个空引用</exception>
    public static short GetHomophoneCount(string pinyin)
    {
        ArgumentNullException.ThrowIfNull(pinyin);
        return !IsValidPinyin(pinyin) ? (short)-1 : homophoneDictionary.GetHomophoneUnit(pinyinDictionary, pinyin).Count;
    }

    /// <summary>检索指定字符的笔画数</summary>
    /// <param name="ch">指出需要识别的字符</param>
    /// <returns>
    /// 返回指定字符的笔画数。
    /// 如果字符不是有效值则返回 -1。
    /// </returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public static short GetStrokeNumber(char ch) => !IsValidChar(ch) ? (short)-1 : charDictionary.GetCharUnit(ch).StrokeNumber;

    /// <summary>检索具有指定笔画数的所有字符串</summary>
    /// <param name="strokeNumber">指出需要被识别的笔画数</param>
    /// <returns>
    /// 返回具有指定笔画数的字符列表。
    /// 如果笔画数是无效值返回空。
    /// </returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public static char[]? GetChars(short strokeNumber) => !IsValidStrokeNumber(strokeNumber) ? null : strokeDictionary.GetStrokeUnit(strokeNumber).CharList;

    /// <summary>检索具有指定笔画数的字符个数。</summary>
    /// <param name="strokeNumber">显示需要被识别的笔画数。</param>
    /// <returns>
    /// 返回具有指定笔画数的字符数。
    /// 如果笔画数是无效值返回-1。
    /// </returns>
    /// <remarks>
    /// 请参阅 <see cref="T:Microsoft.International.Converters.PinYinConverter.ChineseChar" /> 来查看使用 ChineseChar 的实例
    /// </remarks>
    public static short GetCharCount(short strokeNumber) => !IsValidStrokeNumber(strokeNumber) ? (short)-1 : strokeDictionary.GetStrokeUnit(strokeNumber).CharCount;

    static bool ExistSameElement<T>(T[] array1, T[] array2) where T : IComparable
    {
        int index1 = 0;
        int index2 = 0;
        while (index1 < array1.Length && index2 < array2.Length)
        {
            if (array1[index1].CompareTo(array2[index2]) < 0)
            {
                ++index1;
            }
            else
            {
                if (array1[index1].CompareTo(array2[index2]) <= 0)
                    return true;
                ++index2;
            }
        }
        return false;
    }
}
