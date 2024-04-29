namespace BD.Common8.AspNetCore.Services;

/// <summary>
/// Provides an abstraction for validating passwords.
/// </summary>
public interface IPasswordValidator
{
    /// <summary>
    /// Validates a password as an asynchronous operation.
    /// </summary>
    /// <param name="manager">The <see cref="UserManagerImpl{TUser}"/> to retrieve the <paramref name="user"/> properties from.</param>
    /// <param name="user">The user whose password should be validated.</param>
    /// <param name="password">The password supplied for validation</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    ValueTask<IdentityResult> ValidateAsync(IUserManager manager, BMUser user, string? password);
}
