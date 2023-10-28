namespace BD.Common8.Primitives.Columns;

/// <summary>
/// 是否软删除
/// </summary>
public interface ISoftDeleted
{
    /// <inheritdoc cref="ISoftDeleted"/>
    bool SoftDeleted { get; set; }
}