// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddDbContext<TContext>(
        this WebApplicationBuilder builder,
        Action<DbContextOptionsBuilder>? o = null,
        string databaseProvider = SqlConstants.PostgreSQL,
        string connectionStringKeyName = "DefaultConnection")
        where TContext : DbContext
    {
        SqlConstants.DatabaseProvider = databaseProvider;
        var isPostgreSQL = SqlConstants.DatabaseProvider == SqlConstants.PostgreSQL;
        if (isPostgreSQL)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        var connectionString = builder.Configuration.GetConnectionString(connectionStringKeyName);
        connectionString.ThrowIsNull();
        builder.Services.AddDbContext<TContext>(options =>
        {
            switch (databaseProvider)
            {
                case SqlConstants.PostgreSQL:
                    options.UseNpgsql(connectionString);
                    break;
                case SqlConstants.SqlServer:
                    options.UseSqlServer(connectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseProvider), databaseProvider);
            }
            o?.Invoke(options);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddDbContext<TService, TContext>(this WebApplicationBuilder builder,
        Action<DbContextOptionsBuilder>? o = null,
        string databaseProvider = SqlConstants.PostgreSQL,
        string connectionStringKeyName = "DefaultConnection")
        where TContext : DbContext, TService
        where TService : class
    {
        builder.AddDbContext<TContext>(o, databaseProvider, connectionStringKeyName);
        builder.Services.AddScoped<TService>(s => s.GetRequiredService<TContext>());
    }
}