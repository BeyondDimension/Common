// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.StrokeUnitPredicate
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class StrokeUnitPredicate
{
    readonly int ExpectedStrokeNum;

    internal StrokeUnitPredicate(int strokeNum) => ExpectedStrokeNum = strokeNum;

    internal bool Match(StrokeUnit strokeUnit) => strokeUnit.StrokeNumber == ExpectedStrokeNum;
}
