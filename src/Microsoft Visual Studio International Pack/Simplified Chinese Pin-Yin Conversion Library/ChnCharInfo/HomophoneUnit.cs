// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.HomophoneUnit
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

#pragma warning disable SA1600 // Elements should be documented

sealed class HomophoneUnit
{
    internal short Count;
    internal char[] HomophoneList = null!;

    internal static HomophoneUnit Deserialize(BinaryReader binaryReader)
    {
        HomophoneUnit homophoneUnit = new HomophoneUnit()
        {
            Count = binaryReader.ReadInt16()
        };
        homophoneUnit.HomophoneList = new char[homophoneUnit.Count];
        for (int index = 0; index < homophoneUnit.Count; ++index)
            homophoneUnit.HomophoneList[index] = binaryReader.ReadChar();
        return homophoneUnit;
    }

    internal void Serialize(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(Count);
        for (int index = 0; index < Count; ++index)
            binaryWriter.Write(HomophoneList[index]);
    }
}
