#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
using BD.Common8.Http.ClientFactory.Server.Services.Implementation;
using System.Net.Http.Client;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加使用 <see cref="IHttpClientFactory"/> 实现的 <see cref="IClientHttpClientFactory"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddClientHttpClientFactory(
        this IServiceCollection services)
        => ClientHttpClientFactory.AddClientHttpClientFactory(services);
}
