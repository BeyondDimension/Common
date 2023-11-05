namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 提供首选项设置平台服务的实现
/// </summary>
sealed partial class PreferencesPlatformServiceImpl : IPreferencesGenericPlatformService
{
    /// <summary>
    /// 数据表名称
    /// </summary>
    const string TableName = "1984415E";

    /// <summary>
    /// Id 字段的列名
    /// </summary>
    const string ColumnName_Id = "0F5E4BAA";

    /// <summary>
    /// Value 字段的列名
    /// </summary>
    const string ColumnName_Value = "4FC331D7";

    /// <summary>
    /// SharedName 字段的列名
    /// </summary>
    const string ColumnName_SharedName = "F6A739AA";

    [SQLiteTable(TableName)]
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public sealed class Entity : IEntity<string>
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        [SQLiteColumn(ColumnName_Id)]
        [SQLitePrimaryKey]
        [SQLiteNotNull]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        [SQLiteColumn(ColumnName_Value)]
        [SQLiteNotNull]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 共享名称
        /// </summary>
        [SQLiteColumn(ColumnName_SharedName)]
        [SQLiteNotNull]
        public string SharedName { get; set; } = string.Empty;

        /// <summary>
        /// 获取调试显示字符串
        /// </summary>
        string DebuggerDisplay() => SharedName == string.Empty ?
            $"{Id}, {Value}" :
            $"{Id}, {Value}, {SharedName}";
    }
}
