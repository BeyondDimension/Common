namespace BD.Common8.Ipc.Services;

/// <summary>
/// Ipc 服务端后台服务
/// </summary>
public interface IIpcServerService
{
    /// <summary>
    /// 启动服务，如果服务已启动则跳过
    /// </summary>
    ValueTask RunAsync();

    /// <summary>
    /// 获取用于客户端的连接字符串
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IpcAppConnectionString GetConnectionString(IpcAppConnectionStringType? type = null);
}
