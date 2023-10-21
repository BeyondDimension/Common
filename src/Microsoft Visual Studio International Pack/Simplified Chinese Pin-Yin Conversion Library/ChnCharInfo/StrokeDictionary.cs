// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.StrokeDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class StrokeDictionary
{
    internal const short MaxStrokeNumber = 48;
    internal int Length;
    internal int Count;
    internal short Offset;
    internal readonly byte[] Reserved = new byte[24];
    internal List<StrokeUnit> StrokeUnitTable = null!;
    internal readonly short EndMark = short.MaxValue;

    internal void Serialize(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(Length);
        binaryWriter.Write(Count);
        binaryWriter.Write(Offset);
        binaryWriter.Write(Reserved);
        for (int index = 0; index < Count; ++index)
            StrokeUnitTable[index].Serialize(binaryWriter);
        binaryWriter.Write(EndMark);
    }

    internal static StrokeDictionary Deserialize(BinaryReader binaryReader)
    {
        StrokeDictionary strokeDictionary = new StrokeDictionary();
        binaryReader.ReadInt32();
        strokeDictionary.Length = binaryReader.ReadInt32();
        strokeDictionary.Count = binaryReader.ReadInt32();
        strokeDictionary.Offset = binaryReader.ReadInt16();
        binaryReader.ReadBytes(24);
        strokeDictionary.StrokeUnitTable = [];
        for (int index = 0; index < strokeDictionary.Count; ++index)
            strokeDictionary.StrokeUnitTable.Add(StrokeUnit.Deserialize(binaryReader));
        _ = binaryReader.ReadInt16();
        return strokeDictionary;
    }

    internal StrokeUnit GetStrokeUnitByIndex(int index) => index >= 0 && index < Count ? StrokeUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);

    internal StrokeUnit GetStrokeUnit(int strokeNum) => strokeNum > 0 && strokeNum <= 48 ? StrokeUnitTable.Find(new StrokeUnitPredicate(strokeNum).Match)! : throw new ArgumentOutOfRangeException(nameof(strokeNum));
}
