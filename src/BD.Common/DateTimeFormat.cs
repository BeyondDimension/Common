namespace System;

/// <summary>
/// 常用日期时间格式化字符串
/// </summary>
public static partial class DateTimeFormat
{
    /// <summary>
    /// 年月日时分秒 7 位毫秒
    /// <para>yyyy-MM-dd HH:mm:ss.fffffff</para>
    /// </summary>
    public const string Complete = "yyyy-MM-dd HH:mm:ss.fffffff";

    /// <summary>
    /// 年月日时分秒 3 位毫秒
    /// <para>yyyy-MM-dd HH:mm:ss.fff</para>
    /// </summary>
    public const string F3 = "yyyy-MM-dd HH:mm:ss.fff";

    /// <summary>
    /// 月日时分秒 7 位毫秒
    /// <para>MM-dd HH:mm:ss.fffffff</para>
    /// </summary>
    public const string Debug = "MM-dd HH:mm:ss.fffffff";

    /// <summary>
    /// 月日时分秒 3 位毫秒
    /// <para>MM-dd HH:mm:ss.fff</para>
    /// </summary>
    public const string Debug2 = "MM-dd HH:mm:ss.fff";

    /// <summary>
    /// 时分秒 7 位毫秒
    /// <para>HH:mm:ss.fffffff</para>
    /// </summary>
    public const string Debug3 = "HH:mm:ss.fffffff";

    /// <summary>
    /// 月日时分秒
    /// <para>MM-dd HH:mm:ss</para>
    /// </summary>
    public const string Debug4 = "MM-dd HH:mm:ss";

    /// <summary>
    /// 日时分秒 3 位毫秒
    /// <para>dd HH:mm:ss.fff</para>
    /// </summary>
    public const string Debug5 = "dd HH:mm:ss.fff";

    /// <summary>
    /// 年月日时分秒
    /// <para>yyyy-MM-dd HH:mm:ss</para>
    /// </summary>
    public const string Standard = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// 年月日时分秒 7 位毫秒
    /// <para>yyyyMMddHHmmssfffffff</para>
    /// </summary>
    public const string Connect = "yyyyMMddHHmmssfffffff";

    /// <summary>
    /// 年月日
    /// <para>yyyy-MM-dd</para>
    /// </summary>
    public const string Date = "yyyy-MM-dd";

    /// <summary>
    /// 月日时分
    /// <para>MM-dd HH:mm</para>
    /// </summary>
    public const string NoYearNoSecond = "MM-dd HH:mm";

    /// <summary>
    /// 年月日
    /// <para>yyyy年MM月dd日</para>
    /// </summary>
    public const string DateCN = "yyyy年MM月dd日";

    /// <summary>
    /// 年月日
    /// <para>yyyy年M月d日</para>
    /// </summary>
    public const string DateCN2 = "yyyy年M月d日";

    /// <summary>
    /// “R”或“r”标准格式说明符表示由 DateTimeFormatInfo.RFC1123Pattern 属性定义的自定义日期和时间格式字符串。 该模式反映已定义的标准，并且属性是只读的。 因此，无论所使用的区域性或所提供的格式提供程序是什么，它总是相同的。 定义格式字符串为“ddd, dd MMM yyyy HH':'mm':'ss 'GMT'”。 当使用此标准格式说明符时，格式设置或分析操作始终使用固定区域性
    /// <para>https://learn.microsoft.com/zh-cn/dotnet/standard/base-types/standard-date-and-time-format-strings#RFC1123</para>
    /// </summary>
    public const string RFC1123 = "r";

    /// <summary>
    /// 年月日时分秒 7 位毫秒
    /// <para>yyyy-MM-dd HHmmssfffffff</para>
    /// </summary>
    public const string File = "yyyy-MM-dd HHmmssfffffff";

    /// <summary>
    /// 年月
    /// <para>yyyy/MM</para>
    /// </summary>
    public const string YearMonth = "yyyy/MM";
}
