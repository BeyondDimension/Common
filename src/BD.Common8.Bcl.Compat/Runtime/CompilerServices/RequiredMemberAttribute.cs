// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://learn.microsoft.com/zh-cn/dotnet/api/system.runtime.compilerservices.requiredmemberattribute
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/RequiredMemberAttribute.cs

#if NETFRAMEWORK || NETSTANDARD || !NET7_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// 指定类型具有必需成员或需要成员。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
sealed class RequiredMemberAttribute : Attribute
{ }
#endif