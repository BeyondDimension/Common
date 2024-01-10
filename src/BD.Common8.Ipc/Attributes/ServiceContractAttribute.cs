namespace BD.Common8.Ipc.Attributes;

/// <summary>
/// 指示类在进程间通信 (IPC) 应用程序中定义服务协定。
/// </summary>
/// <param name="serviceType"></param>
/// <param name="generatorType"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ServiceContractImplAttribute(Type serviceType, IpcGeneratorType generatorType) : Attribute
{
    /// <summary>
    /// 需要根据服务接口类型生成
    /// </summary>
    public Type ServiceType { get; } = serviceType;

    /// <inheritdoc cref="IpcGeneratorType"/>
    public IpcGeneratorType GeneratorType { get; } = generatorType;

    /// <summary>
    /// <see cref="HubTypeFullName"/> 的默认值
    /// </summary>
    public const string DefaultHubTypeFullName = "BD.Common8.SourceGenerator.Ipc.Server.IpcHub";

    /// <summary>
    /// 自定义生成的服务端 Hub 类型命名空间与类型名
    /// </summary>
    public string HubTypeFullName { get; set; } = DefaultHubTypeFullName;

    /// <summary>
    /// 自定义生成的客户端调用 Hub 地址
    /// </summary>
    public string? HubUrl { get; set; } = null;
}
