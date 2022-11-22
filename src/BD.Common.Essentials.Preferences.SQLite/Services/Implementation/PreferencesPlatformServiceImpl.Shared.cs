using SQLite;
using ColumnAttribute = SQLite.ColumnAttribute;
using SQLiteNotNull = SQLite.NotNullAttribute;
using SQLiteTable = SQLite.TableAttribute;

namespace BD.Common.Services.Implementation;

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
        [Column(ColumnName_Id)]
        [PrimaryKey]
        [SQLiteNotNull]
        public string Id { get; set; } = string.Empty;

        [Column(ColumnName_Value)]
        [SQLiteNotNull]
        public string Value { get; set; } = string.Empty;

        [Column(ColumnName_SharedName)]
        [SQLiteNotNull]
        public string SharedName { get; set; } = string.Empty;

        string DebuggerDisplay() => SharedName == string.Empty ? $"{Id}, {Value}" : $"{Id}, {Value}, {SharedName}";
    }
}
