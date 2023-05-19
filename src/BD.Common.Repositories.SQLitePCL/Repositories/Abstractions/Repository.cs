namespace BD.Common.Repositories.Abstractions;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SQLiteAsyncConnection GetConnection(string dbPath)
        => new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static async ValueTask<SQLiteAsyncConnection> GetDbConnection<T>()
    {
        var connection = DbConnection;
        await GetDbConnection<T>(connection);
        return connection;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static Task<T> AttemptAndRetry<T>(Func<CancellationToken, Task<T>> @delegate, int numRetries = 10, CancellationToken cancellationToken = default)
    {
        return Policy.Handle<SQLiteException>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(@delegate, cancellationToken);
        static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
    }

    #endregion
}