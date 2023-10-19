namespace BD.Common8.Primitives.PersonalData.PhoneNumber.Columns;

/// <inheritdoc cref="IPhoneNumber"/>
public interface IReadOnlyPhoneNumber
{
    /// <inheritdoc cref="IPhoneNumber"/>
    string? PhoneNumber { get; }
}