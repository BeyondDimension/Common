namespace BD.Common.Columns;

/// <summary>
/// 列(只读) - 昵称
/// </summary>
public interface IReadOnlyNickName
{
    /// <inheritdoc cref="INickName.NickName"/>
    string? NickName { get; }
}