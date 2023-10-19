namespace BD.Common8.Primitives.PersonalData.BirthDate.Columns;

/// <summary>
/// 生日/出生日期
/// </summary>
public interface IBirthDate
{
    /// <inheritdoc cref="IBirthDate"/>
    DateTime? BirthDate { get; set; }

    /// <summary>
    /// 人类最长寿命(岁)
    /// </summary>
    public const int HumanMaxAge = 122;

    /// <summary>
    /// 出生日期默认选择值(18岁)
    /// </summary>
    public static DateTime DefaultSelected => DateTime.Now.Date.AddYears(-18).AddDays(-1);

    /// <summary>
    /// 出生日期选择范围
    /// </summary>
    public static (DateTime min, DateTime max) SelectionRange
    {
        get
        {
            var max = DateTime.Now.Date;
            var min = max.AddYears(0 - HumanMaxAge - 1);
            return (min, max);
        }
    }
}

/// <inheritdoc cref="IBirthDate"/>
public interface IReadOnlyBirthDate
{
    /// <inheritdoc cref="IBirthDate"/>
    DateTime BirthDate { get; }
}

/// <inheritdoc cref="IBirthDate"/>
public interface IBirthDateTimeZone : IBirthDate
{
    /// <summary>
    /// 填写生日/出生日期纪录的客户端所在时区
    /// </summary>
    sbyte BirthDateTimeZone { get; set; }
}