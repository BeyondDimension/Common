namespace BD.Common.Columns;

/// <summary>
/// 手机号码
/// </summary>
public interface IPhoneNumber
{
    /// <inheritdoc cref="IPhoneNumber"/>
    string? PhoneNumber { get; set; }

    public const int Db_MaxLength_PhoneNumber = 20;
}