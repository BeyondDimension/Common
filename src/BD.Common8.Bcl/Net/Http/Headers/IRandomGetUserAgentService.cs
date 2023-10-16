namespace System.Net.Http.Headers;

/// <summary>
/// 随机获取 User-Agent 服务
/// </summary>
public interface IRandomGetUserAgentService
{
    /// <summary>
    /// 获取 User-Agent 字符串
    /// </summary>
    /// <returns></returns>
    string GetUserAgent();
}
