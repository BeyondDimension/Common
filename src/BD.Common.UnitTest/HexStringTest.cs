namespace BD.Common.UnitTest;

public sealed class HexStringTest
{
    static string ByteArrayToString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");

    static byte[] StringToByteArray(string hex)
    {
        var numberChars = hex.Length;
        var bytes = new byte[numberChars / 2];
        for (var i = 0; i < numberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    [Test]
    public void ToHexString()
    {
        var bytes = Hashs.ByteArray.SHA384("TEST"u8.ToArray());
        var hexStr1 = bytes.ToHexString();
        var hexStr2 = ByteArrayToString(bytes);
        Assert.IsTrue(hexStr1 == hexStr2);
    }

    [Test]
    public void FromHexString()
    {
        var bytes = Hashs.ByteArray.SHA384("TEST"u8.ToArray());
        var hexStr1 = bytes.ToHexString();

        var bytes1 = Convert2.FromHexString(hexStr1);
        var bytes2 = StringToByteArray(hexStr1);
        Assert.IsTrue(bytes1.SequenceEqual(bytes2));
    }
}
