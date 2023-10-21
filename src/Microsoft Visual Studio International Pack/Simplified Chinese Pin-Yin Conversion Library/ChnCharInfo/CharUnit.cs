// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.CharUnit
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class CharUnit
{
    internal char Char;
    internal byte StrokeNumber;
    internal byte PinyinCount;
    internal short[] PinyinIndexList = null!;

    internal static CharUnit Deserialize(BinaryReader binaryReader)
    {
        CharUnit charUnit = new()
        {
            Char = binaryReader.ReadChar(),
            StrokeNumber = binaryReader.ReadByte(),
            PinyinCount = binaryReader.ReadByte(),
        };
        charUnit.PinyinIndexList = new short[charUnit.PinyinCount];
        for (int index = 0; index < charUnit.PinyinCount; ++index)
            charUnit.PinyinIndexList[index] = binaryReader.ReadInt16();
        return charUnit;
    }

    internal void Serialize(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(Char);
        binaryWriter.Write(StrokeNumber);
        binaryWriter.Write(PinyinCount);
        for (int index = 0; index < PinyinCount; ++index)
            binaryWriter.Write(PinyinIndexList[index]);
    }
}
