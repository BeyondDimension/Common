using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.Regexps;

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
    public static bool IsEmail(this string str) => Regex.IsMatch(str, Email);
}
