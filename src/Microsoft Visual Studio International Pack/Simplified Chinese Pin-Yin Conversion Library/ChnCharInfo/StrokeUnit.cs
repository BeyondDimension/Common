// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.StrokeUnit
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

/// <summary>
/// 提供成员变量和方法来处理字符笔画单元
/// </summary>
sealed class StrokeUnit
{
    /// <summary>
    /// 笔画的数量
    /// </summary>
    internal byte StrokeNumber;

    /// <summary>
    /// 字符的数量
    /// </summary>
    internal short CharCount;

    /// <summary>
    /// 存储字符数组的集合
    /// </summary>
    internal char[] CharList = null!;

    /// <summary>
    /// 从 <see cref="BinaryReader"/> 对象中反序列化 <see cref="StrokeUnit"/> 对象
    /// </summary>
    /// <param name="binaryReader"></param>
    /// <returns></returns>
    internal static StrokeUnit Deserialize(BinaryReader binaryReader)
    {
        StrokeUnit strokeUnit = new()
        {
            StrokeNumber = binaryReader.ReadByte(),
            CharCount = binaryReader.ReadInt16(),
        };
        strokeUnit.CharList = new char[strokeUnit.CharCount];
        for (int index = 0; index < strokeUnit.CharCount; ++index)
            strokeUnit.CharList[index] = binaryReader.ReadChar();
        return strokeUnit;
    }

    /// <summary>
    /// 将 <see cref="StrokeUnit"/> 对象序列化到 <see cref="BinaryWriter"/> 对象中
    /// </summary>
    /// <param name="binaryWriter"></param>
    internal void Serialize(BinaryWriter binaryWriter)
    {
        if (CharCount == 0)
            return;
        binaryWriter.Write(StrokeNumber);
        binaryWriter.Write(CharCount);
        binaryWriter.Write(CharList);
    }
}
