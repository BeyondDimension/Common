namespace BD.Common8.FeishuOApi.Sdk.Services.Abstractions;

/// <summary>
/// 飞书开放平台 WebApi 调用接口
/// </summary>
public interface IFeishuApiClient
{
    /// <summary>
    /// 发送文本消息
    /// </summary>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiRspImpl<HttpStatusCode>> SendMessageAsync(
        string? title,
        string? text,
        CancellationToken cancellationToken = default);
}
