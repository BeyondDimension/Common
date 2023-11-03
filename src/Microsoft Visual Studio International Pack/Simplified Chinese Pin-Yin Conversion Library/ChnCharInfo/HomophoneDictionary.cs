// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.HomophoneDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 同音词字典
/// </summary>
sealed class HomophoneDictionary
{
    /// <summary>
    /// 字典长度
    /// </summary>
    internal int Length;

    /// <summary>
    /// 偏移量
    /// </summary>
    internal short Offset;

    /// <summary>
    /// 同音词单元数量
    /// </summary>
    internal short Count;

    /// <summary>
    /// 保留字段
    /// </summary>
    internal readonly byte[] Reserved = new byte[8];

    /// <summary>
    /// 同音词单元表
    /// </summary>
    internal List<HomophoneUnit> HomophoneUnitTable = null!;

    /// <summary>
    /// 结束标记
    /// </summary>
    internal readonly short EndMark = short.MaxValue;

    /// <summary>
    /// 将 <see cref="HomophoneDictionary"/> 序列化为 <see cref="BinaryWriter"/> 对象
    /// </summary>
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

    /// <summary>
    /// 从 <see cref="BinaryReader"/> 对象中反序列化 <see cref="HomophoneDictionary"/> 对象
    /// </summary>
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

    /// <summary>
    /// 通过索引获取指定位置的同音词单元
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal HomophoneUnit GetHomophoneUnit(int index) => index >= 0 && index < Count ? HomophoneUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);

    /// <summary>
    /// 通过拼音获取对应的同音词单元
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal HomophoneUnit GetHomophoneUnit(
      PinyinDictionary pinyinDictionary,
      string pinyin)
    {
        return GetHomophoneUnit(pinyinDictionary.GetPinYinUnitIndex(pinyin));
    }
}
