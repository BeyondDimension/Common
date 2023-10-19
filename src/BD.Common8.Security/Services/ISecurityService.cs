namespace BD.Common8.Security.Services;

/// <summary>
/// 安全服务
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// 此值不可更改
    /// </summary>
    const EncryptionMode DefaultEncryptionMode = EncryptionMode.EmbeddedAesWithLocal;

    /// <summary>
    /// 字符串加密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="encryptionMode"></param>
    /// <param name="secondaryPassword"></param>
    /// <returns></returns>
    ValueTask<byte[]?> EncryptStringToBytesAsync(string? value, EncryptionMode encryptionMode = DefaultEncryptionMode, string? secondaryPassword = null);

    /// <summary>
    /// byte[] 加密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="encryptionMode"></param>
    /// <param name="secondaryPassword"></param>
    /// <returns></returns>
    ValueTask<byte[]?> EncryptBytesToBytesAsync(byte[]? value, EncryptionMode encryptionMode = DefaultEncryptionMode, string? secondaryPassword = null);

    /// <summary>
    /// byte[] 解密成字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="secondaryPassword"></param>
    /// <returns></returns>
    ValueTask<StringDecryptResult> DecryptBytesToStringAsync(byte[]? value, string? secondaryPassword = null);

    /// <summary>
    /// byte[] 解密成 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="secondaryPassword"></param>
    /// <returns></returns>
    ValueTask<BytesDecryptResult> DecryptBytesToBytesAsync(byte[]? value, string? secondaryPassword = null);

    /// <summary>
    /// 解密结果状态码
    /// </summary>
    public enum DecryptResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,

        /// <summary>
        /// 嵌入 Aes 失败
        /// </summary>
        EmbeddedAesFail = 901,

        /// <summary>
        /// 仅本机失败
        /// </summary>
        LocalFail,

        /// <summary>
        /// 二级密码失败
        /// </summary>
        SecondaryPasswordFail,

        /// <summary>
        /// 密文值不正确
        /// </summary>
        IncorrectValueFail,

        /// <summary>
        /// 转换字符串失败
        /// </summary>
        UTF8GetStringFail,
    }

    readonly record struct StringDecryptResult
    {
        public required string? Content { get; init; }

        public required DecryptResultCode ResultCode { get; init; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string?(StringDecryptResult result) => result.Content;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DecryptResultCode(StringDecryptResult result) => result.ResultCode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StringDecryptResult(DecryptResultCode resultCode) => new() { Content = null, ResultCode = resultCode };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringDecryptResult Success(string? content) => new() { Content = content, ResultCode = DecryptResultCode.Success };
    }

    readonly record struct BytesDecryptResult
    {
        public required byte[]? Content { get; init; }

        public required DecryptResultCode ResultCode { get; init; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte[]?(BytesDecryptResult result) => result.Content;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DecryptResultCode(BytesDecryptResult result) => result.ResultCode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator BytesDecryptResult(DecryptResultCode resultCode) => new() { Content = null, ResultCode = resultCode };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BytesDecryptResult Success(byte[]? content) => new() { Content = content, ResultCode = DecryptResultCode.Success };
    }
}