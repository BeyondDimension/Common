namespace BD.Common.Repositories.SourceGenerator.Helpers;

static class AttributeDataHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetClassFullName(this AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ClassNameEquals(this AttributeData attribute, string attributeClassFullName)
        => attribute.GetClassFullName() == attributeClassFullName;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDescription(AttributeData attribute)
        => attribute.ClassNameEquals(TypeFullNames.Description);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetDescription(this ImmutableArray<AttributeData> attributes)
    {
        var description = attributes.FirstOrDefault(IsDescription)?.
            ConstructorArguments.FirstOrDefault().Value;
        return description?.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static object? GetObjectValue(this TypedConstant typedConstant)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BackManageFieldAttribute? GetBackManageFieldAttribute(this ImmutableArray<AttributeData> attributes)
    {
        const string typeFullName = "BD.Common.Repositories.SourceGenerator.Annotations.BackManageFieldAttribute";
        var attribute = attributes.FirstOrDefault(static x => x.ClassNameEquals(typeFullName));
        if (attribute == null) return null;

        BackManageFieldAttribute attr = new();
        foreach (var item in attribute.NamedArguments)
            attr.SetValue(item.Key, item.Value.GetObjectValue());
        return attr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GenerateRepositoriesAttribute? GetGenerateRepositoriesAttribute(this ImmutableArray<AttributeData> attributes)
    {
        const string typeFullName = "BD.Common.Repositories.SourceGenerator.Annotations.GenerateRepositoriesAttribute";
        var attribute = attributes.FirstOrDefault(static x => x.ClassNameEquals(typeFullName));
        if (attribute == null) return null;

        GenerateRepositoriesAttribute attr = new();
        foreach (var item in attribute.NamedArguments)
            attr.SetValue(item.Key, item.Value.GetObjectValue());
        return attr;
    }
}
