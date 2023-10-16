// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETFRAMEWORK || NETSTANDARD || !NETCOREAPP3_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// 指示参数将为另一个参数传递的表达式捕获为字符串。
/// </summary>
/// <remarks>
/// 初始化 <see cref="CallerArgumentExpressionAttribute"/> 类的新实例。
/// </remarks>
/// <param name="parameterName">其表达式应捕获为字符串的参数的名称。</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
{
    /// <summary>
    /// 获取其表达式应捕获为字符串的参数的名称。
    /// </summary>
    public string ParameterName { get; } = parameterName;
}
#endif