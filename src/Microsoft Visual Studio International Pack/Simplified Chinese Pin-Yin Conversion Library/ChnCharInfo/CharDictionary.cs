// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.CharDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class CharDictionary
{
    internal int Length;
    internal int Count;
    internal short Offset;
    internal readonly byte[] Reserved = new byte[24];
    internal List<CharUnit> CharUnitTable = null!;
    internal readonly short EndMark = short.MaxValue;

    internal void Serialize(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(Length);
        binaryWriter.Write(Count);
        binaryWriter.Write(Offset);
        binaryWriter.Write(Reserved);
        for (int index = 0; index < Count; ++index)
            CharUnitTable[index].Serialize(binaryWriter);
        binaryWriter.Write(EndMark);
    }

    internal static CharDictionary Deserialize(BinaryReader binaryReader)
    {
        CharDictionary charDictionary = new();
        binaryReader.ReadInt32();
        charDictionary.Length = binaryReader.ReadInt32();
        charDictionary.Count = binaryReader.ReadInt32();
        charDictionary.Offset = binaryReader.ReadInt16();
        binaryReader.ReadBytes(24);
        charDictionary.CharUnitTable = [];
        for (int index = 0; index < charDictionary.Count; ++index)
            charDictionary.CharUnitTable.Add(CharUnit.Deserialize(binaryReader));
        _ = binaryReader.ReadInt16();
        return charDictionary;
    }

    internal CharUnit GetCharUnit(int index) => index >= 0 && index < Count ? CharUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);

    internal CharUnit GetCharUnit(char ch) => CharUnitTable.Find(new CharUnitPredicate(ch).Match)!;
}
