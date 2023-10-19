namespace BD.Common8.Security.Services;

/// <summary>
/// 嵌入 AES 加密模式数据保护提供者
/// </summary>
public interface IEmbeddedAesDataProtectionProvider
{
    /// <summary>
    /// 字符串加密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    byte[]? EncryptStringToBytes(string? value);

    /// <summary>
    /// byte[] 加密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    byte[]? EncryptBytesToBytes(byte[]? value);

    /// <summary>
    /// byte[] 解密成字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    string? DecryptBytesToString(byte[]? value);

    /// <summary>
    /// byte[] 解密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    byte[]? DecryptBytesToBytes(byte[]? value);
}