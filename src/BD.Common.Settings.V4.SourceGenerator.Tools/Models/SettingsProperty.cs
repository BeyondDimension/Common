using System.Diagnostics.CodeAnalysis;

namespace BD.Common.Settings.Models;

/// <summary>
/// 设置属性
/// </summary>
/// <param name="IsValueType">是否为值类型</param>
/// <param name="TypeName">类型名称</param>
/// <param name="PropertyName">属性名称</param>
/// <param name="DefaultValue">默认值</param>
/// <param name="DefaultValueIsConst">默认值是否为常量</param>
/// <param name="Summary">注释</param>
public sealed record class SettingsProperty(
    string TypeName = "",
    string PropertyName = "",
    string DefaultValue = "",
    bool DefaultValueIsConst = true,
    string Summary = "",
    bool? IsRegionOrEndregion = null,
    string? Sharp = null,
    bool? IsValueType = null)
{
    public bool GetIsValueType()
    {
        if (IsValueType.HasValue)
            return IsValueType.Value;
        if (TypeName.EndsWith("[]"))
            return false;
        return TypeName switch
        {
            "sbyte" or "byte" or
            "short" or "ushort" or
            "int" or "uint" or
            "long" or "ulong" or
            "double" or "float" or "decimal" or
            "bool" => true,
            _ => false,
        };
    }

    public bool IsDictionary([NotNullWhen(true)] out string? key, [NotNullWhen(true)] out string? value)
    {
        try
        {
            if (AppSettings.Instance.DictionaryTypes.Any(TypeName.StartsWith))
            {
                var index = TypeName.IndexOf('<');
                var arr = TypeName.Substring(index + 1, TypeName.Length - index - 2).Split(',');
                if (arr.Length == 2)
                {
                    key = arr[0].Trim();
                    value = arr[1].Trim();
                    return true;
                }
            }
        }
        catch
        {

        }
        key = value = null;
        return false;
    }

    public bool IsCollection([NotNullWhen(true)] out string? value)
    {
        try
        {
            if (AppSettings.Instance.CollectionTypes.Any(TypeName.StartsWith))
            {
                var index = TypeName.IndexOf('<');
                value = TypeName.Substring(index + 1, TypeName.Length - index - 2);
                return true;
            }
            else if (TypeName.EndsWith("[]"))
            {
                value = TypeName[..^2];
                return true;
            }
        }
        catch
        {

        }
        value = default;
        return false;
    }
}
