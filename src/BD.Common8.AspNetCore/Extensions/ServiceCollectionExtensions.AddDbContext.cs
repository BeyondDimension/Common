namespace BD.Common8.AspNetCore.Extensions;

/// <summary>
/// 提供用于配置和添加 DbContext 到应用程序的扩展方法
/// </summary>
public static partial class AddDbContext_ServiceCollectionExtensions
{
    public const string DefaultValue_databaseProvider = SqlConstants.PostgreSQL;
    public const string DefaultValue_connectionStringKeyName = "DefaultConnection";

    /// <summary>
    /// 添加 DbContext 到应用程序
    /// </summary>
    /// <typeparam name="TContext">要添加的 DbContext 类型</typeparam>
    /// <param name="builder">WebApplicationBuilder 实例</param>
    /// <param name="o">配置 DbContextOptionsBuilder 的可选操作</param>
    /// <param name="databaseProvider">数据库提供程序名称</param>
    /// <param name="connectionStringKeyName">连接字符串键的名称，为 <see langword="null"/> 时不调用 GetConnectionString 与 AddDbContext</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddDbContext<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TContext>(
        this WebApplicationBuilder builder,
        Action<DbContextOptionsBuilder>? o = null,
        string databaseProvider = DefaultValue_databaseProvider,
        string? connectionStringKeyName = DefaultValue_connectionStringKeyName)
        where TContext : DbContext
    {
        SqlConstants.DatabaseProvider = databaseProvider;
        var isPostgreSQL = SqlConstants.DatabaseProvider == SqlConstants.PostgreSQL;
        if (isPostgreSQL)
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        if (connectionStringKeyName != null)
        {
            var connectionString = builder.Configuration.GetConnectionString(connectionStringKeyName);
            connectionString.ThrowIsNull();
            builder.Services.AddDbContext<TContext>(options =>
            {
                switch (databaseProvider)
                {
                    case SqlConstants.PostgreSQL:
                        options.UseNpgsql(connectionString);
                        break;
#if HAS_SQLSERVER
                    case SqlConstants.SqlServer:
                        options.UseSqlServer(connectionString);
                        break;
#endif
                    default:
                        throw new ArgumentOutOfRangeException(nameof(databaseProvider), databaseProvider);
                }
                o?.Invoke(options);
            });
        }
    }

    /// <summary>
    /// 添加 DbContext 作为 TService 到应用程序并配置服务的生命周期为 Scoped
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型</typeparam>
    /// <typeparam name="TContext">要添加的 DbContext 类型</typeparam>
    /// <param name="builder">WebApplicationBuilder 实例</param>
    /// <param name="o">配置 DbContextOptionsBuilder 的可选操作</param>
    /// <param name="databaseProvider">数据库提供程序名称</param>
    /// <param name="connectionStringKeyName">连接字符串键的名称，为 <see langword="null"/> 时不调用 GetConnectionString 与 AddDbContext</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddDbContext<TService, [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TContext>(this WebApplicationBuilder builder,
        Action<DbContextOptionsBuilder>? o = null,
        string databaseProvider = DefaultValue_databaseProvider,
        string? connectionStringKeyName = DefaultValue_connectionStringKeyName)
        where TContext : DbContext, TService
        where TService : class
    {
        builder.AddDbContext<TContext>(o, databaseProvider, connectionStringKeyName);
        builder.Services.AddScoped<TService>(s => s.GetRequiredService<TContext>());
    }
}