namespace Microsoft.EntityFrameworkCore;

public static partial class DbContextExtensions
{
    /// <summary>
    /// 根据类型获取表名称
    /// </summary>
    /// <param name="database"></param>
    /// <param name="entityType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTableNameByClrType(this DatabaseFacade database, IEntityType entityType)
    {
        var tableName = entityType.GetTableName();
        tableName.ThrowIsNull();
        var schema = entityType.GetSchema();
        var databaseProviderName = database.ProviderName;
        return databaseProviderName switch
        {
            SqlConstants.SqlServer => $"[{(string.IsNullOrEmpty(schema) ? "dbo" : schema)}].[{tableName}]",
            SqlConstants.PostgreSQL => string.IsNullOrEmpty(schema) ? $"\"{tableName}\"" : $"\"{schema}\".\"{tableName}\"",
            _ => throw ThrowHelper.GetArgumentOutOfRangeException(databaseProviderName),
        };
    }

    /// <summary>
    /// 根据类型获取表名称
    /// </summary>
    /// <param name="context"></param>
    /// <param name="entityType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTableNameByClrType(this DbContext context, [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] Type entityType)
    {
        var entityType_ = context.Model.FindEntityType(entityType);
        return context.Database.GetTableNameByClrType(entityType_.ThrowIsNull(nameof(entityType)));
    }
}
