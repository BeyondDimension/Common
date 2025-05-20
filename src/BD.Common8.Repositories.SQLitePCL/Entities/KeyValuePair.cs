using BD.Common8.Entities.Abstractions;
using System.Diagnostics;

namespace BD.Common8.Repositories.SQLitePCL.Entities;

/// <summary>
/// 提供了用于存储键值对的模型
/// </summary>
[global::SQLite.Table("C2F5F5F5")]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class KeyValuePair : IEntity<string>
{
    /// <summary>
    /// 获取或设置键的唯一标识符
    /// </summary>
    [global::SQLite.Column("B1E54167")]
    [global::SQLite.PrimaryKey]
    [global::SQLite.NotNull]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置对应键的值
    /// </summary>
    [global::SQLite.Column("70E8B6F4")]
    [global::SQLite.NotNull]
    public byte[] Value { get; set; } = [];

    /// <summary>
    /// 返回用于调试显示的字符串表示形式
    /// </summary>
    string DebuggerDisplay() => $"{Id}, {Value}";
}