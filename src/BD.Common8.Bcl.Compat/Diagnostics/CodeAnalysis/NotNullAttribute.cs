// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETFRAMEWORK || NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
///     Specifies that an output is not <see langword="null"/> even if the
///     corresponding type allows it.
/// </summary>
[AttributeUsage(
       AttributeTargets.Field | AttributeTargets.Parameter |
       AttributeTargets.Property | AttributeTargets.ReturnValue,
       Inherited = false
   )]
sealed class NotNullAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullAttribute"/> class.
    /// </summary>
    public NotNullAttribute() { }
}
#endif