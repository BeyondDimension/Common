using Microsoft.AspNetCore.Diagnostics;

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

    /// <summary>
    /// 使用后台管理系统
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <param name="app"></param>
    /// <param name="appSettings"></param>
    public static void UseBMS<TBMAppSettings>(
        this WebApplication app,
        TBMAppSettings appSettings)
        where TBMAppSettings : BMAppSettings
    {
        app.UseExceptionHandler(exceptionHandlerApp => // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/error-handling
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.JSON;
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                var rsp = new ApiResponse
                {
                    Messages = [exception?.ToString() ?? ""],
                };
                await context.Response.WriteAsJsonAsync(rsp);
            });
        });

        app.UseResponseCompression();

        if (!appSettings.NotUseForwardedHeaders)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false,
                ForwardLimit = null,
                KnownProxies = { IPAddress.Parse(string.IsNullOrWhiteSpace(appSettings.ForwardedHeadersKnownProxies) ? "::ffff:172.18.0.1" : appSettings.ForwardedHeadersKnownProxies), },
            });
        }

        if (!appSettings.DisabledApiDoc)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
        }
        else
        {
            app.UseWelcomePage("/");
        }

        if (appSettings.UseCors)
        {
            app.UseCors();
        }

        app.UseAuthentication(); // 鉴权，检测有没有登录，登录的是谁，赋值给 User
        app.UseAuthorization(); // 授权，检测权限
        app.MapControllers();
    }
}