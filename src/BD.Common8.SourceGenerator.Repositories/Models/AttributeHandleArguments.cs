namespace BD.Common8.SourceGenerator.Repositories.Models;

public readonly record struct AttributeHandleArguments(
    ClassType ClassType,
    Stream Stream,
    string AttributeName,
    string? AttributeValue,
    PropertyMetadata Metadata)
{
}
