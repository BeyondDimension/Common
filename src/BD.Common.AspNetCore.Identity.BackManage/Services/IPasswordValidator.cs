namespace BD.Common.Services;

/// <summary>
/// Provides an abstraction for validating passwords.
/// </summary>
public interface IPasswordValidator
{
    /// <summary>
    /// Validates a password as an asynchronous operation.
    /// </summary>
    /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve the <paramref name="user"/> properties from.</param>
    /// <param name="user">The user whose password should be validated.</param>
    /// <param name="password">The password supplied for validation</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    ValueTask<IdentityResult> ValidateAsync(IUserManager manager, SysUser user, string? password);
}
