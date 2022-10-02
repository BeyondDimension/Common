// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

/// <summary>
/// 更新时间
/// </summary>
public interface IUpdateTime
{
    /// <inheritdoc cref="IUpdateTime"/>
    DateTimeOffset UpdateTime { get; set; }
}
