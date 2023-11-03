// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.CharUnitPredicate
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 字符单元
/// </summary>
sealed class CharUnitPredicate
{
    /// <summary>
    /// 存储期望匹配的字符
    /// </summary>
    readonly char ExpectedChar;

    /// <summary>
    /// 初始化 <see cref="CharUnitPredicate"/> 实例
    /// </summary>
    /// <param name="ch"></param>
    internal CharUnitPredicate(char ch) => ExpectedChar = ch;

    /// <summary>
    /// 用于比较给定的字符单元与期望字符是否一致
    /// </summary>
    internal bool Match(CharUnit charUnit) => charUnit.Char == ExpectedChar;
}
