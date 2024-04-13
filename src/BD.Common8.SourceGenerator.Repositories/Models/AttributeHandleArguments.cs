namespace BD.Common8.SourceGenerator.Repositories.Models;

public readonly record struct AttributeHandleArguments(
    ClassType ClassType,
    Stream Stream,
    AttributeData Attribute,
    string AttributeClassFullName,
    PropertyMetadata Metadata)
{
}
