namespace System.Security.Cryptography;

/// <summary>
/// 对称加密算法 - AES
/// </summary>
public static class AESUtils
{
    #region 创建 Aes 实例

    /// <summary>
    /// AES 密钥的位数
    /// </summary>
    public const int AesKeySize = 256;

    /// <summary>
    /// 创建 <see cref="Aes"/> 实例
    /// </summary>
    /// <param name="keySize">密钥大小，默认值为 <see cref="AesKeySize"/></param>
    /// <param name="padding">填充</param>
    /// <param name="mode">模式</param>
    /// <returns></returns>
    public static Aes Create(int keySize = AesKeySize, PaddingMode padding = PaddingMode.PKCS7, CipherMode mode = CipherMode.CBC)
    {
        var aes = Aes.Create();
        aes.KeySize = keySize;
        aes.Padding = padding;
        aes.Mode = mode;
        return aes;
    }

    /// <summary>
    /// 通过 ByteArray 创建 <see cref="Aes"/> 实例
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Aes Create(byte[] data) => Parameters.Create(data).Create();

    /// <summary>
    /// 通过 ByteArray 创建 <see cref="Aes"/> 实例
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Aes? Create_Nullable(byte[]? data)
    {
        if (data == default) return default;
        return Create(data);
    }

    /// <summary>
    /// 通过 String 创建 <see cref="Aes"/> 实例
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Aes Create(string data) => Parameters.Create(data).Create();

    /// <summary>
    /// 通过 <see cref="Parameters"/>(String) 创建 <see cref="Aes"/> 实例
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Aes? Create_Nullable(string? data)
    {
        if (data == default) return default;
        return Create(data);
    }

    /// <summary>
    /// 通过 args 创建 <see cref="Aes"/> 实例
    /// </summary>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <param name="mode"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Aes Create(
        byte[] key,
        byte[] iv,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7) => new Parameters(key, iv, mode, padding).Create();

    #endregion

    #region 从 Aes 实例中获取密钥

    /// <summary>
    /// 将 <see cref="Aes"/> 实例中的 [密钥,向量,模式,填充] 转换为可传输的模型类对象
    /// </summary>
    /// <param name="aes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Parameters ToParams(this Aes aes) => new(aes.Key, aes.IV, aes.Mode, aes.Padding);

    /// <summary>
    /// 将 <see cref="Aes"/> 实例中的密钥转换为可传输的字符串
    /// </summary>
    /// <param name="aes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToParamsString(this Aes aes) => aes.ToParams().ToString();

    /// <summary>
    /// 将 <see cref="Aes"/> 实例中的密钥转换为可传输的 <see cref="byte"/>[]
    /// </summary>
    /// <param name="aes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToParamsByteArray(this Aes aes) => aes.ToParams().ToByteArray();

    #endregion

    #region 加密(Encrypt)

    /// <summary>
    /// AES 加密(ByteArray → ByteArray)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Encrypt(this Aes aes, byte[] data)
    {
        using var transform = aes.CreateEncryptor();
        return transform.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// AES 加密(ByteArray → ByteArray)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? Encrypt_Nullable(this Aes aes, byte[]? data)
    {
        if (data == default) return default;
        return Encrypt(aes, data);
    }

    /// <summary>
    /// AES 加密(String → ByteArray)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] EncryptToByteArray(this Aes aes, string text)
    {
        var data = Encoding.UTF8.GetBytes(text);
        return Encrypt(aes, data);
    }

    /// <summary>
    /// AES 加密(String → ByteArray)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? EncryptToByteArray_Nullable(this Aes aes, string? text)
    {
        if (text == default) return default;
        return EncryptToByteArray(aes, text);
    }

    /// <summary>
    /// AES 加密(String → String)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Encrypt(this Aes aes, string text)
    {
        var result = EncryptToByteArray(aes, text);
        return result.Base64UrlEncode();
    }

    /// <summary>
    /// AES 加密(String → String)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncryptHex(this Aes aes, string text)
    {
        var result = EncryptToByteArray(aes, text);
        return result.ToHexString();
    }

    /// <summary>
    /// AES 加密(String → String)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Encrypt_Nullable(this Aes aes, string? text)
    {
        if (text == default) return default;
        return Encrypt(aes, text);
    }

    /// <summary>
    /// AES 加密(ByteArray → String)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncryptToString(this Aes aes, byte[] data)
    {
        var result = Encrypt(aes, data);
        return result.Base64UrlEncode();
    }

    /// <summary>
    /// AES 加密(ByteArray → String)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? EncryptToString_Nullable(this Aes aes, byte[]? data)
    {
        if (data == default) return default;
        return EncryptToString(aes, data);
    }

    #endregion

    #region 解密(Decrypt)

    /// <summary>
    /// AES 解密(ByteArray → ByteArray)
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Decrypt(this Aes aes, byte[] data)
    {
        using var transform = aes.CreateDecryptor();
        return transform.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// (解密) ByteArray → ByteArray
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? Decrypt_Nullable(this Aes aes, byte[]? data)
    {
        if (data == default) return default;
        return Decrypt(aes, data);
    }

    /// <summary>
    /// (解密) String → ByteArray
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] DecryptToByteArray(this Aes aes, string text)
    {
        var data = text.Base64UrlDecodeToByteArray();
        return Decrypt(aes, data);
    }

    /// <summary>
    /// (解密) String → ByteArray
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] DecryptHexToByteArray(this Aes aes, string text)
    {
        var data = Convert2.FromHexString(text);
        return Decrypt(aes, data);
    }

    /// <summary>
    /// (解密) String → ByteArray
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? DecryptToByteArray_Nullable(this Aes aes, string? text)
    {
        if (text == default) return default;
        return DecryptToByteArray(aes, text);
    }

    /// <summary>
    /// (解密) String → String
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Decrypt(this Aes aes, string text)
    {
        var result = DecryptToByteArray(aes, text);
        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// (解密) String → String
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecryptHex(this Aes aes, string text)
    {
        var result = DecryptHexToByteArray(aes, text);
        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// (解密) String → String
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Decrypt_Nullable(this Aes aes, string? text)
    {
        if (text == default) return default;
        return Decrypt(aes, text);
    }

    /// <summary>
    /// (解密) ByteArray → String
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecryptToString(this Aes aes, byte[] data)
    {
        var result = Decrypt(aes, data);
        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// (解密) ByteArray → String
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? DecryptToString_Nullable(this Aes aes, byte[]? data)
    {
        if (data == default) return default;
        return DecryptToString(aes, data);
    }

    #endregion

    /// <summary>
    /// <see cref="Aes"/> [密钥,向量,模式,填充] 模型类
    /// <para>使用内置函数序列化此模型，不可使用 JSON 或 MessagePack 处理此类</para>
    /// <para>String: <see cref="ToString"/> / <see cref="Create(string)"/></para>
    /// <para>ByteArray: <see cref="ToByteArray"/> / <see cref="Create(byte[])"/></para>
    /// </summary>
    internal sealed class Parameters
    {
        Parameters(CipherMode mode, PaddingMode padding)
        {
            _mode = mode;
            _padding = padding;
            OnPropertyChanged();
        }

        Parameters(Flags flags)
        {
            OnPropertyChanged(flags);
        }

        /// <summary>
        /// 使用指定的密钥、向量、加密模式和填充模式创建参数实例
        /// </summary>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="mode">加密模式，默认为密码块模式 <see cref="CipherMode.CBC"/></param>
        /// <param name="padding">填充模式，默认为 <see cref=" PaddingMode.PKCS7"/> 填充</param>
        public Parameters(string key, string iv, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7) : this(mode, padding)
        {
            Key = key;
            IV = iv;
        }

        /// <summary>
        /// 使用指定的密钥、向量、加密模式和填充模式创建参数实例
        /// </summary>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="mode">加密模式，默认为密码块模式 <see cref="CipherMode.CBC"/></param>
        /// <param name="padding">填充模式，默认为 <see cref=" PaddingMode.PKCS7"/> 填充</param>
        public Parameters(byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7) : this(mode, padding)
        {
            KeyByteArray = key;
            IVByteArray = iv;
        }

        /// <summary>
        /// 使用指定的密钥、向量和标志位创建参数实例
        /// </summary>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="flags">标志位</param>
        public Parameters(string key, string iv, Flags flags) : this(flags)
        {
            Key = key;
            IV = iv;
        }

        /// <summary>
        /// 使用指定的密钥、向量和标志位创建参数实例
        /// </summary>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="flags"><see cref="Flags"/></param>
        public Parameters(byte[] key, byte[] iv, Flags flags) : this(flags)
        {
            KeyByteArray = key;
            IVByteArray = iv;
        }

        /// <summary>
        /// 加密模式，填充枚举
        /// </summary>
        [Flags]
        public enum Flags
        {
            CipherMode_CBC = 2,

            /// <summary>
            /// ECB模式已被定义为不安全的，除非对接第三方服务要求使用之外，否则不应使用！
            /// </summary>
            [Obsolete("low safety.")]
            CipherMode_ECB = 16,

            //[Obsolete("target .NET Standard 1.4 is not supported.")]
            CipherMode_OFB = 32,

            //[Obsolete("target .NET Standard 1.4 is not supported.")]
            CipherMode_CFB = 64,

            CipherMode_CTS = 512,
            PaddingMode_None = 4096,
            PaddingMode_PKCS7 = 8,
            PaddingMode_Zeros = 1024,

            //[Obsolete("target .NET Standard 1.4 is not supported.")]
            PaddingMode_ANSIX923 = 2048,

            //[Obsolete("target .NET Standard 1.4 is not supported.")]
            PaddingMode_ISO10126 = 4,
        }

        string? _key;
        byte[]? _keyByteArray;
        string? _iv;
        byte[]? _ivByteArray;
        CipherMode _mode;
        PaddingMode _padding;
        Flags _flags;

        /// <summary>
        /// 模式与填充
        /// </summary>
        public Flags ModeAndPadding
        {
            get => _flags;
            set
            {
                if (value != _flags)
                {
                    OnPropertyChanged(value);
                }
            }
        }

        /// <summary>
        /// 密钥(String)
        /// </summary>
        public string? Key
        {
            get => _key;
            set
            {
                _key = value;
                _keyByteArray = value.Base64UrlDecodeToByteArray_Nullable();
            }
        }

        /// <summary>
        /// 密钥(ByteArray)
        /// </summary>
        public byte[]? KeyByteArray
        {
            get => _keyByteArray;
            set
            {
                _keyByteArray = value;
                _key = value.Base64UrlEncode_Nullable();
            }
        }

        /// <summary>
        /// 向量(String)
        /// </summary>
        public string? IV
        {
            get => _iv;
            set
            {
                _iv = value;
                _ivByteArray = value.Base64UrlDecodeToByteArray_Nullable();
            }
        }

        /// <summary>
        /// 向量(ByteArray)
        /// </summary>
        public byte[]? IVByteArray
        {
            get => _ivByteArray;
            set
            {
                _ivByteArray = value;
                _iv = value.Base64UrlEncode_Nullable();
            }
        }

        /// <summary>
        /// 模式
        /// </summary>
        public CipherMode Mode
        {
            get => _mode;
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 填充
        /// </summary>
        public PaddingMode Padding
        {
            get => _padding;
            set
            {
                if (value != _padding)
                {
                    _padding = value;
                    OnPropertyChanged();
                }
            }
        }

        void OnPropertyChanged()
        {
            var mode = Flags.CipherMode_CBC;
            var padding = Flags.PaddingMode_PKCS7;
#pragma warning disable CS0618 // 类型或成员已过时
            switch (_mode)
            {
                case CipherMode.ECB:
                    mode = Flags.CipherMode_ECB;
                    break;

                case CipherMode.OFB:
                    mode = Flags.CipherMode_OFB;
                    break;

                case CipherMode.CFB:
                    mode = Flags.CipherMode_CFB;
                    break;

                case CipherMode.CTS:
                    mode = Flags.CipherMode_CTS;
                    break;
            }
            switch (_padding)
            {
                case PaddingMode.None:
                    padding = Flags.PaddingMode_None;
                    break;

                case PaddingMode.Zeros:
                    padding = Flags.PaddingMode_Zeros;
                    break;

                case PaddingMode.ANSIX923:
                    padding = Flags.PaddingMode_ANSIX923;
                    break;

                case PaddingMode.ISO10126:
                    padding = Flags.PaddingMode_ISO10126;
                    break;
            }
#pragma warning restore CS0618 // 类型或成员已过时
            _flags = mode | padding;
        }

        void OnPropertyChanged(Flags flags)
        {
#pragma warning disable CS0618 // 类型或成员已过时
            if ((flags & Flags.CipherMode_CBC) == Flags.CipherMode_CBC)
                _mode = CipherMode.CBC;
            else if ((flags & Flags.CipherMode_CTS) == Flags.CipherMode_CTS)
                _mode = CipherMode.CTS;
            else if ((flags & Flags.CipherMode_OFB) == Flags.CipherMode_OFB)
                _mode = CipherMode.OFB;
            else if ((flags & Flags.CipherMode_CFB) == Flags.CipherMode_CFB)
                _mode = CipherMode.CFB;
            else if ((flags & Flags.CipherMode_ECB) == Flags.CipherMode_ECB)
                _mode = CipherMode.ECB;

            if ((flags & Flags.PaddingMode_PKCS7) == Flags.PaddingMode_PKCS7)
                _padding = PaddingMode.PKCS7;
            else if ((flags & Flags.PaddingMode_Zeros) == Flags.PaddingMode_Zeros)
                _padding = PaddingMode.Zeros;
            else if ((flags & Flags.PaddingMode_ANSIX923) == Flags.PaddingMode_ANSIX923)
                _padding = PaddingMode.ANSIX923;
            else if ((flags & Flags.PaddingMode_ISO10126) == Flags.PaddingMode_ISO10126)
                _padding = PaddingMode.ISO10126;
            else if ((flags & Flags.PaddingMode_None) == Flags.PaddingMode_None)
                _padding = PaddingMode.None;
#pragma warning restore CS0618 // 类型或成员已过时

            _flags = flags;
        }

        /// <summary>
        /// 从字节数组创建 <see cref="Parameters"/> 实例
        /// </summary>
        /// <param name="data">包含数据的字节数组</param>
        /// <returns><see cref="Parameters"/> 实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parameters Create(byte[] data)
        {
            var uint16 = BitConverter.ToUInt16(data, 0);
            var flags = (Flags)uint16;
            var mIVByteArray = data.Skip(2).Take(16).ToArray();
            var mKeyByteArray = data.Skip(18).Reverse().ToArray();
            return new Parameters(mKeyByteArray, mIVByteArray, flags);
        }

        /// <summary>
        /// 从字符串创建 <see cref="Parameters"/> 实例
        /// </summary>
        /// <param name="data">包含数据的字符串</param>
        /// <returns><see cref="Parameters"/> 实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Parameters Create(string data)
        {
            var bytes = data.Base64UrlDecodeToByteArray();
            return Create(bytes);
        }

        /// <summary>
        /// 将 <see cref="Parameters"/> 实例转换为字节数组
        /// </summary>
        /// <returns>转换后的字节数组</returns>
        public byte[] ToByteArray()
        {
            IVByteArray.ThrowIsNull();
            KeyByteArray.ThrowIsNull();
            return [.. BitConverter.GetBytes((ushort)ModeAndPadding), .. IVByteArray, .. KeyByteArray.Reverse()];
        }

        /// <summary>
        /// 将 <see cref="Parameters"/> 实例转换为 Base64 编码的字符串
        /// </summary>
        /// <returns>转换后的 Base64 编码字符串</returns>
        public new string ToString() => ToByteArray().Base64UrlEncode();

        /// <summary>
        /// 创建 Aes 实例
        /// </summary>
        /// <returns>Aes 实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aes Create()
        {
            IVByteArray.ThrowIsNull();
            KeyByteArray.ThrowIsNull();
            var aes = Aes.Create();
            aes.Key = KeyByteArray;
            aes.IV = IVByteArray;
            aes.Mode = Mode;
            aes.Padding = Padding;
            return aes;
        }

        /// <summary>
        /// 比较两个 <see cref="Parameters"/> 实例是否相等
        /// </summary>
        /// <param name="other">要比较的另一个 <see cref="Parameters"/> 实例</param>
        /// <returns>如果两个实例相等，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        public bool Equals(Parameters? other)
        {
            if (other == null) return false;
            return Mode == other.Mode &&
                Padding == other.Padding &&
                KeyByteArray.SequenceEqual_Nullable(other.KeyByteArray) &&
                IVByteArray.SequenceEqual_Nullable(other.IVByteArray);
        }
    }

    /// <summary>
    /// 根据字符串获取 KeyIV 参数
    /// </summary>
    /// <param name="s">输入的字符串</param>
    /// <returns>KeyIV 参数</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KeyIV GetParameters(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        return GetParameters(bytes);
    }

    /// <summary>
    /// 根据字节数组获取 KeyIV 参数
    /// </summary>
    /// <param name="bytes">输入的字节数组</param>
    /// <returns>KeyIV 参数</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KeyIV GetParameters(byte[] bytes)
    {
        var key = Hashs.ByteArray.SHA1(bytes).Concat(Hashs.ByteArray.Crc32(bytes)).ToArray();
        var iv = Hashs.ByteArray.MD5(bytes);
        return new() { Key = key, IV = iv };
    }

    /// <summary>
    /// 密钥和向量参数结构体
    /// </summary>
    public readonly struct KeyIV
    {
        /// <inheritdoc cref="SymmetricAlgorithm.Key"/>
        public required readonly byte[] Key { get; init; }

        /// <inheritdoc cref="SymmetricAlgorithm.IV"/>
        public required readonly byte[] IV { get; init; }
    }
}