namespace BD.Common8.Primitives.Columns;

/// <summary>
/// 列(只读) - 短信验证码
/// </summary>
public interface IReadOnlySmsCode
{
    /// <inheritdoc cref="ISmsCode.SmsCode"/>
    string? SmsCode { get; }
}