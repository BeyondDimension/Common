#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加由 Repository 实现的 <see cref="ISecureStorage"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddRepositorySecureStorage(this IServiceCollection services)
    {
        services.TryAddSingleton<ISecureStorage, RepositorySecureStorage>();
        return services;
    }
}