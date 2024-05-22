namespace BD.Common8.SourceGenerator.Repositories.Models;

public readonly record struct PropertyHandleArguments(
    Stream Stream,
    string? Arguments,
    PropertyMetadata Metadata)
{
}
