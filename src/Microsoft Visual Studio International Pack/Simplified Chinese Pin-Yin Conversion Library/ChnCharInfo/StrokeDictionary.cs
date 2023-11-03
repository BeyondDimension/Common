// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.StrokeDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 笔画词典
/// </summary>
sealed class StrokeDictionary
{
    /// <summary>
    /// 最大笔画数
    /// </summary>
    internal const short MaxStrokeNumber = 48;

    /// <summary>
    /// 长度
    /// </summary>
    internal int Length;

    /// <summary>
    /// 数量
    /// </summary>
    internal int Count;

    /// <summary>
    /// 偏移量
    /// </summary>
    internal short Offset;

    /// <summary>
    /// 保留字节
    /// </summary>
    internal readonly byte[] Reserved = new byte[24];

    /// <summary>
    /// 笔画单元表
    /// </summary>
    internal List<StrokeUnit> StrokeUnitTable = null!;

    /// <summary>
    /// 结束标记
    /// </summary>
    internal readonly short EndMark = short.MaxValue;

    /// <summary>
    /// 将 <see cref="StrokeDictionary"/> 对象序列化为 <see cref="BinaryWriter"/>
    /// </summary>
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

    /// <summary>
    /// 将 <see cref="BinaryWriter"/> 对象反序列化为 <see cref="StrokeDictionary"/>
    /// </summary>
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

    /// <summary>
    /// 根据给定的索引值获取在笔画单元表中对应位置的笔画单元
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal StrokeUnit GetStrokeUnitByIndex(int index) => index >= 0 && index < Count ? StrokeUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);

    /// <summary>
    /// 根据给定的笔画数获取对应的笔画单元
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal StrokeUnit GetStrokeUnit(int strokeNum) => strokeNum > 0 && strokeNum <= 48 ? StrokeUnitTable.Find(new StrokeUnitPredicate(strokeNum).Match)! : throw new ArgumentOutOfRangeException(nameof(strokeNum));
}
