namespace BD.Common8.Ipc.Services;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 客户端连接服务
/// </summary>
public interface IIpcClientService : IAsyncDisposable
{
    /// <summary>
    /// 以异步操作发送 HTTP 请求，仅响应正文
    /// <para>响应正文泛型支持</para>
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponseBody?> SendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull;

    /// <summary>
    /// 以异步操作发送 HTTP 请求，响应正文与请求正文
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponseBody?> SendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TResponseBody : notnull
        where TRequestBody : notnull;

    /// <summary>
    /// 以异步操作发送 HTTP 请求，仅响应正文，返回异步迭代器
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<TResponseBody?> SendAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(WebApiClientSendArgs args, CancellationToken cancellationToken = default) where TResponseBody : notnull;

    /// <summary>
    /// 以异步操作发送 HTTP 请求，响应正文与请求正文，返回异步迭代器
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <typeparam name="TRequestBody"></typeparam>
    /// <param name="args"></param>
    /// <param name="requestBody"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<TResponseBody?> SendAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRequestBody>(WebApiClientSendArgs args, TRequestBody requestBody, CancellationToken cancellationToken = default)
        where TResponseBody : notnull
        where TRequestBody : notnull;

    /// <summary>
    /// 以异步操作发送到 Hub 的请求
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="hubUrl"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponseBody?> HubSendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        string? hubUrl,
        string methodName,
        object?[]? args = null,
        CancellationToken cancellationToken = default)
        where TResponseBody : notnull;

    /// <summary>
    /// 以异步操作发送到 Hub 的请求，返回异步迭代器
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="hubUrl"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<TResponseBody?> HubSendAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        string? hubUrl,
        string methodName,
        object?[]? args = null,
        CancellationToken cancellationToken = default)
        where TResponseBody : notnull;
}
