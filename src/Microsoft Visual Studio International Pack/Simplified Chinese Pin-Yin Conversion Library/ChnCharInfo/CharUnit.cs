// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.CharUnit
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 字符单元
/// </summary>
sealed class CharUnit
{
    /// <summary>
    /// 字符
    /// </summary>
    internal char Char;

    /// <summary>
    /// 笔画数
    /// </summary>
    internal byte StrokeNumber;

    /// <summary>
    /// 拼音数量
    /// </summary>
    internal byte PinyinCount;

    /// <summary>
    /// 拼音索引列表
    /// </summary>
    internal short[] PinyinIndexList = null!;

    /// <summary>
    /// 从 <see cref="BinaryReader"/> 对象中反序列化 <see cref="CharUnit"/> 对象
    /// </summary>
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

    /// <summary>
    /// 将 <see cref="CharUnit"/> 序列化为 <see cref="BinaryWriter"/> 对象
    /// </summary>
    internal void Serialize(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(Char);
        binaryWriter.Write(StrokeNumber);
        binaryWriter.Write(PinyinCount);
        for (int index = 0; index < PinyinCount; ++index)
            binaryWriter.Write(PinyinIndexList[index]);
    }
}
