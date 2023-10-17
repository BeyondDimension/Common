namespace BD.Common8.Ipc.Attributes;

/// <summary>
/// 指示方法定义一个操作，该操作是进程间通信 (IPC) 应用程序中服务协定的一部分。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OperationContractAttribute : Attribute
{
    /// <summary>
    /// 获取或设置一个值，该值指定源生成器是否忽略标注的方法。
    /// </summary>
    public bool Ignore { get; set; }
}