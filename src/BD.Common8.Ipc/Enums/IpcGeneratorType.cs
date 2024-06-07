namespace BD.Common8.Ipc.Enums;

/// <summary>
/// Ipc 生成的实现类型
/// </summary>
public enum IpcGeneratorType : byte
{
    /// <summary>
    /// 生成使用 WebApi 的客户端调用实现
    /// </summary>
    //[Obsolete("use Grpc")]
    ClientWebApi = 1,

    /// <summary>
    /// 生成使用 SignalR 的客户端调用实现
    /// </summary>
    //[Obsolete("use Grpc")]
    ClientSignalR = 2,

    /// <summary>
    /// 生成服务端实现
    /// </summary>
    Server = 3,

    /// <summary>
    /// 生成使用 WebApi 聚合服务的客户端调用实现，通过源生成代码 route 中 name 与 body switch 调用服务函数
    /// </summary>
    [Obsolete("not implemented")]
    ClientWebApiAggregation = 4,

    /// <summary>
    /// Grpc
    /// </summary>
    Grpc = 5,
}
