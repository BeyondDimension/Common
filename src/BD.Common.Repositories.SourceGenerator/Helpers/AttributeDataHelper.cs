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
}
