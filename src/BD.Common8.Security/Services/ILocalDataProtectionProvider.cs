namespace BD.Common8.Security.Services;

/// <summary>
/// 本机加密模式数据保护提供者
/// </summary>
public interface ILocalDataProtectionProvider
{
    /// <summary>
    /// byte[] 加密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ValueTask<byte[]?> EncryptBytesToBytesAsync(byte[]? value);

    /// <summary>
    /// byte[] 解密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ValueTask<byte[]?> DecryptBytesToBytesAsync(byte[]? value);

    /// <summary>
    /// 用于数据保护服务的接口
    /// </summary>
    public interface IProtectedData
    {
        /// <summary>
        /// 加密数据
        /// </summary>
        byte[] Protect(byte[] userData);

        /// <summary>
        /// 解密数据并存储在字节数组中
        /// </summary>
        byte[] Unprotect(byte[] encryptedData);
    }

    /// <summary>
    /// 用于数据保护服务的接口
    /// </summary>
    public interface IDataProtectionProvider
    {
        /// <summary>
        /// 对数据进行加密数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回一个任务，该任务包含了加密后的数据，以字节数组形式返回</returns>
        Task<byte[]> ProtectAsync(byte[] data);

        /// <summary>
        /// 解密数据并存储在字节数组中
        /// </summary>
        /// <returns>返回一个任务，该任务包含了解密后的数据，以字节数组形式返回</returns>
        Task<byte[]> UnprotectAsync(byte[] data);
    }
}