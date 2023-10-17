namespace BD.Common8.Ipc.Server.Services;

/// <summary>
/// 用于定义以指定 prefix 为前缀的所有终结点。
/// <para>Ipc 服务接口必须继承此接口，且调用 <see cref="IpcServiceCollectionServiceExtensions.AddSingletonWithIpc{TService, TImplementation}(IServiceCollection)"/> 添加到 <see cref="Ioc"/></para>
/// </summary>
public partial interface IMapGroup
{
    /// <summary>
    /// 当注册路由终结点时，实现由源生成器提供。
    /// </summary>
    /// <param name="builder"></param>
    static abstract void OnMapGroup(IEndpointRouteBuilder builder);
}
