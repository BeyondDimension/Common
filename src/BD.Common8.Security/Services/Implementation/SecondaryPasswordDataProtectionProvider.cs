namespace BD.Common8.Security.Services.Implementation;

/// <inheritdoc cref="ISecondaryPasswordDataProtectionProvider"/>
public class SecondaryPasswordDataProtectionProvider : ISecondaryPasswordDataProtectionProvider
{
    /// <inheritdoc cref="SecondaryPasswordDataProtectionType"/>
    protected readonly SecondaryPasswordDataProtectionType defaultESecondaryPasswordDataProtectionType;

    /// <summary>
    /// 初始化 <see cref="SecondaryPasswordDataProtectionProvider"/> 类的新实例
    /// 将默认的二次密码数据保护类型设置为 <see cref="SecondaryPasswordDataProtectionType.AesCBC"/>
    /// </summary>
    public SecondaryPasswordDataProtectionProvider()
    {
        defaultESecondaryPasswordDataProtectionType = SecondaryPasswordDataProtectionType.AesCBC;
    }

    /// <summary>
    /// 二级密码数据保护类型
    /// </summary>
    protected enum SecondaryPasswordDataProtectionType
    {
        AesCBC,
    }

    /// <summary>
    /// 将指定的字节数组与 <inheritdoc cref="defaultESecondaryPasswordDataProtectionType"/> 进行拼接
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte[] Concat(byte[] value)
    {
        using var stream = new MemoryStream();
        stream.Write(BitConverter.GetBytes((int)defaultESecondaryPasswordDataProtectionType));
        stream.Write(value);
        var r = stream.ToArray();
        return r;
    }

    /// <summary>
    /// 使用 AES 加密算法加密字节数组
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte[] Encrypt(Aes aes, byte[] value)
    {
        var r = aes.Encrypt(value);
        return r;
    }

    /// <inheritdoc/>
    public byte[]? EncryptBytesToBytes(byte[]? value, string secondaryPassword)
    {
        if (string.IsNullOrEmpty(secondaryPassword))
            throw new ArgumentNullException(nameof(secondaryPassword));

        if (value == null) return value;
        if (value.Length == 0) return value;

        return defaultESecondaryPasswordDataProtectionType switch
        {
            SecondaryPasswordDataProtectionType.AesCBC => DecryptAesCBC(),
            _ => null,
        };

        byte[]? DecryptAesCBC()
        {
            var keyIV = AESUtils.GetParameters(secondaryPassword);
            using var aes = AESUtils.Create(keyIV.Key, keyIV.IV, CipherMode.CBC, PaddingMode.PKCS7);

            var value_1 = Encrypt(aes, value);
            var value_1_r = Concat(value_1);
            return value_1_r;
        }
    }

    /// <summary>
    /// 使用 AES 加密算法解密字节数组
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte[]? Decrypt(Aes aes, byte[] value)
    {
        using var transform = aes.CreateDecryptor();
        var data_r = transform.TransformFinalBlock(value, sizeof(int), value.Length - sizeof(int));
        return data_r;
    }

    /// <inheritdoc/>
    public byte[]? DecryptBytesToBytes(byte[]? value, string secondaryPassword)
    {
        if (string.IsNullOrEmpty(secondaryPassword))
            throw new ArgumentNullException(nameof(secondaryPassword));

        if (value == null) return value;
        if (value.Length == 0) return value;
        if (value.Length <= sizeof(int)) return null;

        var d_type = (SecondaryPasswordDataProtectionType)BitConverter.ToInt32(value, 0);
        try
        {
            return d_type switch
            {
                SecondaryPasswordDataProtectionType.AesCBC => DecryptAesCBC(),
                _ => null,
            };
        }
        catch
        {
            return null;
        }

        byte[]? DecryptAesCBC()
        {
            var keyIV = AESUtils.GetParameters(secondaryPassword);
            using var aes = AESUtils.Create(keyIV.Key, keyIV.IV, CipherMode.CBC, PaddingMode.PKCS7);
            return Decrypt(aes, value);
        }
    }
}