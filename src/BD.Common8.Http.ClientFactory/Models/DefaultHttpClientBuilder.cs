namespace BD.Common8.Http.ClientFactory.Models;

/// <summary>
/// 默认的 <see cref="HttpClient"/> 构建器类
/// </summary>
record class DefaultHttpClientBuilder(
    string Name,
    IServiceCollection Services,
    Action<HttpClient>? ConfigureClient) : IFusilladeHttpClientBuilder
{
    /// <summary>
    /// 获取或设置配置处理程序的委托
    /// </summary>
    public Func<Func<HttpMessageHandler>, HttpMessageHandler>? ConfigureHandler { get; set; }
}