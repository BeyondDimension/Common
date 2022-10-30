using Microsoft.EntityFrameworkCore.Metadata;

// ReSharper disable once CheckNamespace
namespace BD.Common;

/// <summary>
/// EFCore 工具类
/// </summary>
public static class EFCoreHelper
{
    /// <summary>
    /// 根据类型获取表名称，仅支持 SqlServer
    /// </summary>
    /// <param name="database"></param>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public static string GetTableNameByClrType(DatabaseFacade database, IEntityType entityType)
    {
        var tableName = entityType.GetTableName();
        if (tableName == null) throw new NullReferenceException(nameof(tableName));
        var schema = entityType.GetSchema();
        var databaseProviderName = database.ProviderName;
        return databaseProviderName switch
        {
            SqlConstants.SqlServer => $"[{(string.IsNullOrEmpty(schema) ? "dbo" : schema)}].[{tableName}]",
            SqlConstants.PostgreSQL => string.IsNullOrEmpty(schema) ? $"\"{tableName}\"" : $"\"{schema}\".\"{tableName}\"",
            _ => throw new ArgumentOutOfRangeException(nameof(databaseProviderName), databaseProviderName, "Currently only sqlserver is supported."),
        };
    }

    /// <summary>
    /// 根据类型获取表名称，仅支持 SqlServer
    /// </summary>
    /// <param name="context"></param>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public static string GetTableNameByClrType(DbContext context, Type entityType)
    {
        var entityType_ = context.Model.FindEntityType(entityType);
        return GetTableNameByClrType(context.Database, entityType_.ThrowIsNull(nameof(entityType)));
    }
}
