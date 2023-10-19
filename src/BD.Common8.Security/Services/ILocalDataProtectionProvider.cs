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

#pragma warning disable SA1600 // Elements should be documented
    public interface IProtectedData
    {
        byte[] Protect(byte[] userData);

        byte[] Unprotect(byte[] encryptedData);
    }

    public interface IDataProtectionProvider
    {
        Task<byte[]> ProtectAsync(byte[] data);

        Task<byte[]> UnprotectAsync(byte[] data);
    }
#pragma warning restore SA1600 // Elements should be documented
}