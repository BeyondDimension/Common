namespace BD.Common8.SourceGenerator.Repositories.Extensions;

static partial class AttributeDataExtensions
{
    public static BackManageFieldAttribute? GetBackManageFieldAttribute(
        this ImmutableArray<AttributeData> attributes)
    {
        const string typeFullName = "BD.Common.Repositories.SourceGenerator.Annotations.BackManageFieldAttribute";
        const string typeFullName2 = "BD.Common8.SourceGenerator.Repositories.Models.BackManageFieldAttribute";
        var attribute = attributes.FirstOrDefault(static x => x.ClassNameEquals(typeFullName) || x.ClassNameEquals(typeFullName2));
        if (attribute == null) return null;

        BackManageFieldAttribute attr = new();
        foreach (var item in attribute.NamedArguments)
            attr.SetValue(item.Key, item.Value.GetObjectValue());
        return attr;
    }

    public static GenerateRepositoriesAttribute? GetGenerateRepositoriesAttribute(
        this ImmutableArray<AttributeData> attributes)
    {
        const string typeFullName = "BD.Common.Repositories.SourceGenerator.Annotations.GenerateRepositoriesAttribute";
        const string typeFullName2 = "BD.Common8.SourceGenerator.Repositories.Models.GenerateRepositoriesAttribute";
        var attribute = attributes.FirstOrDefault(static x => x.ClassNameEquals(typeFullName) || x.ClassNameEquals(typeFullName2));
        if (attribute == null) return null;

        GenerateRepositoriesAttribute attr = new();
        foreach (var item in attribute.NamedArguments)
            attr.SetValue(item.Key, item.Value.GetObjectValue());
        return attr;
    }
}