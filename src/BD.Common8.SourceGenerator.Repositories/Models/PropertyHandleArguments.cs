namespace BD.Common8.SourceGenerator.Repositories.Models;

public readonly record struct PropertyHandleArguments(
    Stream Stream,
    ImmutableArray<AttributeData> Attributes,
    PropertyMetadata Metadata)
{
}
