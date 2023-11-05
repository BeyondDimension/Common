// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.PinyinUnitPredicate
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 提供检查拼音单元是否与预期拼音匹配的功能
/// </summary>
sealed class PinyinUnitPredicate
{
    /// <summary>
    /// 存储预期拼音
    /// </summary>
    readonly string ExpectedPinyin;

    /// <summary>
    /// 传入拼音，并将其赋值给 <see cref="ExpectedPinyin"/> 字段
    /// </summary>
    /// <param name="pinyin"></param>
    internal PinyinUnitPredicate(string pinyin) => ExpectedPinyin = pinyin;

    /// <summary>
    /// 用于检查给定的 <see cref="PinyinUnit"/> 是否与预期的拼音匹配
    /// </summary>
    /// <param name="pinyinUnit"></param>
    /// <returns></returns>
    internal bool Match(PinyinUnit pinyinUnit) => string.Compare(pinyinUnit.Pinyin, ExpectedPinyin, true, CultureInfo.CurrentCulture) == 0;
}
