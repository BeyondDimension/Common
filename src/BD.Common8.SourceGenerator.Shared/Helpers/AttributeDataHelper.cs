namespace BD.Common8.SourceGenerator.Helpers;

#pragma warning disable SA1600 // Elements should be documented

public static class AttributeDataHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetClassFullName(this AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ClassNameEquals(this AttributeData attribute, string attributeClassFullName)
        => attribute.GetClassFullName() == attributeClassFullName;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDescription(AttributeData attribute)
        => attribute.ClassNameEquals("System.ComponentModel.DescriptionAttribute");

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
