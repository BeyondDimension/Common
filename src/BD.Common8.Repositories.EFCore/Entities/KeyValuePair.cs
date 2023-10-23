namespace BD.Common8.Repositories.Entities;

/// <summary>
/// 键值对表
/// <para>https://stackoverflow.com/questions/514603/key-value-pairs-in-a-database-table</para>
/// </summary>
[Table("KeyValuePairs")]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class KeyValuePair : IEntity<string>, ISoftDeleted
{
    /// <summary>
    /// 键
    /// </summary>
    [Key] // EF 主键
    [Comment("键")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 值
    /// </summary>
    [Required] // EF not null
    [Comment("值")]
    public string Value { get; set; } = string.Empty;

    /// <inheritdoc/>
    [Comment("是否软删除")]
    public bool SoftDeleted { get; set; }

    string DebuggerDisplay() => $"{Id}, {Value}";
}