// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.PinyinUnit
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class PinyinUnit
{
    internal string Pinyin = null!;

    internal static PinyinUnit Deserialize(BinaryReader binaryReader)
    {
        PinyinUnit pinyinUnit = new PinyinUnit();
        byte[] bytes = binaryReader.ReadBytes(7);
        pinyinUnit.Pinyin = Encoding.ASCII.GetString(bytes, 0, 7);
        char[] chArray = new char[1];
        pinyinUnit.Pinyin = pinyinUnit.Pinyin.TrimEnd(chArray);
        return pinyinUnit;
    }

    internal void Serialize(BinaryWriter binaryWriter)
    {
        byte[] numArray = new byte[7];
        Encoding.ASCII.GetBytes(Pinyin, 0, Pinyin.Length, numArray, 0);
        binaryWriter.Write(numArray);
    }
}
