// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.CharDictionary
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 字符字典
/// </summary>
sealed class CharDictionary
{
    /// <summary>
    /// 字典长度
    /// </summary>
    internal int Length;

    /// <summary>
    /// 字符单元数量
    /// </summary>
    internal int Count;

    /// <summary>
    /// 偏移量
    /// </summary>
    internal short Offset;

    /// <summary>
    /// 保留的字节数组
    /// </summary>
    internal readonly byte[] Reserved = new byte[24];

    /// <summary>
    /// 字符单元表
    /// </summary>
    internal List<CharUnit> CharUnitTable = null!;

    /// <summary>
    /// 结束标记
    /// </summary>
    internal readonly short EndMark = short.MaxValue;

    /// <summary>
    /// 将 <see cref="CharDictionary"/> 序列化为 <see cref="BinaryWriter"/> 对象
    /// </summary>
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

    /// <summary>
    /// 从 <see cref="BinaryReader"/> 对象中反序列化 <see cref="CharDictionary"/> 对象
    /// </summary>
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

    /// <summary>
    /// 根据索引获取字符单元
    /// </summary>
    internal CharUnit GetCharUnit(int index) => index >= 0 && index < Count ? CharUnitTable[index] : throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);

    /// <summary>
    /// 根据字符获取字符单元
    /// </summary>
    internal CharUnit GetCharUnit(char ch) => CharUnitTable.Find(new CharUnitPredicate(ch).Match)!;
}
