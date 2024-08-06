namespace BD.Common8.Ipc.Enums;

/// <summary>
/// Ipc 应用程序连接字符串类型
/// </summary>
public enum IpcAppConnectionStringType : byte
{
    /// <summary>
    /// TCP/HTTP 协议，常见的 Web 技术，便于从网页以及跨语言编程中调用，需要监听一个 TCP 端口
    /// </summary>
    Https,

    /// <summary>
    /// UnixSocket，简称 UDS，需要一个文件，文件路径字符串长度也有限制，路径不能太长，使用文件流来通信
    /// </summary>
    UnixSocket,

    /// <summary>
    /// NamedPipe 命名管道，Windows 原生支持的管道通信，Linux 上使用 Unix 域套接字 (UDS) 来实现，需要一个唯一的字符串作为管道名，Windows 上涉及管理员权限进程交互需要额外配置
    /// </summary>
    NamedPipe,
}
