// ReSharper disable once CheckNamespace
namespace BD.Common.Data;

public static partial class SqlConstants
{
    public const string SqlServer = "Microsoft.EntityFrameworkCore.SqlServer";

    public const string PostgreSQL = "Npgsql.EntityFrameworkCore.PostgreSQL";

    /// <summary>
    /// 当前数据库提供程序
    /// </summary>
    public static string DatabaseProvider { get; set; } = SqlServer;

    static bool? _ZPlusEnable;

    /// <summary>
    /// 使用启用 Z.EntityFramework.Plus.EFCore
    /// </summary>
    public static bool ZPlusEnable
    {
        get
        {
            if (_ZPlusEnable.HasValue) return _ZPlusEnable.Value;
            return DatabaseProvider switch
            {
                SqlServer => true,
                _ => false,
            };
        }
        set => _ZPlusEnable = value;
    }

    public static string DateTimeOffsetDefaultValueSql => DatabaseProvider switch
    {
        SqlServer => SYSDATETIMEOFFSET,
        PostgreSQL => now,
        _ => throw new ArgumentOutOfRangeException(nameof(DatabaseProvider), DatabaseProvider),
    };

    public static string GuidDefaultValueSql => DatabaseProvider switch
    {
        SqlServer => NEWSEQUENTIALID,
        PostgreSQL => gen_random_uuid,
        _ => throw new ArgumentOutOfRangeException(nameof(DatabaseProvider), DatabaseProvider),
    };

    public static string NextValueSequenceDefaultValueSql(string sequenceName) => DatabaseProvider switch
    {
        SqlServer => $"NEXT VALUE FOR {sequenceName}",
        PostgreSQL => $"nextval('\"{sequenceName}\"')",
        _ => throw new ArgumentOutOfRangeException(nameof(DatabaseProvider), DatabaseProvider),
    };

    /// <summary>
    /// 返回包含计算机的日期和时间的 datetimeoffset(7) 值，SQL Server 的实例正在该计算机上运行。 时区偏移量包含在内。
    /// <para>https://docs.microsoft.com/zh-cn/sql/t-sql/functions/sysdatetimeoffset-transact-sql?view=azuresqldb-current</para>
    /// </summary>
    public const string SYSDATETIMEOFFSET = "SYSDATETIMEOFFSET()";

    /// <summary>
    /// 在启动 Windows 后在指定计算机上创建大于先前通过该函数生成的任何 GUID 的 GUID。 在重新启动 Windows 后，GUID 可以再次从一个较低的范围开始，但仍是全局唯一的。 在 GUID 列用作行标识符时，使用 NEWSEQUENTIALID 可能比使用 NEWID 函数的速度更快。 其原因在于，NEWID 函数导致随机行为并且使用更少的缓存数据页。 使用 NEWSEQUENTIALID 还有助于完全填充数据和索引页。
    /// <para>https://docs.microsoft.com/zh-cn/sql/t-sql/functions/newsequentialid-transact-sql?view=azuresqldb-current</para>
    /// </summary>
    public const string NEWSEQUENTIALID = "newsequentialid()";

    /// <summary>
    /// 允许将显式值插入到表的标识列中。
    /// <para>https://docs.microsoft.com/zh-cn/sql/t-sql/statements/set-identity-insert-transact-sql?view=azuresqldb-current</para>
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="enable"></param>
    /// <returns></returns>
    public static string IDENTITY_INSERT(string tableName, bool enable)
    {
        var value = enable ? ON : OFF;
        var sql = $"SET IDENTITY_INSERT {tableName} {value}";
        return sql;
    }

    public const string ON = "ON";

    public const string OFF = "OFF";

    #region PostgreSQL Functions

    // https://www.postgresql.org/docs/15/functions-uuid.html

    public const string gen_random_uuid = "gen_random_uuid()";

    // https://www.postgresql.org/docs/15/functions-datetime.html#FUNCTIONS-DATETIME-CURRENT

    public const string now = "now()";

    #endregion
}