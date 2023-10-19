namespace BD.Common8.Security.Services;

/// <summary>
/// 二级密码加密模式数据保护提供者
/// </summary>
public interface ISecondaryPasswordDataProtectionProvider
{
    /// <summary>
    /// byte[] 加密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="secondaryPassword"></param>
    /// <returns></returns>
    byte[]? EncryptBytesToBytes(byte[]? value, string secondaryPassword);

    /// <summary>
    /// byte[] 解密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="secondaryPassword"></param>
    /// <returns></returns>
    byte[]? DecryptBytesToBytes(byte[]? value, string secondaryPassword);
}
