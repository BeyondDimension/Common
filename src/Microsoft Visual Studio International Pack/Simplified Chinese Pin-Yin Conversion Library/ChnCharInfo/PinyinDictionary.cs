// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.PinyinDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 拼音词典
/// </summary>
sealed class PinyinDictionary
{
    /// <summary>
    /// 长度
    /// </summary>
    internal short Length;

    /// <summary>
    /// 数量
    /// </summary>
    internal short Count;

    /// <summary>
    ///  偏移量
    /// </summary>
    internal short Offset;

    /// <summary>
    /// 保留数组
    /// </summary>
    internal readonly byte[] Reserved = new byte[8];

    /// <summary>
    /// 拼音单元表
    /// </summary>
    internal List<PinyinUnit> PinyinUnitTable = null!;

    /// <summary>
    /// 结束标记
    /// </summary>
    internal readonly short EndMark = short.MaxValue;

    /// <summary>
    /// 将 <see cref="PinyinDictionary"/> 序列化为 <see cref="BinaryWriter"/> 对象
    /// </summary>
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

    /// <summary>
    /// 从 <see cref="BinaryReader"/> 对象中反序列化 <see cref="PinyinDictionary"/> 对象
    /// </summary>
    /// <param name="binaryReader"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 根据指定的拼音查找对应的拼音单元在拼音单元表中的索引
    /// </summary>
    internal int GetPinYinUnitIndex(string pinyin) => PinyinUnitTable.FindIndex(new PinyinUnitPredicate(pinyin).Match);

    /// <summary>
    /// 根据指定的拼音获取对应的拼音单元
    /// </summary>
    internal PinyinUnit GetPinYinUnit(string pinyin) => PinyinUnitTable.Find(new PinyinUnitPredicate(pinyin).Match)!;

    /// <summary>
    /// 根据索引获取拼音单元
    /// </summary>    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal PinyinUnit GetPinYinUnitByIndex(int index) => index >= 0 && index < Count ? PinyinUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);
}
