namespace BD.Common8.SourceGenerator.Helpers;

/// <summary>
/// 提供用于处理 AttributeData 类
/// </summary>
public static class AttributeDataHelper
{
    /// <summary>
    /// 获取特性的完整类名
    /// </summary>
    public static string? GetClassFullName(this AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString();

    /// <summary>
    /// 判断特性的类名是否与给定的特性类名相等
    /// </summary>
    public static bool ClassNameEquals(this AttributeData attribute, string attributeClassFullName)
        => attribute.GetClassFullName() == attributeClassFullName;

    /// <summary>
    /// 判断特性是否为 <see cref="System.ComponentModel.DescriptionAttribute"/> 类
    /// </summary>
    public static bool IsDescription(AttributeData attribute)
        => attribute.ClassNameEquals("System.ComponentModel.DescriptionAttribute");

    /// <summary>
    /// 获取特性的描述值
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static string? GetDescription(this ImmutableArray<AttributeData> attributes)
    {
        var description = attributes.FirstOrDefault(IsDescription)?.
            ConstructorArguments.FirstOrDefault().Value;
        return description?.ToString();
    }

    public static string[]? GetStrings(this TypedConstant typedConstant)
    {
        if (typedConstant.IsNull)
            return null;

        var arr = typedConstant.Values.Select(x => x.GetObjectValue()?.ToString()!).Where(x => !string.IsNullOrEmpty(x))?.ToArray();
        if (arr == null)
            return null;

        if (arr.Length == 0)
            return null;

        return arr;
    }

    /// <summary>
    /// 获取对象值
    /// </summary>
    public static object? GetObjectValue(this TypedConstant typedConstant)
    {
        try
        {
            return typedConstant.Value;
        }
        catch
        {
        }
        try
        {
            return typedConstant.Values.Select(x => x.Value is TypedConstant typedConstant1 ? typedConstant1.GetObjectValue() : x.Value);
        }
        catch
        {
        }
        return null;
    }
}
