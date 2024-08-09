namespace BD.Common8.Ipc.Services;

/// <summary>
/// Ipc 服务端后台服务
/// </summary>
public partial interface IIpcServerService
{
    /// <summary>
    /// 启动服务，如果服务已启动则跳过
    /// </summary>
    /// <returns></returns>
    ValueTask RunAsync();
}
