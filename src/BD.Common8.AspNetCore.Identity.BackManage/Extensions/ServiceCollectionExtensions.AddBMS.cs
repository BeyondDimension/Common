using static BD.Common8.AspNetCore.Extensions.AddDbContext_ServiceCollectionExtensions;

namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加后台管理系统
    /// <list type="number">
    /// <item>配置服务器安全密钥</item>
    /// <item>添加 <see cref="BMAppSettings"/> 的 <see cref="IOptions{TOptions}"/> 服务</item>
    /// <item>配置 AutoMapper</item>
    /// <item>AddDbContext</item>
    /// <item>AddTenantIdentity</item>
    /// <item>AddHttpContextRequestAbortedProvider</item>
    /// <item>AddBMRepositories</item>
    /// <item>AddBMIdentity</item>
    /// <item>AddBMWebApi</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="privateKey"></param>
    /// <param name="appSettings"></param>
    /// <param name="configureApplicationPartManager"></param>
    /// <param name="addDbContext"></param>
    public static unsafe void AddBMS<TBMAppSettings, [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TContext>(
        this WebApplicationBuilder builder,
        byte[] privateKey,
        TBMAppSettings appSettings,
        delegate* managed<ApplicationPartManager, void> configureApplicationPartManager = default,
        bool addDbContext = DefaultValue_addDbContext)
        where TBMAppSettings : BMAppSettings
        where TContext : BMDbContextBase
    {
        #region 配置服务器安全密钥

        MemoryPackFormatterProvider.Register(RSAParametersFormatterAttribute.Formatter.Default);
        ServerSecurity.RSA = Serializable.DMP2<RSAParameters>(privateKey).Create();

        #endregion

        #region 添加 BMAppSettings 的 IOptions<TOptions> 服务

        builder.Services.AddSingleton<IOptions<BMAppSettings>>(static s => s.GetRequiredService<IOptions<TBMAppSettings>>());

        #endregion

        #region 配置 AutoMapper

        HashSet<Assembly> assembliesAutoMapper = new();
        assembliesAutoMapper.Add(typeof(BMMenuTreeItem).Assembly);
        assembliesAutoMapper.Add(typeof(BMMenu).Assembly);

        builder.Services.AddAutoMapper((serviceProvider, cfg) =>
        {
            cfg.AddCollectionMappers();
            cfg.AddProfile<BMMenuProfile>();
        }, assembliesAutoMapper.ToArray());

        #endregion

        builder.AddDbContext<BMDbContextBase, TContext>(addDbContext: addDbContext);
        builder.Services.AddTenantIdentity<TContext>();
        builder.Services.AddHttpContextRequestAbortedProvider();
        builder.Services.AddBMRepositories<TContext>();
        builder.Services.AddBMIdentity<TBMAppSettings, TContext>(appSettings);
        builder.Services.AddBMWebApi(appSettings, configureApplicationPartManager);
    }
}