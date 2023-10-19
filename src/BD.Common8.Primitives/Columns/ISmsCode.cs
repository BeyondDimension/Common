namespace BD.Common8.Primitives.Columns;

/// <summary>
/// 列 - 短信验证码
/// </summary>
public interface ISmsCode
{
    /// <summary>
    /// 短信验证码
    /// </summary>
    string? SmsCode { get; set; }
}