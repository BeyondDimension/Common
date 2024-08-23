using Force.Crc32;
using System.IO.Hashing;

namespace BD.Common8.UnitTest;

public sealed class Crc32Test
{
    const bool isLower = true;

    [Test]
    public void String()
    {
        var text = Environment.StackTrace + Environment.OSVersion.VersionString;

        var str_l = Hashs.ComputeHashString(text, new Crc32Algorithm(), isLower);

        var bytes_r = Crc32.Hash(Encoding.UTF8.GetBytes(text));
        bytes_r.AsSpan().Reverse();
        var str_r = bytes_r.ToHexString(isLower);

        Assert.That(str_l, Is.EqualTo(str_r));
    }

    [Test]
    public void Bytes()
    {
        var buffer =
"""
var str_l = Hashs.ComputeHashString(text, CreateCrc32(), isLower);
"""u8.ToArray();

        var bytes_l = Hashs.ComputeHash(buffer, new Crc32Algorithm());
        var bytes_r = Crc32.Hash(buffer);
        bytes_r.AsSpan().Reverse();

        Assert.That(bytes_l.SequenceEqual(bytes_r));
    }
}
