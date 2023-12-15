namespace BD.Common8.Ipc.Enums;

/// <summary>
/// Ipc 生成的实现类型
/// </summary>
public enum IpcGeneratorType : byte
{
    /// <summary>
    /// 生成使用 WebApi 的客户端调用实现
    /// </summary>
    ClientWebApi = 1,

    /// <summary>
    /// 生成使用 SignalR 的客户端调用实现
    /// </summary>
    ClientSignalR = 2,

    /// <summary>
    /// 生成服务端实现
    /// </summary>
    Server = 3,
}
