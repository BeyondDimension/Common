using SR = BD.Common8.PersonalData.BirthDate.Resources.SR;

namespace BD.Common8.PersonalData.BirthDate.Helpers;

public static partial class BirthDateHelper
{
    /// <summary>
    /// 一年的天数
    /// </summary>
    const double AYearDays = 365.2425;

    /// <summary>
    /// 将年龄值转换为用于UI显示的字符串
    /// </summary>
    /// <param name="age"></param>
    /// <returns></returns>
    public static string ToString(byte age)
    {
        return SR.Age_.Format(age);
    }

    static byte AgeResultCorrect(int age)
    {
        if (age < 0)
            return 0;
        else if (age > IBirthDate.HumanMaxAge)
            return IBirthDate.HumanMaxAge;
        else
            return (byte)age;
    }

    /// <summary>
    /// 根据生日计算年龄
    /// </summary>
    /// <param name="birthDate"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    public static byte CalcAge(DateTime? birthDate, byte def = 0)
    {
        if (birthDate.HasValue)
        {
            var today = DateTime.Today;
            var birth = new DateTime(birthDate.Value.Year,
                birthDate.Value.Month,
                birthDate.Value.Day,
                0, 0, 0,
                today.Kind);
            var age = (int)((today - birth).TotalDays / AYearDays);
            return AgeResultCorrect(age);
        }
        return def;
    }

    /// <summary>
    /// 根据生日及用户所在时区计算年龄
    /// </summary>
    /// <param name="timeZone"></param>
    /// <param name="birthDate"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    public static byte CalcAge(sbyte timeZone, DateTime? birthDate, byte def = 0)
    {
        if (birthDate.HasValue)
        {
            var offset = TimeSpan.FromHours(timeZone);
            var today = DateTimeOffset.Now;
            var birth = new DateTimeOffset(birthDate.Value.Year,
                birthDate.Value.Month,
                birthDate.Value.Day,
                0, 0, 0,
                offset);
            var age = (int)((today - birth).TotalDays / AYearDays);
            return AgeResultCorrect(age);
        }
        return def;
    }

    /// <summary>
    /// 获取生日/出生日期
    /// </summary>
    /// <param name="birthDate"></param>
    /// <param name="forceLocal"></param>
    /// <returns></returns>
    public static DateTimeOffset? GetBirthDate(this IBirthDateTimeZone birthDate, bool forceLocal = true)
    {
        if (!birthDate.BirthDate.HasValue) return null;
        return new DateTimeOffset(birthDate.BirthDate.Value.Year,
                birthDate.BirthDate.Value.Month,
                birthDate.BirthDate.Value.Day,
                0, 0, 0,
                forceLocal ? TimeZoneInfo.Local.BaseUtcOffset : TimeSpan.FromHours(birthDate.BirthDateTimeZone));
    }

    /// <summary>
    /// 设置生日/出生日期
    /// </summary>
    /// <param name="birthDate"></param>
    /// <param name="value"></param>
    public static void SetBirthDate(this IBirthDateTimeZone birthDate, DateTimeOffset? value)
    {
        if (!value.HasValue)
        {
            birthDate.BirthDate = null;
            birthDate.BirthDateTimeZone = 0;
        }
        else
        {
            birthDate.BirthDate = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day,
                0, 0, 0,
                birthDate.BirthDate?.Kind ?? DateTimeKind.Unspecified);
            birthDate.BirthDateTimeZone = (sbyte)value.Value.Offset.TotalHours;
        }
    }
}