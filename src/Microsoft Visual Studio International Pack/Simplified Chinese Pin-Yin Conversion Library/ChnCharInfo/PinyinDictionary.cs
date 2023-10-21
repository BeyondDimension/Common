// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.PinyinDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class PinyinDictionary
{
    internal short Length;
    internal short Count;
    internal short Offset;
    internal readonly byte[] Reserved = new byte[8];
    internal List<PinyinUnit> PinyinUnitTable = null!;
    internal readonly short EndMark = short.MaxValue;

    internal void Serialize(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(Length);
        binaryWriter.Write(Count);
        binaryWriter.Write(Offset);
        binaryWriter.Write(Reserved);
        for (int index = 0; index < Count; ++index)
            PinyinUnitTable[index].Serialize(binaryWriter);
        binaryWriter.Write(EndMark);
    }

    internal static PinyinDictionary Deserialize(BinaryReader binaryReader)
    {
        PinyinDictionary pinyinDictionary = new PinyinDictionary();
        binaryReader.ReadInt32();
        pinyinDictionary.Length = binaryReader.ReadInt16();
        pinyinDictionary.Count = binaryReader.ReadInt16();
        pinyinDictionary.Offset = binaryReader.ReadInt16();
        binaryReader.ReadBytes(8);
        pinyinDictionary.PinyinUnitTable = [];
        for (int index = 0; index < pinyinDictionary.Count; ++index)
            pinyinDictionary.PinyinUnitTable.Add(PinyinUnit.Deserialize(binaryReader));
        _ = binaryReader.ReadInt16();
        return pinyinDictionary;
    }

    internal int GetPinYinUnitIndex(string pinyin) => PinyinUnitTable.FindIndex(new PinyinUnitPredicate(pinyin).Match);

    internal PinyinUnit GetPinYinUnit(string pinyin) => PinyinUnitTable.Find(new PinyinUnitPredicate(pinyin).Match)!;

    internal PinyinUnit GetPinYinUnitByIndex(int index) => index >= 0 && index < Count ? PinyinUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);
}
