using System.Runtime.Serialization.Formatters;

namespace BD.Common.UnitTest;

public sealed class RSATest
{
    const int keySizeInBits = 4096;
    const string text = "a你好，Здравствыйте，こんにちは 안녕하세요.😋✅☺️📄💜😂❌😁🔫🛀";

    [SetUp]
    public void Setup()
    {

    }

    static void TestRSA(RSA rsa, string ciphertext)
    {
        var plaintext = rsa.Decrypt(ciphertext);
        Assert.That(plaintext, Is.EqualTo(text));
    }

    [Test]
    public void RSA_Json()
    {
        string privateKey = null!;
        string ciphertext = null!;
        {
            using var rsa = RSA.Create(keySizeInBits);
            privateKey = rsa.ToJsonString(true);
            ciphertext = rsa.Encrypt(text);
        }
        TestContext.WriteLine($"ByteCount: {Encoding.UTF8.GetByteCount(privateKey)}");
        {
            using var rsa = RSAUtils.GetRSAParametersFromJsonString(privateKey).Create();
            TestRSA(rsa, ciphertext);
        }
    }

    [Test]
    public void RSA_MP2()
    {
        MemoryPackFormatterProvider.Register(RSAParametersFormatterAttribute.Formatter.Default);
        byte[] privateKey = null!;
        string ciphertext = null!;
        {
            using var rsa = RSA.Create(keySizeInBits);
            privateKey = Serializable.SMP2(rsa.ExportParameters(true));
            ciphertext = rsa.Encrypt(text);
        }
        TestContext.WriteLine($"ByteCount: {privateKey.Length}");
        {
            using var rsa = Serializable.DMP2<RSAParameters>(privateKey).Create();
            TestRSA(rsa, ciphertext);
        }
    }
}
