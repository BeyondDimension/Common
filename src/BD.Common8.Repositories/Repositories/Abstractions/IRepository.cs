namespace BD.Common8.Repositories.Repositories.Abstractions;

/// <summary>
/// 仓储接口
/// </summary>
public interface IRepository
{
    /// <summary>
    /// 批量操作返回统计受影响的行数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="operate"></param>
    /// <returns></returns>
    public static async Task<int> OperateRangeAsync<T>(
      IEnumerable<T> entities,
      Func<T, Task<int>> operate)
        => (await Task.WhenAll(entities.Select(operate))).Sum();

    /// <summary>
    /// 批量操作返回统计受影响的行数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="operate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> OperateRangeAsync<T>(
      IEnumerable<T> entities,
      Func<T, CancellationToken, Task<int>> operate,
      CancellationToken cancellationToken)
        => (await Task.WhenAll(entities.Select(async x => await operate(x, cancellationToken)))).Sum();

    /// <summary>
    /// 批量操作返回统计受影响的行数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="operate"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<(int rowCount, T entity)> OperateRangeAsyncEnumerable<T>(
      IEnumerable<T> entities,
      Func<T, Task<int>> operate)
    {
        foreach (var entity in entities)
        {
            var rowCount = await operate(entity);
            yield return (rowCount, entity);
        }
    }

    /// <summary>
    /// 批量操作返回统计受影响的行数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="operate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<(int rowCount, T entity)> OperateRangeAsyncEnumerable<T>(
      IEnumerable<T> entities,
      Func<T, CancellationToken, Task<int>> operate,
      [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            var rowCount = await operate(entity, cancellationToken);
            yield return (rowCount, entity);
        }
    }

    /// <summary>
    /// 批量操作返回统计受影响的行数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="operate"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<(int rowCount, DbRowExecResult result, T entity)> OperateRangeAsyncEnumerable<T>(
        IEnumerable<T> entities,
        Func<T, Task<(int rowCount, DbRowExecResult result)>> operate)
    {
        foreach (var entity in entities)
        {
            var (rowCount, logic) = await operate(entity);
            yield return (rowCount, logic, entity);
        }
    }

    /// <summary>
    /// 批量操作返回统计受影响的行数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="operate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<(int rowCount, DbRowExecResult result, T entity)> OperateRangeAsyncEnumerable<T>(
        IEnumerable<T> entities,
        Func<T, CancellationToken, Task<(int rowCount, DbRowExecResult result)>> operate,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            var (rowCount, logic) = await operate(entity, cancellationToken);
            yield return (rowCount, logic, entity);
        }
    }
}