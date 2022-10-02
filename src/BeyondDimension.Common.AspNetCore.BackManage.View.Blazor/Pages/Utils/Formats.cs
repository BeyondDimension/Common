// ReSharper disable once CheckNamespace
namespace BD.Common.Pages.Utils;

public static partial class Formats
{
    public const string Date = "yy-MM-dd";
    public const string DateMonth = "yy-MM";
    public const string DateTime = "yy-MM-dd HH:mm:ss";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DateTryParse(string value, out DateTime date) => System.DateTime.TryParseExact(value,
                    Date,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DateTimeTryParse(string value, out DateTime date) => System.DateTime.TryParseExact(value,
                    DateTime,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecimalFormat(decimal d) => d.ToString("F4");
}
