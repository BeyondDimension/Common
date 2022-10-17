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

    static SQLiteAsyncConnection GetConnection()
    {
        var dbPath = DataBaseDirectory;
        if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
        dbPath = Path.Combine(dbPath, "application.dbf");
        return new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
    }

    static readonly Lazy<SQLiteAsyncConnection> dbConnection = new(GetConnection);

    static SQLiteAsyncConnection DbConnection => dbConnection.Value;

    protected static async ValueTask<SQLiteAsyncConnection> GetDbConnection<T>()
    {
        if (!DbConnection.TableMappings.Any(x => x.MappedType == typeof(T)))
        {
            // On sqlite-net v1.6.0+, enabling write-ahead logging allows for faster database execution
            await DbConnection.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
            await DbConnection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
        }
        return DbConnection;
    }

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

    protected static Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 10)
    {
        return Policy.Handle<SQLiteException>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);
        static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
    }

    #endregion
}