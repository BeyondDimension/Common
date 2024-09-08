using Microsoft.Extensions.WebEncoders;
using static BD.Common8.AspNetCore.Extensions.AddDbContext_ServiceCollectionExtensions;

namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加后台管理系统配置
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="privateKey"></param>
    /// <param name="appSettings"></param>
    /// <param name="configureApplicationPartManager"></param>
    /// <param name="o">配置 DbContextOptionsBuilder 的可选操作</param>
    /// <param name="databaseProvider">数据库提供程序名称</param>
    /// <param name="connectionStringKeyName">连接字符串键的名称，为 <see langword="null"/> 时不调用 GetConnectionString 与 AddDbContext</param>
    public static unsafe void AddBMS<TBMAppSettings, [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TContext>(
        this WebApplicationBuilder builder,
        byte[] privateKey,
        TBMAppSettings appSettings,
        delegate* managed<ApplicationPartManager, void> configureApplicationPartManager = default,
        Action<DbContextOptionsBuilder>? o = null,
        string databaseProvider = DefaultValue_databaseProvider,
        string? connectionStringKeyName = DefaultValue_connectionStringKeyName)
        where TBMAppSettings : BMAppSettings
        where TContext : BMDbContextBase
    {
        #region 1. 启用 Https 时的响应压缩

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        #endregion

        #region 2. 设置禁止 Unicode 编码（默认将中文编码为 \\uxxxx 之类的）

        builder.Services.Configure<WebEncoderOptions>(options =>
        {
            options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
        });

        #endregion

        #region 3. 配置 HttpContext.RequestAborted 实现的请求取消提供程序

        builder.Services.AddHttpContextRequestAbortedProvider();

        #endregion

        #region 4. 添加路由与控制器

        builder.Services.AddRouting();
        builder.Services.AddControllers(opt =>
        {
            opt.Filters.Add<UserIsLockedOutFilterAttribute>();
        }).ConfigureApplicationPartManager(apm =>
        {
            apm.ApplicationParts.Add(
                new AssemblyPart(typeof(BMLoginController).GetTypeInfo().Assembly));
            if (configureApplicationPartManager != default)
                configureApplicationPartManager(apm);
        });

        #endregion

        #region 5. 配置 AutoMapper

        HashSet<Assembly> assembliesAutoMapper = new();
        assembliesAutoMapper.Add(typeof(BMMenuTreeItem).Assembly);
        assembliesAutoMapper.Add(typeof(BMMenu).Assembly);

        builder.Services.AddAutoMapper((serviceProvider, cfg) =>
        {
            cfg.AddCollectionMappers();
            cfg.AddProfile<BMMenuProfile>();
        }, assembliesAutoMapper.ToArray());

        #endregion

        #region 6. 配置模型验证失败时响应格式

        builder.Services.ConfigureApiBehaviorOptionsInvalidModelStateResponseFactory();

        #endregion

        #region 7. 配置基类的数据库上下文服务

        builder.AddDbContext<BMDbContextBase, TContext>(o, databaseProvider, connectionStringKeyName);

        #endregion

        #region 8. 配置允许跨域访问

        builder.Services.AddCorsByViewUrl(appSettings);

        #endregion

        #region 9. 配置服务器安全密钥

        MemoryPackFormatterProvider.Register(RSAParametersFormatterAttribute.Formatter.Default);
        ServerSecurity.RSA = Serializable.DMP2<RSAParameters>(privateKey).Create();

        #endregion

        #region 10. 添加 BMAppSettings 基类的 IOptions<TOptions> 服务

        builder.Services.AddSingleton<IOptions<BMAppSettings>>(static s => s.GetRequiredService<IOptions<TBMAppSettings>>());

        #endregion

        #region 11. 配置租户身份服务

        builder.Services.AddTenantIdentity<TContext>();

        #endregion

        #region 12. 配置后台管理系统的仓储服务

        builder.Services.AddBMRepositories<TContext>();

        #endregion

        #region 13. 配置后台管理系统的身份服务

        builder.Services.AddBMIdentity<TBMAppSettings, TContext>(appSettings);

        #endregion
    }
}