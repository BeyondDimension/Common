namespace BD.Common8.Ipc.Attributes;

/// <summary>
/// 指示接口在进程间通信 (IPC) 应用程序中定义服务协定。
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public sealed class ServiceContractAttribute : Attribute
{
}
