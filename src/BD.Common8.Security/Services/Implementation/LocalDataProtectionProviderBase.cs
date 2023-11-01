using static BD.Common8.Security.Services.ILocalDataProtectionProvider;

namespace BD.Common8.Security.Services.Implementation;

/// <inheritdoc cref="ILocalDataProtectionProvider"/>
public abstract class LocalDataProtectionProviderBase : ILocalDataProtectionProvider
{
    readonly Lazy<Aes> _aes;

#pragma warning disable IDE1006 // 命名样式
    /// <summary>
    /// 获取 Aes 实例
    /// </summary>
    protected Aes aes => _aes.Value;
#pragma warning restore IDE1006 // 命名样式

    /// <summary>
    /// 默认的本地数据保护类型
    /// </summary>
    protected readonly LocalDataProtectionType defaultELocalDataProtectionType;

    /// <summary>
    /// <see cref="IProtectedData"/>
    /// </summary>
    protected readonly IProtectedData protectedData;

    /// <summary>
    /// <see cref="dataProtectionProvider"/>
    /// </summary>
    protected readonly IDataProtectionProvider dataProtectionProvider;

    /// <summary>
    /// 初始化 <see cref="LocalDataProtectionProviderBase"/>
    /// </summary>
    /// <param name="protectedData"></param>
    /// <param name="dataProtectionProvider"></param>
    public LocalDataProtectionProviderBase(
        IProtectedData protectedData,
        IDataProtectionProvider dataProtectionProvider)
    {
        this.protectedData = protectedData;
        this.dataProtectionProvider = dataProtectionProvider;
        if (OperatingSystem.IsWindows())
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240))
                defaultELocalDataProtectionType = LocalDataProtectionType.Win10WithAesCFB;
            else
                defaultELocalDataProtectionType = LocalDataProtectionType.ProtectedDataWithAesCFB;
        else
            defaultELocalDataProtectionType = LocalDataProtectionType.AesCFB;
        _aes = new Lazy<Aes>(() =>
        {
            // https://github.com/dotnet/runtime/issues/42214#issuecomment-698495584
            // AES CFB in Windows 7 catch Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException: Unknown error (0xc10000bb)
            // AES CFB in Android catch CryptographicException: Bad PKCS7 padding. Invalid length
            var machineSecretKey = MachineSecretKey;
            var mode = OperatingSystem.IsAndroid() ? CipherMode.CBC : CipherMode.CFB;
            var r = AESUtils.Create(machineSecretKey.Key,
                machineSecretKey.IV, mode, PaddingMode.PKCS7);
            return r;
        });
    }

    /// <summary>
    /// 本机数据保护类型枚举
    /// </summary>
    protected enum LocalDataProtectionType
    {
        None,

        AesCFB,

        ProtectedDataWithAesCFB,

        Win10WithAesCFB,
    }

    /// <summary>
    /// 获取本机唯一标识的加密密钥和初始向量值
    /// </summary>
    protected virtual AESUtils.KeyIV MachineSecretKey => MachineUniqueIdentifier.MachineSecretKey;

    /// <summary>
    /// 将指定的字节数组与 <inheritdoc cref="defaultELocalDataProtectionType"/> 进行拼接
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte[] Concat(byte[] value)
    {
        using var stream = new MemoryStream();
        stream.Write(BitConverter.GetBytes((int)defaultELocalDataProtectionType));
        stream.Write(value);
        var r = stream.ToArray();
        return r;
    }

    /// <summary>
    /// 使用 AES 加密算法加密字节数组
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte[] Encrypt(byte[] value)
    {
        var r = aes.Encrypt(value);
        return r;
    }

    /// <inheritdoc/>
    public async ValueTask<byte[]?> EncryptBytesToBytesAsync(byte[]? value)
    {
        if (value == null) return value;
        if (value.Length == 0) return value;
        switch (defaultELocalDataProtectionType)
        {
            case LocalDataProtectionType.None:
                return value;
            case LocalDataProtectionType.AesCFB:
                var value_1 = Encrypt(value);
                var value_1_r = Concat(value_1);
                return value_1_r;
            case LocalDataProtectionType.ProtectedDataWithAesCFB:
                var value_2 = Encrypt(value);
                var value_2_pd = protectedData.Protect(value_2);
                var value_2_r = Concat(value_2_pd);
                return value_2_r;
            case LocalDataProtectionType.Win10WithAesCFB:
                var value_3 = Encrypt(value);
                var value_3_dpp = await dataProtectionProvider.ProtectAsync(value_3);
                var value_3_r = Concat(value_3_dpp);
                return value_3_r;
            default:
                throw ThrowHelper.GetArgumentOutOfRangeException(defaultELocalDataProtectionType);
        }
    }

    /// <summary>
    /// 使用 AES 加密算法解密字节数组
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte[]? Decrypt(byte[] value)
    {
        using var transform = aes.CreateDecryptor();
        var data_r = transform.TransformFinalBlock(value, sizeof(int), value.Length - sizeof(int));
        return data_r;
    }

    /// <summary>
    /// 将一个字节数组剔除前面4个字节
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static byte[] UnConcat(byte[] value) => value.Skip(sizeof(int)).ToArray();

    /// <inheritdoc/>
    public async ValueTask<byte[]?> DecryptBytesToBytesAsync(byte[]? value)
    {
        if (value == null)
            return value;
        if (value.Length == 0)
            return value;
        if (value.Length <= sizeof(int))
            return null;

        var d_type = (LocalDataProtectionType)BitConverter.ToInt32(value, 0);
        try
        {
            switch (d_type)
            {
                case LocalDataProtectionType.None:
                    return value;
                case LocalDataProtectionType.AesCFB:
                    return Decrypt(value);
                case LocalDataProtectionType.ProtectedDataWithAesCFB:
                    var value_2 = UnConcat(value);
                    var value_2_pd = protectedData.Unprotect(value_2);
                    var value_2_r = AESUtils.Decrypt(aes, value_2_pd);
                    return value_2_r;
                case LocalDataProtectionType.Win10WithAesCFB:
                    var value_3 = UnConcat(value);
                    var value_3_dpp = await dataProtectionProvider.UnprotectAsync(value_3);
                    var value_3_r = AESUtils.Decrypt(aes, value_3_dpp);
                    return value_3_r;
                default:
                    return null;
            }
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc cref="IProtectedData"/>
    public sealed class EmptyProtectedData : IProtectedData
    {
        /// <inheritdoc/>
        public byte[] Protect(byte[] userData) => userData;

        /// <inheritdoc/>
        public byte[] Unprotect(byte[] encryptedData) => encryptedData;
    }

    /// <inheritdoc cref="IDataProtectionProvider"/>
    public sealed class EmptyDataProtectionProvider : IDataProtectionProvider
    {
        /// <inheritdoc/>
        public Task<byte[]> ProtectAsync(byte[] data) => Task.FromResult(data);

        /// <inheritdoc/>
        public Task<byte[]> UnprotectAsync(byte[] data) => Task.FromResult(data);
    }
}