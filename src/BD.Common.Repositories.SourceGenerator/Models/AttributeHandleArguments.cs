namespace BD.Common.Repositories.SourceGenerator.Models;

public readonly record struct AttributeHandleArguments(
    ClassType ClassType,
    Stream Stream,
    AttributeData Attribute,
    string AttributeClassFullName,
    PropertyMetadata Metadata)
{
}
