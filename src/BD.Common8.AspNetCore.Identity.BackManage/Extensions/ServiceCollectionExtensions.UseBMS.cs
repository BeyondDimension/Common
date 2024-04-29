using Microsoft.AspNetCore.Diagnostics;

namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 使用后台管理系统
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <param name="app"></param>
    /// <param name="appSettings"></param>
    /// <param name="useRouteRoot">是否注册根路由使用一些默认行为，开启 ApiDoc 时将跳转 /swagger/index.html，否则显示 WelcomePage</param>
    /// <param name="useDeveloperExceptionPage">是否使用开发人员异常页 https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#developer-exception-page</param>
    public static void UseBMS<TBMAppSettings>(
        this WebApplication app,
        TBMAppSettings appSettings,
        bool useRouteRoot = true,
        bool useDeveloperExceptionPage = false)
        where TBMAppSettings : BMAppSettings
    {
        var isDevelopment = app.Environment.IsDevelopment();

        if (useDeveloperExceptionPage)
        {
            app.UseDeveloperExceptionPage();
        }
        else
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
        }

        if (!isDevelopment)
        {
            app.UseResponseCompression();
        }

        app.UseForwardedHeaders(appSettings);

        if (!appSettings.DisabledApiDoc)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            if (useRouteRoot)
            {
                app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
            }
        }
        else
        {
            if (useRouteRoot)
            {
                app.UseWelcomePage("/");
            }
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