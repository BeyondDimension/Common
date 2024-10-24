namespace System.Security.Cryptography;

/// <summary>
/// 非对称加密算法 - RSA
/// </summary>
public static partial class RSAUtils
{
    #region FromJsonString

    /// <summary>
    /// 从 JSON 字符串获取 RSA 参数
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSAParameters GetRSAParametersFromJsonString(string jsonString)
    {
#if !NO_SYSTEM_TEXT_JSON
        var rsaParams = SystemTextJsonSerializer.Deserialize(jsonString, ParametersJsonSerializerContext.Default.Parameters);
#else
        var rsaParams = Serializable.DJSON<Parameters>(jsonString);
#endif
        if (rsaParams == null) throw new NullReferenceException(nameof(rsaParams));
        return rsaParams.ToStruct();
    }

#if !NO_SYSTEM_TEXT_JSON

    /// <summary>
    /// 从 JSON UTF8 流获取 RSA 参数
    /// </summary>
    /// <param name="utf8Json"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSAParameters GetRSAParametersFromUtf8Json(Stream utf8Json)
    {
        var rsaParams = SystemTextJsonSerializer.Deserialize(utf8Json, ParametersJsonSerializerContext.Default.Parameters);
        if (rsaParams == null) throw new NullReferenceException(nameof(rsaParams));
        return rsaParams.ToStruct();
    }

    /// <summary>
    /// 通过 JSON UTF8 流中的密钥信息初始化 RSA 对象
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="utf8Json"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromJsonString(RSA rsa, Stream utf8Json)
    {
        var rsaParams = GetRSAParametersFromUtf8Json(utf8Json);
        rsa.ImportParameters(rsaParams);
    }

#endif

    /// <summary>
    /// 通过 JSON 字符串中的密钥信息初始化 RSA 对象
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="jsonString"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromJsonString(RSA rsa, string jsonString)
    {
        var rsaParams = GetRSAParametersFromJsonString(jsonString);
        rsa.ImportParameters(rsaParams);
    }

    #endregion FromJsonString

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
    /// 创建 <see cref="RSA" /> 对象
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

    /// <summary>
    /// 创建 <see cref="RSA" /> 对象
    /// </summary>
    /// <param name="utf8Json"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSA CreateFromJsonString(Stream utf8Json)
    {
        var rsaParams = GetRSAParametersFromUtf8Json(utf8Json);
        var rsa = RSA.Create(rsaParams);
        return rsa;
    }

    #endregion 创建 RSA 实例

    #region 从 RSA 实例中获取密钥

    /// <summary>
    /// 创建并返回包含当前 <see cref="RSA"/> 对象的密钥的 JSON 字符串
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="includePrivateParameters"><see langword="true" /> 表示同时包含 RSA 公钥和私钥；<see langword="false" /> 表示仅包含公钥</param>
    /// <returns>包含当前 <see cref="RSA"/> 对象的密钥的 JSON 字符串</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToJsonString(this RSA rsa, bool includePrivateParameters)
    {
        var rsaParams = rsa.ExportParameters(includePrivateParameters).ToObject();
#if !NO_SYSTEM_TEXT_JSON
        var jsonString = SystemTextJsonSerializer.Serialize(rsaParams, ParametersJsonSerializerContext.Default.Parameters);
#else
        var jsonString = Serializable.SJSON(rsaParams, ignoreNullValues: true);
#endif
        return jsonString;
    }

    #endregion 从 RSA 实例中获取密钥

    #region Padding

    /// <summary>
    /// RSA 填充，不可更改此值！
    /// </summary>
    [Obsolete("use DefaultPadding")]
    internal static RSAEncryptionPadding Padding => RSAEncryptionPadding.OaepSHA256;

    /// <summary>
    /// 根据 OAEP 哈希算法获取 RSA 加密填充方式
    /// </summary>
    /// <param name="oaepHashAlgorithmName">OAEP 哈希算法的名称</param>
    /// <returns>对应的 RSA 加密填充方式</returns>
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
                    if (oaepHashAlgorithmName!.Equals(nameof(HashAlgorithmName.SHA1), StringComparison.OrdinalIgnoreCase))
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

    /// <summary>
    /// 将指定要与 RSA 加密或解密操作一起使用的填充模式转换成字符串表现形式
    /// </summary>
    /// <param name="rSAEncryptionPadding"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    /// <summary>
    /// 获取在特定操作系统上使用的默认加密填充模式，如果操作系统是 Android，则返回 <see cref="RSAEncryptionPadding.OaepSHA1"/>；否则返回 <see cref="RSAEncryptionPadding.OaepSHA256"/>
    /// </summary>
    public static RSAEncryptionPadding DefaultPadding
#if NET5_0_OR_GREATER
        => OperatingSystem.IsAndroid() ?
            RSAEncryptionPadding.OaepSHA1 :
#else
        =>
#endif
            RSAEncryptionPadding.OaepSHA256;

    #endregion Padding

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

    #endregion 加密(Encrypt)

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

    #endregion 解密(Decrypt)

    /// <summary>
    /// <see cref="RSA"/> 密钥 模型类
    /// <para>可使用 JSON 序列化此模型，不支持 MessagePack</para>
    /// </summary>
    internal sealed class Parameters
    {
#if !NO_SYSTEM_TEXT_JSON

        /// <summary>
        /// 初始化 <see cref="Parameters"/> 类的新实例
        /// </summary>
        [SystemTextJsonConstructor]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonConstructor]
#endif
        public Parameters() { }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的D参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("z")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("z")]
#endif
        public string? D { get; set; }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的 DP 参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("x")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("x")]
#endif
        public string? DP { get; set; }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的 DQ 参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("c")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("c")]
#endif
        public string? DQ { get; set; }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的 Exponent 参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("v")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("v")]
#endif
        public string? Exponent { get; set; }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的 InverseQ 参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("b")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("b")]
#endif
        public string? InverseQ { get; set; }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的 Modulus 参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("n")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("n")]
#endif
        public string? Modulus { get; set; }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的 P 参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("m")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("m")]
#endif
        public string? P { get; set; }

        /// <summary>
        /// 表示 <see cref="T:System.Security.Cryptography.RSA"/> 算法的 Q 参数
        /// </summary>
#if !NO_SYSTEM_TEXT_JSON

        [SystemTextJsonProperty("a")]
#endif
#if !NO_NEWTONSOFT_JSON
        [NewtonsoftJsonProperty("a")]
#endif
        public string? Q { get; set; }

        /// <summary>
        /// 判断两个 <see cref="Parameters"/> 对象是否相等
        /// </summary>
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

    /// <summary>
    /// 用于序列化 <see cref="RSAUtils.Parameters"/> 类的 JSON 上下文类
    /// </summary>
#if !NO_SYSTEM_TEXT_JSON

    [JsonSourceGenerationOptions(
        DefaultIgnoreCondition = SystemTextJsonIgnoreCondition.WhenWritingDefault,
        AllowTrailingCommas = true
        )]
    [JsonSerializable(typeof(Parameters))]
    internal sealed partial class ParametersJsonSerializerContext : SystemTextJsonSerializerContext
    {
    }

#endif

    /// <summary>
    /// 将 <see cref="Parameters"/> 对象转换为 <see cref="RSAParameters"/> 结构体
    /// </summary>
    /// <param name="parms">要转换的 <see cref="Parameters"/> 对象</param>
    /// <returns>转换后得到的 <see cref="RSAParameters"/> 结构体</returns>
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

    /// <summary>
    /// 将 <see cref="RSAParameters"/> 结构体转换为 json 字符串
    /// </summary>
    /// <param name="rSAParameters"></param>
    /// <returns></returns>
    public static string ToJsonString(this RSAParameters rSAParameters)
    {
        var rsaParams = rSAParameters.ToObject();
        var result = SystemTextJsonSerializer.Serialize(rsaParams, ParametersJsonSerializerContext.Default.Parameters);
        return result;
    }

    /// <summary>
    /// 将 <see cref="RSAParameters"/> 结构体转换为 <see cref="Parameters"/> 对象
    /// </summary>
    /// <param name="parms">要转换的 <see cref="RSAParameters"/> 结构体</param>
    /// <returns>转换后得到的 <see cref="Parameters"/> 对象</returns>
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

    /// <summary>
    /// 使用指定参数创建 RSA 实例
    /// </summary>
    /// <param name="parms">要使用的密钥参数</param>
    /// <returns>创建的 RSA 实例</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RSA Create(this RSAParameters parms) => RSA.Create(parms);
}