// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// https://learn.microsoft.com/zh-cn/dotnet/api/system.runtime.compilerservices.tupleelementnamesattribute
// https://github.com/dotnet/roslyn/blob/Visual-Studio-2022-Version-17.8-Preview-1/src/Compilers/Test/Resources/Core/NetFX/ValueTuple/TupleElementNamesAttribute.cs

#if NETFRAMEWORK && !NET47_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Indicates that the use of <see cref="System.ValueTuple"/> on a member is meant to be treated as a tuple with element names.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct)]
sealed class TupleElementNamesAttribute : Attribute
{
    readonly string[] _transformNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="TupleElementNamesAttribute"/> class.
    /// </summary>
    /// <param name="transformNames">
    /// Specifies, in a prefix traversal of a type's
    /// construction, which <see cref="System.ValueType"/> occurrences are meant to
    /// carry element names.
    /// </param>
    /// <remarks>
    /// This constructor is meant to be used on types that are built on an underlying
    /// occurrence of <see cref="System.ValueType"/> that is meant to carry element names.
    /// For instance, if <c>C</c> is a generic type with two type parameters, then a
    /// use of the constructed type <c>C{<see cref="System.ValueTuple{T1, T2}"/>, <see cref="System.ValueTuple{T1, T2, T3}"/></c>
    /// might be intended to treat the first type argument as a tuple with element names
    /// and the second as a tuple without element names. In which case, the appropriate attribute
    /// specification should use <c>transformNames</c> value of <c>{ "name1", "name2", null }</c>.
    /// </remarks>
    public TupleElementNamesAttribute(string[] transformNames)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(transformNames);
#else
        _transformNames = transformNames ?? throw new ArgumentNullException(nameof(transformNames));
#endif

        _transformNames = transformNames;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TupleElementNamesAttribute"/> class.
    /// </summary>
    /// <remarks>
    /// When <see cref="TupleElementNamesAttribute"/> is created with this constructor,
    /// it can be omitted instead.
    /// </remarks>
    public TupleElementNamesAttribute()
    {
        _transformNames = [];
    }

    /// <summary>
    /// Specifies, in a prefix traversal of a type's
    /// construction, which <see cref="System.ValueTuple"/> occurrences are meant to
    /// carry element names.
    /// </summary>
    public IList<string> TransformNames => _transformNames;
}
#endif