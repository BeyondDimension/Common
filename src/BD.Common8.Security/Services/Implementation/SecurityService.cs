using static BD.Common8.Security.Services.ISecurityService;

namespace BD.Common8.Security.Services.Implementation;

/// <inheritdoc cref="ISecurityService"/>
sealed class SecurityService : ISecurityService
{
    readonly IEmbeddedAesDataProtectionProvider ea;
    readonly ILocalDataProtectionProvider local;
    readonly ISecondaryPasswordDataProtectionProvider sp;

    /// <summary>
    /// 初始化 <inheritdoc cref="SecurityService"/> 的实例
    /// </summary>
    /// <param name="ea"></param>
    /// <param name="local"></param>
    /// <param name="sp"></param>
    public SecurityService(
        IEmbeddedAesDataProtectionProvider ea,
        ILocalDataProtectionProvider local,
        ISecondaryPasswordDataProtectionProvider sp)
    {
        this.ea = ea;
        this.local = local;
        this.sp = sp;
    }

    /// <summary>
    /// 将指定的字节数组和 <inheritdoc cref="EncryptionMode"/> 连接起来
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte[]? Concat(byte[]? value, EncryptionMode encryptionMode)
    {
        if (value == null || value.Length == 0)
            return null;
        using var stream = new MemoryStream();
        stream.Write(BitConverter.GetBytes((int)encryptionMode));
        stream.Write(value);
        var r = stream.ToArray();
        return r;
    }

    /// <inheritdoc/>
    public async ValueTask<byte[]?> EncryptStringToBytesAsync(string? value, EncryptionMode encryptionMode, string? secondaryPassword)
    {
        if (string.IsNullOrEmpty(value)) return null;
        var value2 = Encoding.UTF8.GetBytes(value);
        return await EncryptBytesToBytesAsync(value2, encryptionMode, secondaryPassword);
    }

    /// <inheritdoc/>
    public async ValueTask<byte[]?> EncryptBytesToBytesAsync(byte[]? value, EncryptionMode encryptionMode, string? secondaryPassword)
    {
        if (value == null || value.Length == 0)
            return value;

        switch (encryptionMode)
        {
            case EncryptionMode.EmbeddedAes:
                var value_1 = ea.EncryptBytesToBytes(value);
                var value_1_r = Concat(value_1, encryptionMode);
                return value_1_r;
            case EncryptionMode.EmbeddedAesWithLocal:
                var value_2 = ea.EncryptBytesToBytes(value);
                var value_2_local = await local.EncryptBytesToBytesAsync(value_2);
                var value_2_r = Concat(value_2_local, encryptionMode);
                return value_2_r;
            case EncryptionMode.EmbeddedAesWithSecondaryPassword:
                if (string.IsNullOrWhiteSpace(secondaryPassword))
                    throw new ArgumentNullException(nameof(secondaryPassword));
                var value_3 = ea.EncryptBytesToBytes(value);
                var value_3_sp = sp.EncryptBytesToBytes(value_3, secondaryPassword);
                var value_3_r = Concat(value_3_sp, encryptionMode);
                return value_3_r;
            case EncryptionMode.EmbeddedAesWithSecondaryPasswordWithLocal:
                if (string.IsNullOrWhiteSpace(secondaryPassword))
                    throw new ArgumentNullException(nameof(secondaryPassword));
                var value_4 = ea.EncryptBytesToBytes(value);
                var value_4_sp = sp.EncryptBytesToBytes(value_4, secondaryPassword);
                var value_4_local = await local.EncryptBytesToBytesAsync(value_4_sp);
                var value_4_r = Concat(value_4_local, encryptionMode);
                return value_4_r;
            default:
                throw ThrowHelper.GetArgumentOutOfRangeException(encryptionMode);
        }
    }

    /// <summary>
    /// 将一个字节数组剔除前面4个字节
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte[] UnConcat(byte[] value) => value.Skip(sizeof(int)).ToArray();

    /// <inheritdoc/>
    public async ValueTask<StringDecryptResult> DecryptBytesToStringAsync(byte[]? value, string? secondaryPassword)
    {
        var result = await DecryptBytesToBytesAsync(value, secondaryPassword);
        if (result.ResultCode != DecryptResultCode.Success || result.Content == null || result.Content.Length == 0)
            return result.ResultCode;
        try
        {
            var content_ = Encoding.UTF8.GetString(result.Content);
            return StringDecryptResult.Success(content_);
        }
        catch
        {
            return DecryptResultCode.UTF8GetStringFail;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<BytesDecryptResult> DecryptBytesToBytesAsync(byte[]? value, string? secondaryPassword)
    {
        if (value == null || value.Length == 0)
            return BytesDecryptResult.Success(value);
        if (value.Length <= sizeof(int))
            return DecryptResultCode.IncorrectValueFail;

        var d_type = (EncryptionMode)BitConverter.ToInt32(value, 0);
        switch (d_type)
        {
            case EncryptionMode.EmbeddedAes:
                var value_1 = UnConcat(value);
                var value_1_r = ea.DecryptBytesToBytes(value_1);
                return new BytesDecryptResult
                {
                    Content = value_1_r,
                    ResultCode = value_1_r == null ? DecryptResultCode.EmbeddedAesFail : DecryptResultCode.Success,
                };
            case EncryptionMode.EmbeddedAesWithLocal:
                var value_2 = UnConcat(value);
                var value_2_local = await local.DecryptBytesToBytesAsync(value_2);
                if (value_2_local == null)
                    return DecryptResultCode.LocalFail;
                var value_2_r = ea.DecryptBytesToBytes(value_2_local);
                return new BytesDecryptResult
                {
                    Content = value_2_r,
                    ResultCode = value_2_r == null ? DecryptResultCode.EmbeddedAesFail : DecryptResultCode.Success,
                };
            case EncryptionMode.EmbeddedAesWithSecondaryPassword:
                if (string.IsNullOrWhiteSpace(secondaryPassword))
                    return DecryptResultCode.SecondaryPasswordFail;
                var value_3 = UnConcat(value);
                var value_3_sp = sp.DecryptBytesToBytes(value_3, secondaryPassword);
                if (value_3_sp == null)
                    return DecryptResultCode.SecondaryPasswordFail;
                var value_3_r = ea.DecryptBytesToBytes(value_3_sp);
                return new BytesDecryptResult
                {
                    Content = value_3_r,
                    ResultCode = value_3_r == null ? DecryptResultCode.EmbeddedAesFail : DecryptResultCode.Success,
                };
            case EncryptionMode.EmbeddedAesWithSecondaryPasswordWithLocal:
                if (string.IsNullOrWhiteSpace(secondaryPassword))
                    return DecryptResultCode.SecondaryPasswordFail;
                var value_4 = UnConcat(value);
                var value_4_local = await local.DecryptBytesToBytesAsync(value_4);
                if (value_4_local == null)
                    return DecryptResultCode.LocalFail;
                var value_4_sp = sp.DecryptBytesToBytes(value_4_local, secondaryPassword);
                if (value_4_sp == null)
                    return DecryptResultCode.SecondaryPasswordFail;
                var value_4_r = ea.DecryptBytesToBytes(value_4_sp);
                return new BytesDecryptResult
                {
                    Content = value_4_r,
                    ResultCode = value_4_r == null ? DecryptResultCode.EmbeddedAesFail : DecryptResultCode.Success,
                };
            default:
                return DecryptResultCode.IncorrectValueFail;
        }
    }
}