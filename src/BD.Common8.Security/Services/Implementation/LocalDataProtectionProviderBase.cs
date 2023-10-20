using static BD.Common8.Security.Services.ILocalDataProtectionProvider;

namespace BD.Common8.Security.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <inheritdoc cref="ILocalDataProtectionProvider"/>
public abstract class LocalDataProtectionProviderBase : ILocalDataProtectionProvider
{
    readonly Lazy<Aes> _aes;

#pragma warning disable IDE1006 // 命名样式
    protected Aes aes => _aes.Value;
#pragma warning restore IDE1006 // 命名样式

    protected readonly LocalDataProtectionType defaultELocalDataProtectionType;
    protected readonly IProtectedData protectedData;
    protected readonly IDataProtectionProvider dataProtectionProvider;

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

    protected enum LocalDataProtectionType
    {
        None,

        AesCFB,

        ProtectedDataWithAesCFB,

        Win10WithAesCFB,
    }

    protected virtual AESUtils.KeyIV MachineSecretKey => MachineUniqueIdentifier.MachineSecretKey;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte[] Concat(byte[] value)
    {
        using var stream = new MemoryStream();
        stream.Write(BitConverter.GetBytes((int)defaultELocalDataProtectionType));
        stream.Write(value);
        var r = stream.ToArray();
        return r;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte[]? Decrypt(byte[] value)
    {
        using var transform = aes.CreateDecryptor();
        var data_r = transform.TransformFinalBlock(value, sizeof(int), value.Length - sizeof(int));
        return data_r;
    }

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

    public sealed class EmptyProtectedData : IProtectedData
    {
        public byte[] Protect(byte[] userData) => userData;

        public byte[] Unprotect(byte[] encryptedData) => encryptedData;
    }

    public sealed class EmptyDataProtectionProvider : IDataProtectionProvider
    {
        public Task<byte[]> ProtectAsync(byte[] data) => Task.FromResult(data);

        public Task<byte[]> UnprotectAsync(byte[] data) => Task.FromResult(data);
    }
}