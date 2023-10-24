// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Types/Shared/Exceptions.shared.cs
namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// Exception that occurs when calling an API that requires a specific permission.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PermissionException"/> class with the specified message.
/// </remarks>
/// <param name="message">A message that describes this exception in more detail.</param>
public sealed class PermissionException(string message) : UnauthorizedAccessException(message)
{
}