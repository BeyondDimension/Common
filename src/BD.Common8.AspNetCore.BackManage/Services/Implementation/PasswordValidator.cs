namespace BD.Common.Services.Implementation;

/// <summary>
/// Provides the default password policy for Identity.
/// </summary>
public class PasswordValidator : IPasswordValidator
{
    /// <summary>
    /// Constructions a new instance of <see cref="PasswordValidator{TUser}"/>.
    /// </summary>
    /// <param name="errors">The <see cref="IdentityErrorDescriber"/> to retrieve error text from.</param>
    public PasswordValidator(IdentityErrorDescriber? errors = null)
    {
        Describer = errors ?? new IdentityErrorDescriber();
    }

    /// <summary>
    /// Gets the <see cref="IdentityErrorDescriber"/> used to supply error text.
    /// </summary>
    /// <value>The <see cref="IdentityErrorDescriber"/> used to supply error text.</value>
    public IdentityErrorDescriber Describer { get; private set; }

    /// <summary>
    /// Validates a password as an asynchronous operation.
    /// </summary>
    /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve the <paramref name="user"/> properties from.</param>
    /// <param name="user">The user whose password should be validated.</param>
    /// <param name="password">The password supplied for validation</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual ValueTask<IdentityResult> ValidateAsync(IUserManager manager, SysUser user, string? password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }
        if (manager == null)
        {
            throw new ArgumentNullException(nameof(manager));
        }
        List<IdentityError>? errors = null;
        var options = manager.Options.Password;
        if (string.IsNullOrWhiteSpace(password) || password.Length < options.RequiredLength)
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.PasswordTooShort(options.RequiredLength));
        }
        if (options.RequireNonAlphanumeric && password.All(IsLetterOrDigit))
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.PasswordRequiresNonAlphanumeric());
        }
        if (options.RequireDigit && !password.Any(IsDigit))
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.PasswordRequiresDigit());
        }
        if (options.RequireLowercase && !password.Any(IsLower))
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.PasswordRequiresLower());
        }
        if (options.RequireUppercase && !password.Any(IsUpper))
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.PasswordRequiresUpper());
        }
        if (options.RequiredUniqueChars >= 1 && password.Distinct().Count() < options.RequiredUniqueChars)
        {
            errors ??= new List<IdentityError>();
            errors.Add(Describer.PasswordRequiresUniqueChars(options.RequiredUniqueChars));
        }
        return new(errors?.Count > 0
                ? IdentityResult.Failed(errors!.ToArray())
                : IdentityResult.Success);
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is a digit.
    /// </summary>
    /// <param name="c">The character to check if it is a digit.</param>
    /// <returns>True if the character is a digit, otherwise false.</returns>
    public virtual bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is a lower case ASCII letter.
    /// </summary>
    /// <param name="c">The character to check if it is a lower case ASCII letter.</param>
    /// <returns>True if the character is a lower case ASCII letter, otherwise false.</returns>
    public virtual bool IsLower(char c)
    {
        return c >= 'a' && c <= 'z';
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is an upper case ASCII letter.
    /// </summary>
    /// <param name="c">The character to check if it is an upper case ASCII letter.</param>
    /// <returns>True if the character is an upper case ASCII letter, otherwise false.</returns>
    public virtual bool IsUpper(char c)
    {
        return c >= 'A' && c <= 'Z';
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is an ASCII letter or digit.
    /// </summary>
    /// <param name="c">The character to check if it is an ASCII letter or digit.</param>
    /// <returns>True if the character is an ASCII letter or digit, otherwise false.</returns>
    public virtual bool IsLetterOrDigit(char c)
    {
        return IsUpper(c) || IsLower(c) || IsDigit(c);
    }
}
