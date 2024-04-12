namespace System.Security.Cryptography;

public static partial class ServerSecurity
{
    /// <summary>
    /// 16 进制字符串转 Bytes
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    static byte[] HexStringToBytes(string hex)
    {
        if (hex.Length == 0)
            return new byte[1];
        if (hex.Length % 2 == 1)
            hex = "0" + hex;
        byte[] numArray = new byte[hex.Length / 2];
        for (int index = 0; index < hex.Length / 2; ++index)
            numArray[index] = byte.Parse(hex.Substring(2 * index, 2), NumberStyles.AllowHexSpecifier);
        return numArray;
    }

    //static readonly Lazy<RSA> mRSA = new(() =>
    //{
    //    MemoryPackFormatterProvider.Register(RSAParametersFormatterAttribute.Formatter.Default);
    //    var rsa = Serializable.DMP2<RSAParameters>(ResSecrets.RSAPrivateKey).Create();
    //    return rsa;
    //});

    static RSA? _RSA;

    public static RSA RSA { get => _RSA.ThrowIsNull(); set => _RSA = value; }

    /// <summary>
    /// 用于 JS 前端数据的 RSA 解密
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Decrypt(string text)
    {
        var strArray = text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < strArray.Length; i++)
        {
            var data = HexStringToBytes(strArray[i]);
            data = RSA.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            strArray[i] = Encoding.UTF8.GetString(data);
        }

        var output = WebUtility.UrlDecode(string.Concat(strArray));
        return output;
    }

    /// <summary>
    /// 用于 JS 前端数据的 RSA 解密
    /// </summary>
    /// <param name="text">前端分割的RSA加密</param>
    /// <returns></returns>
    public static string DecryptBase64(string text)
    {
        var strArray = text.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var output = WebUtility.UrlDecode(DecryptBase64(strArray));
        return output;
    }

    /// <summary>
    /// 用于 JS 前端数据的 RSA 解密
    /// </summary>
    /// <param name="text">前端分割的RSA加密</param>
    /// <returns></returns>
    public static string DecryptBase64(string[] text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            var data = RSA.Decrypt(Convert.FromBase64String(text[i]), RSAEncryptionPadding.Pkcs1);
            text[i] = Encoding.UTF8.GetString(data);
        }

        var output = WebUtility.UrlDecode(string.Concat(text));
        return output;
    }
}
