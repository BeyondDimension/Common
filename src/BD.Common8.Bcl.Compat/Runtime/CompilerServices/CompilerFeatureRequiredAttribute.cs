// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://learn.microsoft.com/zh-cn/dotnet/api/system.runtime.compilerservices.compilerfeaturerequiredattribute
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/CompilerFeatureRequiredAttribute.cs

#if NETFRAMEWORK || NETSTANDARD || !NET7_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// 指示应用此属性的位置需要对特定功能的编译器支持。
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CompilerFeatureRequiredAttribute"/> class.
/// </remarks>
/// <param name="featureName"></param>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
{
    /// <summary>
    /// The name of the compiler feature.
    /// </summary>
    public string FeatureName { get; } = featureName;

    /// <summary>
    /// If true, the compiler can choose to allow access to the location where this attribute is applied if it does not understand <see cref="FeatureName"/>.
    /// </summary>
    public bool IsOptional { get; init; }

    /// <summary>
    /// The <see cref="FeatureName"/> used for the ref structs C# feature.
    /// </summary>
    public const string RefStructs = nameof(RefStructs);

    /// <summary>
    /// The <see cref="FeatureName"/> used for the required members C# feature.
    /// </summary>
    public const string RequiredMembers = nameof(RequiredMembers);
}
#endif