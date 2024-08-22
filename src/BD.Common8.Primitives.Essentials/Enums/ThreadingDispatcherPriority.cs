// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// https://github.com/dotnet/wpf/blob/v6.0.5/src/Microsoft.DotNet.Wpf/src/WindowsBase/System/Windows/Threading/DispatcherPriority.cs
// https://github.com/AvaloniaUI/Avalonia/blob/0.10.14/src/Avalonia.Base/Threading/DispatcherPriority.cs

namespace BD.Common8.Enums;

/// <summary>
/// 描述了操作可以通过 Dispatcher 调用
/// <para>https://github.com/dotnet/wpf/blob/master/src/Microsoft.DotNet.Wpf/src/WindowsBase/System/Windows/Threading/DispatcherPriority.cs</para>
/// </summary>
///
public enum ThreadingDispatcherPriority : sbyte
{
    /// <summary>
    ///      这是无效的优先级
    /// </summary>
    Invalid = -1,

    /// <summary>
    ///     不处理此优先级的操作
    /// </summary>
    Inactive = 0,

    /// <summary>
    ///     当系统空闲时，将处理此优先级的操作
    /// </summary>
    SystemIdle = 1,

    /// <summary>
    /// 最低可能优先级
    /// </summary>
    MinValue = 1,

    /// <summary>
    ///     当应用程序空闲时，将处理此优先级的操作
    /// </summary>
    ApplicationIdle,

    /// <summary>
    ///     上下文空闲时处理此优先级的操作
    /// </summary>
    ContextIdle,

    /// <summary>
    ///     此优先级的操作在完成所有其他非空闲操作之后进行处理
    /// </summary>
    Background,

    /// <summary>
    ///     此优先级的操作以与输入相同的优先级处理
    /// </summary>
    Input,

    /// <summary>
    ///     当布局和渲染完成，但刚好在服务输入优先级的项目之前。明确地在激发Loaded事件时使用
    /// </summary>
    Loaded,

    /// <summary>
    ///     同时处理此优先级的操作渲染时的优先级
    /// </summary>
    Render,

    /// <summary>
    ///     同时处理此优先级的操作优先级作为数据绑定
    /// </summary>
    [Obsolete("WPF compatibility")]
    DataBind,

    /// <summary>
    ///     此优先级的操作以正常优先级处理
    /// </summary>
    Normal,

    /// <summary>
    ///     此优先级的操作在其他操作之前处理异步操作
    /// </summary>
    Send,

    /// <summary>
    ///     最大可能优先级
    /// </summary>
    MaxValue = Send,
}