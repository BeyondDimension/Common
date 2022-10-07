namespace BD.Common.Pages.Abstractions;

/// <summary>
/// 查询表格
/// </summary>
public interface ITableQuery
{
    bool QueryLoading { get; set; }

    Task QueryAsync();
}

/// <summary>
/// 查询表格泛型
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITableQuery<T> : ITableQuery
{
    T DataSource { get; set; }
}