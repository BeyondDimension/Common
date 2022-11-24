using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class StringExtensions
{
    // https://github.com/CommunityToolkit/dotnet/blob/v8.0.0/CommunityToolkit.Common/Extensions/StringExtensions.cs#L58

    /// <summary>
    /// Determines whether a string is a valid email address.
    /// </summary>
    /// <param name="str">The string to test.</param>
    /// <returns><c>true</c> for a valid email address; otherwise, <c>false</c>.</returns>
    public static bool IsEmail(this string str) => EmailRegex().IsMatch(str);

    /// <summary>
    /// Regular expression for matching an email address.
    /// </summary>
    /// <remarks>General Email Regex (RFC 5322 Official Standard) from https://emailregex.com.</remarks>
    /// <returns></returns>
    [GeneratedRegex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])")]
    private static partial Regex EmailRegex();
}
