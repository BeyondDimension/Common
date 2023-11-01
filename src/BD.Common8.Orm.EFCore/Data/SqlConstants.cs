namespace BD.Common8.Orm.EFCore.Data;

/// <summary>
/// 用于存储 SQL 相关的常量和方法
/// </summary>
public static partial class SqlConstants
{
    /// <summary>
    /// Microsoft.EntityFrameworkCore.SqlServer
    /// </summary>
    public const string SqlServer = "Microsoft.EntityFrameworkCore.SqlServer";

    /// <summary>
    /// Npgsql.EntityFrameworkCore.PostgreSQL
    /// </summary>
    public const string PostgreSQL = "Npgsql.EntityFrameworkCore.PostgreSQL";

    /// <summary>
    /// 当前数据库提供程序
    /// </summary>
    public static string DatabaseProvider { get; set; } = SqlServer;

    /// <summary>
    /// 是否开启 ZPlus 功能
    /// </summary>
    static bool? _ZPlusEnable;

    /// <summary>
    /// 使用启用 Z.EntityFramework.Plus.EFCore
    /// </summary>
    public static bool ZPlusEnable
    {
        get
        {
            if (_ZPlusEnable.HasValue)
                return _ZPlusEnable.Value;
            return DatabaseProvider switch
            {
                SqlServer => true,
                _ => false,
            };
        }
        set => _ZPlusEnable = value;
    }

    /// <summary>
    /// 获取 DateTimeOffset 类型字段的默认值 SQL 表达式
    /// <para> 根据当前数据库提供程序判断：</para>
    /// 如果为 SqlServer，则返回  <see cref="SYSDATETIMEOFFSET"/>；
    /// 如果为 PostgreSQL，则返回 <see cref="now"/>；
    /// 其它情况则抛出异常
    /// </summary>
    public static string DateTimeOffsetDefaultValueSql => DatabaseProvider switch
    {
        SqlServer => SYSDATETIMEOFFSET,
        PostgreSQL => now,
        _ => throw ThrowHelper.GetArgumentOutOfRangeException(DatabaseProvider),
    };

    /// <summary>
    /// 获取 Guid 类型字段的默认值 SQL 表达式
    /// <para> 根据当前数据库提供程序判断：</para>
    /// 如果为 SqlServer，则返回  <see cref="NEWSEQUENTIALID"/>；
    /// 如果为 PostgreSQL，则返回 <see cref="gen_random_uuid"/>；
    /// 其它情况则抛出异常 
    /// </summary>
    public static string GuidDefaultValueSql => DatabaseProvider switch
    {
        SqlServer => NEWSEQUENTIALID,
        PostgreSQL => gen_random_uuid,
        _ => throw ThrowHelper.GetArgumentOutOfRangeException(DatabaseProvider),
    };

    /// <summary>
    /// 根据序列名称获取下一个值的默认值 SQL 表达式
    /// </summary>
    public static string NextValueSequenceDefaultValueSql(string sequenceName) => DatabaseProvider switch
    {
        SqlServer => $"NEXT VALUE FOR {sequenceName}",
        PostgreSQL => $"nextval('\"{sequenceName}\"')",
        _ => throw ThrowHelper.GetArgumentOutOfRangeException(DatabaseProvider),
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

    /// <summary>
    /// 表示 ON 的字符串常量
    /// </summary>
    public const string ON = "ON";

    /// <summary>
    /// 表示 OFF 的字符串常量
    /// </summary>
    public const string OFF = "OFF";

    #region PostgreSQL Functions

    /// <summary>
    /// PostgreSQL 的生成随机 UUID 函数的字符串常量
    /// <para> https://www.postgresql.org/docs/15/functions-uuid.html </para> 
    /// </summary>
    public const string gen_random_uuid = "gen_random_uuid()";

    /// <summary>
    /// PostgreSQL 的获取当前时间函数的字符串常量
    /// <para> https://www.postgresql.org/docs/15/functions-datetime.html#FUNCTIONS-DATETIME-CURRENT </para> 
    /// </summary>
    public const string now = "now()";

    #endregion

    /// <summary>
    /// 类型 <see cref="IOrder"/> 的 Type 对象
    /// </summary>
    public static readonly Type POrder = typeof(IOrder);

    /// <summary>
    /// 类型 <see cref="ISoftDeleted"/> 的 Type 对象
    /// </summary>
    public static readonly Type PSoftDeleted = typeof(ISoftDeleted);

    /// <summary>
    /// 类型 <see cref="ICreationTime"/> 的 Type 对象
    /// </summary>
    public static readonly Type PCreationTime = typeof(ICreationTime);

    /// <summary>
    /// 类型 <see cref="IUpdateTime"/> 的 Type 对象
    /// </summary>
    public static readonly Type PUpdateTime = typeof(IUpdateTime);

    /// <summary>
    /// 类型 <see cref="IDisable"/> 的 Type 对象
    /// </summary>
    public static readonly Type PDisable = typeof(IDisable);

    /// <summary>
    /// 类型 <see cref="INEWSEQUENTIALID"/> 的 Type 对象
    /// </summary>
    public static readonly Type PNEWSEQUENTIALID = typeof(INEWSEQUENTIALID);

    /// <summary>
    /// 类型 <see cref="IPhoneNumber"/> 的 Type 对象
    /// </summary>
    public static readonly Type PPhoneNumber = typeof(IPhoneNumber);

    ///< summary>
    /// 共享类型 Dictionary<string, object> 的 Type 对象
    /// <para>  https://docs.microsoft.com/zh-cn/ef/core/modeling/shadow-properties#property-bag-entity-types </para> 
    /// </summary>
    public static readonly Type SharedType = typeof(Dictionary<string, object>);

    /// <summary>
    /// 存储了特定类型的对象，这些对象被标记为"软删除"
    /// </summary>
    internal static readonly HashSet<Type> SoftDeleted = [];

    /// <summary>
    /// 判断指定类型是否实现了软删除接口
    /// </summary>
    public static bool IsSoftDeleted(Type type) => SoftDeleted.Contains(type);
}