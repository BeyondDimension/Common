namespace BD.Common8.Primitives.Columns;

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