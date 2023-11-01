namespace BD.Common8.SourceGenerator.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 提供 用于处理 AttributeData 类
/// </summary>
public static class AttributeDataHelper
{
    /// <summary>
    /// 获取特性的完整类名
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetClassFullName(this AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString();

    /// <summary>
    /// 判断特性的类名是否与给定的特性类名相等
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ClassNameEquals(this AttributeData attribute, string attributeClassFullName)
        => attribute.GetClassFullName() == attributeClassFullName;

    /// <summary>
    /// 判断特性是否为 DescriptionAttribute 类
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDescription(AttributeData attribute)
        => attribute.ClassNameEquals("System.ComponentModel.DescriptionAttribute");

    /// <summary>
    /// 获取特性的描述值
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetDescription(this ImmutableArray<AttributeData> attributes)
    {
        var description = attributes.FirstOrDefault(IsDescription)?.
            ConstructorArguments.FirstOrDefault().Value;
        return description?.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
