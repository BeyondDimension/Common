namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

sealed partial class PreferencesPlatformServiceImpl : IPreferencesGenericPlatformService
{
    const string TableName = "1984415E";
    const string ColumnName_Id = "0F5E4BAA";
    const string ColumnName_Value = "4FC331D7";
    const string ColumnName_SharedName = "F6A739AA";

    [SQLiteTable(TableName)]
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public sealed class Entity : IEntity<string>
    {
        [SQLiteColumn(ColumnName_Id)]
        [SQLitePrimaryKey]
        [SQLiteNotNull]
        public string Id { get; set; } = string.Empty;

        [SQLiteColumn(ColumnName_Value)]
        [SQLiteNotNull]
        public string Value { get; set; } = string.Empty;

        [SQLiteColumn(ColumnName_SharedName)]
        [SQLiteNotNull]
        public string SharedName { get; set; } = string.Empty;

        string DebuggerDisplay() => SharedName == string.Empty ?
            $"{Id}, {Value}" :
            $"{Id}, {Value}, {SharedName}";
    }
}
