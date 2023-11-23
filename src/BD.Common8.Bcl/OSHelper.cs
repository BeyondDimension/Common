namespace System;

/// <summary>
/// 操作系统相关助手类
/// </summary>
public abstract partial class OSHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OSHelper"/> class.
    /// </summary>
    protected OSHelper() => throw new NotSupportedException();

    /// <summary>
    /// 指示当前应用程序是否发布到应用商店中。
    /// </summary>
    public static bool IsPublishToStore { get; protected set; }
}
