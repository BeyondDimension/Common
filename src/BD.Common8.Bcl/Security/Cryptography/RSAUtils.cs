namespace System.Security.Cryptography;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 非对称加密算法 - RSA
/// </summary>
public static partial class RSAUtils
{
    #region FromJsonString

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSAParameters GetRSAParametersFromJsonString(string jsonString)
    {
        var rsaParams = Serializable.DJSON<Parameters>(jsonString);
        if (rsaParams == null) throw new NullReferenceException(nameof(rsaParams));
        return rsaParams.ToStruct();
    }

    /// <summary>
    /// 通过 JSON 字符串中的密钥信息初始化 RSA 对象。
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="jsonString"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromJsonString(RSA rsa, string jsonString)
    {
        var rsaParams = GetRSAParametersFromJsonString(jsonString);
        rsa.ImportParameters(rsaParams);
    }

    #endregion

    #region 创建 RSA 实例

    ///// <summary>
    ///// 创建 <see cref="RSA" /> 对象。
    ///// </summary>
    ///// <param name="xmlString"></param>
    ///// <returns></returns>
    //public static RSA CreateFromXmlString(string xmlString)
    //{
    //    var rsaParams = GetRSAParametersFromXmlString(xmlString);
    //    var rsa = RSA.Create(rsaParams);
    //    return rsa;
    //}

    /// <summary>
    /// 创建 <see cref="RSA" /> 对象。
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSA CreateFromJsonString(string jsonString)
    {
        var rsaParams = GetRSAParametersFromJsonString(jsonString);
        var rsa = RSA.Create(rsaParams);
        return rsa;
    }

    #endregion

    #region 从 RSA 实例中获取密钥

    /// <summary>
    /// 创建并返回包含当前 <see cref="RSA"/> 对象的密钥的 JSON 字符串。
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="includePrivateParameters"><see langword="true" /> 表示同时包含 RSA 公钥和私钥；<see langword="false" /> 表示仅包含公钥。</param>
    /// <returns>包含当前 <see cref="RSA"/> 对象的密钥的 JSON 字符串。</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToJsonString(this RSA rsa, bool includePrivateParameters)
    {
        var rsaParams = rsa.ExportParameters(includePrivateParameters).ToObject();
        var jsonString = Serializable.SJSON(rsaParams, ignoreNullValues: true);
        return jsonString;
    }

    #endregion

    #region Padding

    /// <summary>
    /// RSA 填充，不可更改此值！
    /// </summary>
    [Obsolete("use DefaultPadding")]
    internal static RSAEncryptionPadding Padding => RSAEncryptionPadding.OaepSHA256;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSAEncryptionPadding GetPaddingByOaepHashAlgorithmName(string? oaepHashAlgorithmName)
    {
        if (!string.IsNullOrWhiteSpace(oaepHashAlgorithmName))
        {
            switch (oaepHashAlgorithmName)
            {
                case "0":
                    return RSAEncryptionPadding.Pkcs1;
                case "1":
                    return RSAEncryptionPadding.OaepSHA1;
                case "2" or "":
                    return RSAEncryptionPadding.OaepSHA256;
                case "3":
                    return RSAEncryptionPadding.OaepSHA384;
                case "5":
                    return RSAEncryptionPadding.OaepSHA512;
                default:
                    if (oaepHashAlgorithmName.Equals(nameof(HashAlgorithmName.SHA1), StringComparison.OrdinalIgnoreCase))
                        return RSAEncryptionPadding.OaepSHA1;
                    if (oaepHashAlgorithmName.Equals(nameof(HashAlgorithmName.SHA256), StringComparison.OrdinalIgnoreCase))
                        return RSAEncryptionPadding.OaepSHA256;
                    if (oaepHashAlgorithmName.Equals(nameof(HashAlgorithmName.SHA384), StringComparison.OrdinalIgnoreCase))
                        return RSAEncryptionPadding.OaepSHA384;
                    if (oaepHashAlgorithmName.Equals(nameof(HashAlgorithmName.SHA512), StringComparison.OrdinalIgnoreCase))
                        return RSAEncryptionPadding.OaepSHA512;
                    break;
            }
        }
        return RSAEncryptionPadding.OaepSHA256;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(RSAEncryptionPadding rSAEncryptionPadding) => rSAEncryptionPadding.Mode switch
    {
        RSAEncryptionPaddingMode.Pkcs1 => "0",
        RSAEncryptionPaddingMode.Oaep => rSAEncryptionPadding.OaepHashAlgorithm.Name switch
        {
            nameof(HashAlgorithmName.SHA1) => "1",
            nameof(HashAlgorithmName.SHA256) => "",
            nameof(HashAlgorithmName.SHA384) => "3",
            nameof(HashAlgorithmName.SHA512) => "5",
            _ => throw new ArgumentOutOfRangeException(nameof(rSAEncryptionPadding)),
        },
        _ => throw new ArgumentOutOfRangeException(nameof(rSAEncryptionPadding)),
    };

    public static RSAEncryptionPadding DefaultPadding
        => OperatingSystem.IsAndroid() ?
            RSAEncryptionPadding.OaepSHA1 :
            RSAEncryptionPadding.OaepSHA256;

    #endregion

    #region 加密(Encrypt)

    /// <summary>
    /// RSA加密(ByteArray → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [Obsolete("use byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)", true)]
    public static byte[] Encrypt(this RSA rsa, byte[] data) => rsa.Encrypt(data, Padding);

    /// <summary>
    /// RSA加密(ByteArray → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? Encrypt_Nullable(this RSA rsa, byte[]? data, RSAEncryptionPadding? padding = null)
    {
        if (data == default) return default;
        padding ??= DefaultPadding;
        return rsa.Encrypt(data, padding);
    }

    /// <summary>
    /// RSA加密(String → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] EncryptToByteArray(this RSA rsa, string text, RSAEncryptionPadding? padding = null)
    {
        var data = Encoding.UTF8.GetBytes(text);
        padding ??= DefaultPadding;
        return rsa.Encrypt(data, padding);
    }

    /// <summary>
    /// RSA加密(String → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? EncryptToByteArray_Nullable(this RSA rsa, string? text, RSAEncryptionPadding? padding = null)
    {
        if (text == default) return default;
        return EncryptToByteArray(rsa, text, padding);
    }

    /// <summary>
    /// RSA加密(String → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Encrypt(this RSA rsa, string text, RSAEncryptionPadding? padding = null)
    {
        var bytes = EncryptToByteArray(rsa, text, padding);
        return bytes.Base64UrlEncode();
    }

    /// <summary>
    /// RSA加密(String → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Encrypt_Nullable(this RSA rsa, string? text, RSAEncryptionPadding? padding = null)
    {
        if (text == default) return default;
        return Encrypt(rsa, text, padding);
    }

    /// <summary>
    /// RSA加密(ByteArray → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncryptToString(this RSA rsa, byte[] data, RSAEncryptionPadding? padding = null)
    {
        padding ??= DefaultPadding;
        var bytes = rsa.Encrypt(data, padding);
        return bytes.Base64UrlEncode();
    }

    /// <summary>
    /// RSA加密(ByteArray → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncryptToHexString(this RSA rsa, byte[] data, RSAEncryptionPadding? padding = null)
    {
        padding ??= DefaultPadding;
        var bytes = rsa.Encrypt(data, padding);
        return bytes.ToHexString();
    }

    /// <summary>
    /// RSA加密(ByteArray → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? EncryptToString_Nullable(this RSA rsa, byte[]? data, RSAEncryptionPadding? padding = null)
    {
        if (data == default) return default;
        return EncryptToString(rsa, data, padding);
    }

    #endregion

    #region 解密(Decrypt)

    /// <summary>
    /// RSA解密(ByteArray → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [Obsolete("use byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)", true)]
    public static byte[] Decrypt(this RSA rsa, byte[] data) => rsa.Decrypt(data, Padding);

    /// <summary>
    /// RSA解密(ByteArray → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? Decrypt_Nullable(this RSA rsa, byte[]? data, RSAEncryptionPadding? padding = null)
    {
        if (data == default) return default;
        padding ??= DefaultPadding;
        return rsa.Decrypt(data, padding);
    }

    /// <summary>
    /// RSA解密(String → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] DecryptToByteArray(this RSA rsa, string text, RSAEncryptionPadding? padding = null)
    {
        var bytes = text.Base64UrlDecodeToByteArray();
        padding ??= DefaultPadding;
        return rsa.Decrypt(bytes, padding);
    }

    /// <summary>
    /// RSA解密(String → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] DecryptToByteArrayHex(this RSA rsa, string text, RSAEncryptionPadding? padding = null)
    {
        var bytes = Convert2.FromHexString(text);
        padding ??= DefaultPadding;
        return rsa.Decrypt(bytes, padding);
    }

    /// <summary>
    /// RSA解密(String → ByteArray)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? DecryptToByteArray_Nullable(this RSA rsa, string? text, RSAEncryptionPadding? padding = null)
    {
        if (text == default) return default;
        return DecryptToByteArray(rsa, text, padding);
    }

    /// <summary>
    /// RSA解密(ByteArray → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecryptToString(this RSA rsa, byte[] data, RSAEncryptionPadding? padding = null)
    {
        padding ??= DefaultPadding;
        var bytes = rsa.Decrypt(data, padding);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// RSA解密(ByteArray → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? DecryptToString_Nullable(this RSA rsa, byte[]? data, RSAEncryptionPadding? padding = null)
    {
        if (data == default) return default;
        return DecryptToString(rsa, data, padding);
    }

    /// <summary>
    /// RSA解密(String → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Decrypt(this RSA rsa, string text, RSAEncryptionPadding? padding = null)
    {
        var result = DecryptToByteArray(rsa, text, padding);
        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// RSA解密(String → String)
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="text"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Decrypt_Nullable(this RSA rsa, string? text, RSAEncryptionPadding? padding = null)
    {
        if (text == default) return default;
        return Decrypt(rsa, text, padding);
    }

    #endregion

    /// <summary>
    /// <see cref="RSA"/> 密钥 模型类
    /// <para>可使用 JSON 序列化此模型，不支持 MessagePack</para>
    /// </summary>
    internal sealed class Parameters
    {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonConstructor]
#endif
        [NewtonsoftJsonConstructor]
        public Parameters() { }

        /// <summary>
        /// Represents the D parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("z")]
#endif
        [NewtonsoftJsonProperty("z")]
        public string? D { get; set; }

        /// <summary>
        /// Represents the DP parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("x")]
#endif
        [NewtonsoftJsonProperty("x")]
        public string? DP { get; set; }

        /// <summary>
        /// Represents the DQ parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("c")]
#endif
        [NewtonsoftJsonProperty("c")]
        public string? DQ { get; set; }

        /// <summary>
        /// Represents the Exponent parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("v")]
#endif
        [NewtonsoftJsonProperty("v")]
        public string? Exponent { get; set; }

        /// <summary>
        /// Represents the InverseQ parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("b")]
#endif
        [NewtonsoftJsonProperty("b")]
        public string? InverseQ { get; set; }

        /// <summary>
        /// Represents the Modulus parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("n")]
#endif
        [NewtonsoftJsonProperty("n")]
        public string? Modulus { get; set; }

        /// <summary>
        /// Represents the P parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("m")]
#endif
        [NewtonsoftJsonProperty("m")]
        public string? P { get; set; }

        /// <summary>
        /// Represents the Q parameter for the <see cref="T:System.Security.Cryptography.RSA"></see> algorithm.
        /// </summary>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [SystemTextJsonProperty("a")]
#endif
        [NewtonsoftJsonProperty("a")]
        public string? Q { get; set; }

        public bool Equals(Parameters? other)
        {
            if (other == null) return false;
            return string.Equals(D, other.D, StringComparison.Ordinal) &&
                string.Equals(DP, other.DP, StringComparison.Ordinal) &&
                string.Equals(DQ, other.DQ, StringComparison.Ordinal) &&
                string.Equals(Exponent, other.Exponent, StringComparison.Ordinal) &&
                string.Equals(InverseQ, other.InverseQ, StringComparison.Ordinal) &&
                string.Equals(Modulus, other.Modulus, StringComparison.Ordinal) &&
                string.Equals(P, other.P, StringComparison.Ordinal) &&
                string.Equals(Q, other.Q, StringComparison.Ordinal);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static RSAParameters ToStruct(this Parameters parms) => new()
    {
        D = parms.D.Base64UrlDecodeToByteArray_Nullable(),
        DP = parms.DP.Base64UrlDecodeToByteArray_Nullable(),
        DQ = parms.DQ.Base64UrlDecodeToByteArray_Nullable(),
        Exponent = parms.Exponent.Base64UrlDecodeToByteArray_Nullable(),
        InverseQ = parms.InverseQ.Base64UrlDecodeToByteArray_Nullable(),
        Modulus = parms.Modulus.Base64UrlDecodeToByteArray_Nullable(),
        P = parms.P.Base64UrlDecodeToByteArray_Nullable(),
        Q = parms.Q.Base64UrlDecodeToByteArray_Nullable(),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Parameters ToObject(this RSAParameters parms) => new()
    {
        D = parms.D.Base64UrlEncode_Nullable(),
        DP = parms.DP.Base64UrlEncode_Nullable(),
        DQ = parms.DQ.Base64UrlEncode_Nullable(),
        Exponent = parms.Exponent.Base64UrlEncode_Nullable(),
        InverseQ = parms.InverseQ.Base64UrlEncode_Nullable(),
        Modulus = parms.Modulus.Base64UrlEncode_Nullable(),
        P = parms.P.Base64UrlEncode_Nullable(),
        Q = parms.Q.Base64UrlEncode_Nullable(),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSA Create(this RSAParameters parms) => RSA.Create(parms);
}