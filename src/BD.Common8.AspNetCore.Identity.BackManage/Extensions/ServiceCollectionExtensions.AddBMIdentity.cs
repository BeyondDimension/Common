namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    const string AuthenticationScheme = "Bearer";

    /// <summary>
    /// 添加后台管理系统的身份验证相关
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <typeparam name="TBMDbContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="appSettings"></param>
    public static void AddBMIdentity<TBMAppSettings, TBMDbContext>(this IServiceCollection services, TBMAppSettings appSettings)
        where TBMAppSettings : BMAppSettings
        where TBMDbContext : DbContext, IApplicationDbContext, IApplicationDbContext<SysUser>
    {
        services.Configure<IdentityOptions>(options =>
        {
            // 密码设置
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 10;
            options.Password.RequiredUniqueChars = 5;

            // 锁定设置
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(6);
            options.Lockout.MaxFailedAccessAttempts = 6;
            options.Lockout.AllowedForNewUsers = true;

            // 用户设置
            options.User.AllowedUserNameCharacters = null!;
            options.User.RequireUniqueEmail = false;

            // 登录设置
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = true;
        });

        var appSettingsSecretKey = appSettings.SecretKey;
        var signingKey = IJWTValueProvider.GetSecurityKey(appSettingsSecretKey.ThrowIsNull());

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler<TBMDbContext>>();
        services.Configure((Action<TBMAppSettings>)(options =>
        {
            options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha384Signature);
        }));

        services.AddScoped<IJWTValueProvider, JWTValueProvider<TBMAppSettings, TBMDbContext, SysUser>>();

        services.AddAuthentication(x =>
        {
            x.DefaultScheme = AuthenticationScheme;
            x.DefaultAuthenticateScheme = AuthenticationScheme;
            x.DefaultChallengeScheme = AuthenticationScheme;
        })
       .AddJwtBearer(x =>
       {
           x.RequireHttpsMetadata = true;
           x.SaveToken = true;
           x.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true, // 签名的密钥必须校验
               ValidateLifetime = true, // 验证凭证的时间是否过期
               ClockSkew = TimeSpan.Zero, // 时间不能有偏差
               IssuerSigningKey = signingKey,
               ValidIssuer = appSettings.Issuer,
               ValidateIssuer = true,
               ValidAudience = appSettings.Audience,
               ValidateAudience = true,
           };
       });
    }
}