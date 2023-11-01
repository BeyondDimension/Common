namespace BD.Common8.Repositories.SQLitePCL.Entities;

/// <summary>
/// 提供了用于存储键值对的模型
/// </summary>
[SQLiteTable("C2F5F5F5")]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class KeyValuePair : IEntity<string>
{
    /// <summary>
    /// 获取或设置键的唯一标识符
    /// </summary>
    [SQLiteColumn("B1E54167")]
    [SQLitePrimaryKey]
    [SQLiteNotNull]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置对应键的值
    /// </summary>
    [SQLiteColumn("70E8B6F4")]
    [SQLiteNotNull]
    public byte[] Value { get; set; } = [];

    /// <summary>
    /// 返回用于调试显示的字符串表示形式
    /// </summary>
    string DebuggerDisplay() => $"{Id}, {Value}";
}