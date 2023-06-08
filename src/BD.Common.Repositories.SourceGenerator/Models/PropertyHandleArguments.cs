namespace BD.Common.Repositories.SourceGenerator.Models;

public readonly record struct PropertyHandleArguments(
    Stream Stream,
    ImmutableArray<AttributeData> Attributes,
    PropertyMetadata Metadata)
{
}
