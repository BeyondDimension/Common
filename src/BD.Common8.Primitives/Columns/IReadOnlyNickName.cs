namespace BD.Common8.Primitives.Columns;

/// <summary>
/// 列(只读) - 昵称
/// </summary>
public interface IReadOnlyNickName
{
    /// <inheritdoc cref="INickName.NickName"/>
    string? NickName { get; }
}