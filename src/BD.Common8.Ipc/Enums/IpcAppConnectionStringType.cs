namespace BD.Common8.Ipc.Enums;

/// <summary>
/// Ipc 应用程序连接字符串类型
/// </summary>
public enum IpcAppConnectionStringType : byte
{
    Https,
    UnixSocket,
    NamedPipe,
}
