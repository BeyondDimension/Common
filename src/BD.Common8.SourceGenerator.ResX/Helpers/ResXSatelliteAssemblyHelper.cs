namespace BD.Common8.SourceGenerator.ResX.Helpers;

/// <summary>
/// ResX 附属资源程序集助手类
/// <para>xyz.resources.dll</para>
/// </summary>
static partial class ResXSatelliteAssemblyHelper
{
    /// <summary>
    /// 判断 cultureName 是否为假的
    /// </summary>
    /// <param name="cultureName"></param>
    /// <returns></returns>
    public static bool IsFakeCultureName(string cultureName)
    {
        try
        {
            var culture = new CultureInfo(cultureName);
            if (culture.EnglishName.StartsWith("Unknown Language"))
                return true;
            if (culture.EnglishName.Equals(cultureName, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 尝试从附属资源文件路径中获取 cultureName
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cultureName"></param>
    /// <returns></returns>
    public static bool TryGetCultureNameByResXSatelliteFilePathPath(
        string filePath,
        [NotNullWhen(true)] out string? cultureName)
    {
        cultureName = null;
        try
        {
            var temp = new int[2];
            var temp_i = 0;
            for (int i = filePath.Length - 1; i >= 0; i--)
            {
                if (filePath[i] == '.')
                {
                    temp[temp_i++] = i;
                    if (temp_i + 1 > temp.Length)
                        break;
                }
            }

            if (filePath[temp[0] + 1] == 'r' &&
                filePath[temp[0] + 2] == 'e' &&
                filePath[temp[0] + 3] == 's' &&
                filePath[temp[0] + 4] == 'x')
            {
                if (temp[1] == 0)
                    return false;
                try
                {
                    cultureName = filePath.Substring(temp[1] + 1, temp[0] - temp[1] - 1);
                    return !IsFakeCultureName(cultureName);
                }
                catch
                {
                    return false;
                }
            }
        }
        catch
        {
        }
        return false;
    }
}