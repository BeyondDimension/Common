namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    static async Task ExceptionHandler(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = MediaTypeNames.JSON;
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var rsp = new ApiResponse
        {
            Messages = [exception?.ToString() ?? ""],
        };
        await context.Response.WriteAsJsonAsync(rsp);
    }

    /// <summary>
    /// 使用后台管理系统
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <param name="app"></param>
    /// <param name="isDevelopment"></param>
    /// <param name="appSettings"></param>
    /// <param name="useRouteRoot">是否注册根路由使用一些默认行为，开启 ApiDoc 时将跳转 /swagger/index.html，否则显示 WelcomePage</param>
    /// <param name="useDeveloperExceptionPage">是否使用开发人员异常页 https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#developer-exception-page</param>
    /// <param name="useSwagger"></param>
    /// <param name="useExceptionHandler"></param>
    public static unsafe void UseBMS<TBMAppSettings>(
        this WebApplication app,
        bool isDevelopment,
        TBMAppSettings appSettings,
        bool useRouteRoot = true,
        bool useDeveloperExceptionPage = false,
        delegate* managed<WebApplication, void> useSwagger = default,
        bool useExceptionHandler = true)
        where TBMAppSettings : BMAppSettings
    {
        // 01. 启用异常处理
        if (useExceptionHandler)
        {
            if (useDeveloperExceptionPage)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(exceptionHandlerApp => // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/error-handling
                {
                    exceptionHandlerApp.Run(ExceptionHandler);
                });
            }
        }

        // 02. 启用响应内容压缩 gzip/br/zstd
        if (!isDevelopment)
        {
            app.UseResponseCompression();
        }

        // 03. 启用修复反向代理导致请求方 IP 地址不正确的问题
        app.UseForwardedHeaders(appSettings);

        // 04. 启用 API 文档
        if (!appSettings.DisabledApiDoc)
        {
            if (useSwagger != default)
            {
                useSwagger(app);
            }
            if (useRouteRoot)
            {
                app.MapGet("/", static () => Results.Redirect("/swagger/index.html"));
            }
        }
        else
        {
            if (useRouteRoot)
            {
                app.UseWelcomePage("/");
            }
        }

        // 05. 启用跨域
        if (appSettings.UseCors)
        {
            app.UseCors();
        }

        // 06. 鉴权，检测有没有登录，登录的是谁，赋值给 User
        app.UseAuthentication();

        // 07. 授权，检测权限
        app.UseAuthorization();

        // 08. 启用控制器
        app.MapControllers();
    }
}