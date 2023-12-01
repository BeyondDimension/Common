namespace BD.Common8.Columns;

/// <summary>
/// 创建时间
/// </summary>
public interface ICreationTime
{
    /// <inheritdoc cref="ICreationTime"/>
    DateTimeOffset CreationTime { get; set; }
}