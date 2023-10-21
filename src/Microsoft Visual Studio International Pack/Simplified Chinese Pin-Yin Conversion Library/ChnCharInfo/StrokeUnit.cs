// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.StrokeUnit
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class StrokeUnit
{
    internal byte StrokeNumber;
    internal short CharCount;
    internal char[] CharList = null!;

    internal static StrokeUnit Deserialize(BinaryReader binaryReader)
    {
        StrokeUnit strokeUnit = new()
        {
            StrokeNumber = binaryReader.ReadByte(),
            CharCount = binaryReader.ReadInt16(),
        };
        strokeUnit.CharList = new char[strokeUnit.CharCount];
        for (int index = 0; index < strokeUnit.CharCount; ++index)
            strokeUnit.CharList[index] = binaryReader.ReadChar();
        return strokeUnit;
    }

    internal void Serialize(BinaryWriter binaryWriter)
    {
        if (CharCount == 0)
            return;
        binaryWriter.Write(StrokeNumber);
        binaryWriter.Write(CharCount);
        binaryWriter.Write(CharList);
    }
}
