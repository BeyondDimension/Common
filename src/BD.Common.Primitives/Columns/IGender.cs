// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

/// <inheritdoc cref="Enums.Gender"/>
public interface IGender
{
    /// <inheritdoc cref="Enums.Gender"/>
    Gender Gender { get; set; }
}

/// <inheritdoc cref="Enums.Gender"/>
public interface IReadOnlyGender
{
    /// <inheritdoc cref="Enums.Gender"/>
    Gender Gender { get; }
}