namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加后台管理系统
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="privateKey"></param>
    /// <param name="appSettings"></param>
    /// <param name="configureApplicationPartManager"></param>
    public static unsafe void AddBMS<TBMAppSettings, [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TContext>(
        this WebApplicationBuilder builder,
        byte[] privateKey,
        TBMAppSettings appSettings,
        delegate* managed<ApplicationPartManager, void> configureApplicationPartManager = default)
        where TBMAppSettings : BMAppSettings
        where TContext : ApplicationDbContextBase
    {
        MemoryPackFormatterProvider.Register(RSAParametersFormatterAttribute.Formatter.Default);
        ServerSecurity.RSA = Serializable.DMP2<RSAParameters>(privateKey).Create();

        builder.Services.AddSingleton<IOptions<BMAppSettings>>(static s => s.GetRequiredService<IOptions<TBMAppSettings>>());

        HashSet<Assembly> assembliesAutoMapper = new();
        assembliesAutoMapper.Add(typeof(SysMenuTreeItem).Assembly);
        assembliesAutoMapper.Add(typeof(SysMenu).Assembly);

        builder.Services.AddAutoMapper((serviceProvider, cfg) =>
        {
            cfg.AddCollectionMappers();
            cfg.AddProfile<SysMenuProfile>();
        }, assembliesAutoMapper.ToArray());

        builder.AddDbContext<ApplicationDbContextBase, TContext>();
        builder.Services.AddTenantIdentity<TContext>();
        builder.Services.AddHttpContextRequestAbortedProvider();
        builder.Services.AddBMRepositories<TContext>();
        builder.Services.AddBMIdentity<TBMAppSettings, TContext>(appSettings);
        builder.Services.AddBMWebApi(appSettings, configureApplicationPartManager);
    }
}