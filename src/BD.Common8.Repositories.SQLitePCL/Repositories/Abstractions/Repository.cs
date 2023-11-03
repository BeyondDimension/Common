namespace BD.Common8.Repositories.SQLitePCL.Repositories.Abstractions;

/// <summary>
/// 由 sqlite-net-pcl 实现的仓储层
/// </summary>
public abstract class Repository : IRepository
{
    #region https://codetraveler.io/2019/11/26/efficiently-initializing-sqlite-database/
    static string? dataBaseDirectory;

    /// <summary>
    /// 获取或设定数据库目录
    /// </summary>
    public static string DataBaseDirectory
    {
        get
        {
            if (string.IsNullOrWhiteSpace(dataBaseDirectory))
                throw new ArgumentNullException(nameof(dataBaseDirectory));
            return dataBaseDirectory;
        }

        set
        {
            dataBaseDirectory = value;
        }
    }

    static SQLiteConnection? connectionSync;

    static SQLiteConnection ConnectionSync
    {
        get
        {
            if (connectionSync == null)
            {
                var dbPath = DataBaseDirectory;
                if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
                dbPath = Path.Combine(dbPath, "application2.dbf"); // 与异步连接使用不同的数据库文件隔离
                connectionSync = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            }
            return connectionSync;
        }
    }

    /// <summary>
    /// 根据数据库路径获取连接对象
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SQLiteAsyncConnection GetConnection(string dbPath)
        => new(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

    /// <summary>
    /// 根据默认数据库目录获取连接对象
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static SQLiteAsyncConnection GetConnection()
    {
        var dbPath = DataBaseDirectory;
        if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
        dbPath = Path.Combine(dbPath, "application.dbf");
        return GetConnection(dbPath);
    }

    static readonly Lazy<SQLiteAsyncConnection> dbConnection = new(GetConnection);

    static SQLiteAsyncConnection DbConnection => dbConnection.Value;

    /// <summary>
    /// 根据泛型参数类型获取数据库连接对象，并创建数据表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static async ValueTask<SQLiteAsyncConnection> GetDbConnection<T>()
    {
        var connection = DbConnection;
        await GetDbConnection<T>(connection);
        return connection;
    }

    /// <summary>
    /// 根据指定的泛型类型获取数据库对象，并检查数据库中是否已存在相关表格，如果不存在，则启用写入日志并创建相应的表格，异步
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static async ValueTask GetDbConnection<T>(SQLiteAsyncConnection connection)
    {
        if (!connection.TableMappings.Any(x => x.MappedType == typeof(T)))
        {
            // On sqlite-net v1.6.0+, enabling write-ahead logging allows for faster database execution
            await connection.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
            await connection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 根据指定的泛型类型获取数据库对象，并检查数据库中是否已存在相关表格，如果不存在，则启用写入日志并创建相应的表格，同步
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SQLiteConnection GetDbConnectionSync<T>()
    {
        var conn = ConnectionSync;
        if (!conn.TableMappings.Any(x => x.MappedType == typeof(T)))
        {
            // On sqlite-net v1.6.0+, enabling write-ahead logging allows for faster database execution
            conn.EnableWriteAheadLogging();
            conn.CreateTables(CreateFlags.None, typeof(T));
        }
        return conn;
    }

    /// <summary>
    /// 默认重试次数
    /// </summary>
    public const int DefaultRetryCount = 3;

    /// <summary>
    /// 根据尝试次数，返回 <see cref="TimeSpan"/> 重试延迟时间
    /// </summary>
    /// <param name="attemptNumber"></param>
    /// <returns></returns>
    public static TimeSpan PollyRetryAttempt(int attemptNumber)
        => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));

    /// <summary>
    /// 带有重试机制的异步操作，当抛出 <see cref="SQLiteException"/> 异常时，将会进行重试
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="delegate">要执行的异步任务</param>
    /// <param name="retryCount">重试次数，默认为 <see cref="DefaultRetryCount"/></param>
    /// <param name="cancellationToken">取消令牌</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static Task<T> AttemptAndRetry<T>(
        Func<CancellationToken, Task<T>> @delegate,
        int retryCount = DefaultRetryCount,
        CancellationToken cancellationToken = default)
        => Policy.Handle<SQLiteException>()
        .WaitAndRetryAsync(retryCount, PollyRetryAttempt)
        .ExecuteAsync(@delegate, cancellationToken);

    #endregion
}