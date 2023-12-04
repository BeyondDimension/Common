using Microsoft.AspNetCore.SignalR.Client;

namespace BD.Common8.Ipc.Client.Services;

/// <summary>
/// Ipc 客户端连接服务
/// </summary>
public interface IIpcClientService : Serializable.IService
{
    /// <summary>
    /// 获取 HttpClient 实例
    /// </summary>
    HttpClient HttpClient { get; }

    /// <summary>
    /// 异步获取 SignalR Hub 连接
    /// </summary>
    /// <returns></returns>
    ValueTask<HubConnection> GetHubConnAsync();
}
