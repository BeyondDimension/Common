namespace BD.Common8.Http.ClientFactory.Server.Services.Implementation;

/// <summary>
/// 客户端 <see cref="HttpClient"/> 工厂
/// </summary>
sealed partial class ClientHttpClientFactory(IHttpClientFactory factory) : IClientHttpClientFactory
{
    readonly IHttpClientFactory factory = factory;

    /// <inheritdoc/>
    HttpClient IClientHttpClientFactory.CreateClient(string name, HttpHandlerCategory category)
    {
        var client = factory.CreateClient(name);
        return client;
    }

    /// <summary>
    /// 将 <see cref="ClientHttpClientFactory"/> 注册到依赖注入
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IServiceCollection AddClientHttpClientFactory(IServiceCollection services)
    {
        IClientHttpClientFactory.AddHttpClientDelegateValue = AddHttpClientByReflection;
        return services.AddSingleton<IClientHttpClientFactory, ClientHttpClientFactory>();
    }

    /// <summary>
    /// 通过反射将 <see cref="HttpClient"/> 添加到服务集合中
    /// </summary>
    static void AddHttpClientByReflection(IServiceCollection services, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
#pragma warning disable IL2060 // Call to 'System.Reflection.MethodInfo.MakeGenericMethod' can not be statically analyzed. It's not possible to guarantee the availability of requirements of the generic method.
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        => _AddHttpClient.Value.MakeGenericMethod(type).Invoke(null, [services]);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2060 // Call to 'System.Reflection.MethodInfo.MakeGenericMethod' can not be statically analyzed. It's not possible to guarantee the availability of requirements of the generic method.

    /// <summary>
    /// 存储了延迟加载的 AddHttpClient
    /// </summary>
    static readonly Lazy<MethodInfo> _AddHttpClient = new(() =>
        {
            var methodAddHttpClient = typeof(ClientHttpClientFactory).GetMethod(nameof(AddHttpClient), BindingFlags.Static | BindingFlags.NonPublic);
            return methodAddHttpClient.ThrowIsNull();
        });

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void AddHttpClient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TClient>(IServiceCollection services) where TClient : class
        => HttpClientFactoryServiceCollectionExtensions.AddHttpClient<TClient>(services);
}
