// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    const string MigrationsHistory = "__EFMigrationsHistory";

    /// <summary>
    /// 添加数据库上下文，并配置使用 SqlServer
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TAppSettings"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="cfgName"></param>
    /// <returns></returns>
    public static (TAppSettings appSettings, string connectionString) AddDbContextWithAppSettings<TContext, TAppSettings>(
        this IServiceCollection services,
        IConfiguration configuration,
        string cfgName = "DefaultConnection",
        string? migrationsHistoryTableNameSuffix = null)
        where TContext : DbContext
        where TAppSettings : class
    {
        var appSettings_ = configuration.GetSection("AppSettings");
        services.Configure<TAppSettings>(appSettings_);
        var appSettings = appSettings_.Get<TAppSettings>();
        var connectionString = configuration.GetConnectionString(cfgName);
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            var hasSuffix = !string.IsNullOrEmpty(migrationsHistoryTableNameSuffix);
            void SqlServerOptionsAction(SqlServerDbContextOptionsBuilder b)
            {
                var tableName = MigrationsHistory;
                if (hasSuffix)
                {
                    tableName = tableName + "_" + migrationsHistoryTableNameSuffix;
                }
                b.MigrationsHistoryTable(tableName);
            }
            Action<SqlServerDbContextOptionsBuilder>? sqlServerOptionsAction
                = hasSuffix ? SqlServerOptionsAction : null;
            services.AddDbContext<TContext>(options
                => options.UseSqlServer(connectionString, sqlServerOptionsAction));
        }
        else
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        return (appSettings, connectionString);

    }
}