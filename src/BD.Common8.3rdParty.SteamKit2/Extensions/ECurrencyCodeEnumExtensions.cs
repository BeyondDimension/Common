using System.Globalization;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace SteamKit2;

/// <summary>
/// 枚举 <see cref="ECurrencyCode"/> 的扩展方法
/// </summary>
public static partial class ECurrencyCodeEnumExtensions
{
    /// <summary>
    /// 根据货币获取文化信息
    /// </summary>
    /// <param name="eCurrencyCode"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CultureInfo? GetCultureInfo(this ECurrencyCode eCurrencyCode)
    {
        if (eCurrencyCode == ECurrencyCode.Invalid)
        {
            return null;
        }
        var str = eCurrencyCode.ToString();
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        var query = from m in cultures
                    let r = new RegionInfo(m.LCID)
                    where string.Equals(r.ISOCurrencySymbol, str, StringComparison.OrdinalIgnoreCase)
                    select m;
        var result = query.FirstOrDefault();
        return result;
    }
}