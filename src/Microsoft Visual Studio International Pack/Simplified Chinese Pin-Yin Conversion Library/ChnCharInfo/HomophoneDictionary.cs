// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.HomophoneDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class HomophoneDictionary
{
    internal int Length;
    internal short Offset;
    internal short Count;
    internal readonly byte[] Reserved = new byte[8];
    internal List<HomophoneUnit> HomophoneUnitTable = null!;
    internal readonly short EndMark = short.MaxValue;

    internal void Serialize(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(Length);
        binaryWriter.Write(Count);
        binaryWriter.Write(Offset);
        binaryWriter.Write(Reserved);
        for (int index = 0; index < Count; ++index)
            HomophoneUnitTable[index].Serialize(binaryWriter);
        binaryWriter.Write(EndMark);
    }

    internal static HomophoneDictionary Deserialize(BinaryReader binaryReader)
    {
        HomophoneDictionary homophoneDictionary = new HomophoneDictionary();
        binaryReader.ReadInt32();
        homophoneDictionary.Length = binaryReader.ReadInt32();
        homophoneDictionary.Count = binaryReader.ReadInt16();
        homophoneDictionary.Offset = binaryReader.ReadInt16();
        binaryReader.ReadBytes(8);
        homophoneDictionary.HomophoneUnitTable = [];
        for (int index = 0; index < homophoneDictionary.Count; ++index)
            homophoneDictionary.HomophoneUnitTable.Add(HomophoneUnit.Deserialize(binaryReader));
        _ = binaryReader.ReadInt16();
        return homophoneDictionary;
    }

    internal HomophoneUnit GetHomophoneUnit(int index) => index >= 0 && index < Count ? HomophoneUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);

    internal HomophoneUnit GetHomophoneUnit(
      PinyinDictionary pinyinDictionary,
      string pinyin)
    {
        return GetHomophoneUnit(pinyinDictionary.GetPinYinUnitIndex(pinyin));
    }
}
