namespace BD.Common8.Repositories.SQLitePCL.Entities;

#pragma warning disable SA1600 // Elements should be documented

[SQLiteTable("C2F5F5F5")]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class KeyValuePair : IEntity<string>
{
    [SQLiteColumn("B1E54167")]
    [SQLitePrimaryKey]
    [SQLiteNotNull]
    public string Id { get; set; } = string.Empty;

    [SQLiteColumn("70E8B6F4")]
    [SQLiteNotNull]
    public byte[] Value { get; set; } = [];

    string DebuggerDisplay() => $"{Id}, {Value}";
}