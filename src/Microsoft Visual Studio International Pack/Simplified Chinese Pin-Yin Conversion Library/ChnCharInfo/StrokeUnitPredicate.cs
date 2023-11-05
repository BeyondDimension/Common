// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.StrokeUnitPredicate
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 用于比对笔画单元的数量
/// </summary>
sealed class StrokeUnitPredicate
{
    /// <summary>
    /// 预期的笔画数量
    /// </summary>
    readonly int ExpectedStrokeNum;

    /// <summary>
    /// 传入笔画数量，并将其赋值给 <see cref="ExpectedStrokeNum"/> 字段
    /// </summary>
    internal StrokeUnitPredicate(int strokeNum) => ExpectedStrokeNum = strokeNum;

    /// <summary>
    /// 比对给定的笔画和预期的笔画数量是否相等
    /// </summary>
    internal bool Match(StrokeUnit strokeUnit) => strokeUnit.StrokeNumber == ExpectedStrokeNum;
}
