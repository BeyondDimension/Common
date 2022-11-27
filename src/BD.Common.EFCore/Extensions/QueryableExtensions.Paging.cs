// ReSharper disable once CheckNamespace
namespace System;

public static partial class QueryableExtensions
{
    #region Paging

    /// <summary>
    /// 根据页码进行分页查询，调用此方法前 必须 进行排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="current"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<PagedModel<TEntity>> PagingAsync<TEntity>(
        this IQueryable<TEntity> source,
        int current = IPagedModel.DefaultCurrent,
        int pageSize = IPagedModel.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        int skipCount, total;
        IQueryable<TEntity> query;
        TEntity[] dataSource;
        PagedModel<TEntity> pagedModel;
#if !__NOT_IMPORT_Z_EF_PLUS__
        if (SqlConstants.ZPlusEnable)
        {
            skipCount = (current - 1) * pageSize;
            var futureTotal = source.DeferredCount().FutureValue();
            query = source.Skip(skipCount).Take(pageSize);
#if DEBUG
        var sqlString = query.ToQueryString();
#endif
            var futureDataSource = query.Future();
            total = await futureTotal.ValueAsync(cancellationToken);
            dataSource = await futureDataSource.ToArrayAsync(cancellationToken);
            pagedModel = new PagedModel<TEntity>
            {
                Current = current,
                PageSize = pageSize,
                Total = total,
                DataSource = dataSource,
            };
            return pagedModel;
        }
#endif
        skipCount = (current - 1) * pageSize;
        total = await source.CountAsync(cancellationToken);
        query = source.Skip(skipCount).Take(pageSize);
#if DEBUG
        var sqlString = query.ToQueryString();
#endif
        dataSource = await query.ToArrayAsync(cancellationToken);
        pagedModel = new PagedModel<TEntity>
        {
            Current = current,
            PageSize = pageSize,
            Total = total,
            DataSource = dataSource,
        };
        return pagedModel;
    }

    /// <summary>
    /// 根据偏移量进行分页查询，调用此方法前 必须 进行排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="offset"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<PagedModel<TEntity>> PagingOffsetAsync<TEntity>(
        this IQueryable<TEntity> source,
        int offset = 0,
        int pageSize = IPagedModel.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (offset > 0) source = source.Skip(offset);
        var dataSource = await source.Take(pageSize).ToArrayAsync(cancellationToken);
        var pagedModel = new PagedModel<TEntity>
        {
            PageSize = pageSize,
            DataSource = dataSource,
        };
        return pagedModel;
    }

    /// <summary>
    /// 根据页码进行分页查询，此方法 已使用 创建时间倒序排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="current"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<PagedModel<TEntity>> PagingSortAsync<TEntity>(
      this IQueryable<TEntity> source,
      int current = IPagedModel.DefaultCurrent,
      int pageSize = IPagedModel.DefaultPageSize,
      CancellationToken cancellationToken = default) where TEntity : class, ICreationTime
    {
        source = source.OrderByDescending(x => x.CreationTime);
        return source.PagingAsync(current, pageSize, cancellationToken);
    }

    /// <summary>
    /// 根据创建时间描点进行分页查询，此方法 已使用 创建时间倒序排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="anchor"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<PagedModel<TEntity>> PagingSortAsync<TEntity>(
        this IQueryable<TEntity> source,
        ICreationTime? anchor = null,
        int pageSize = IPagedModel.DefaultPageSize,
        CancellationToken cancellationToken = default) where TEntity : class, ICreationTime
    {
        var anchorCreationTime = anchor == null ? default : anchor.CreationTime;
        return source.PagingSortAsync(anchorCreationTime, pageSize, cancellationToken);
    }

    /// <summary>
    /// 根据创建时间描点进行分页查询，此方法 已使用 创建时间倒序排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="anchor"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<PagedModel<TEntity>> PagingSortAsync<TEntity>(
       this IQueryable<TEntity> source,
       DateTimeOffset anchor = default,
       int pageSize = IPagedModel.DefaultPageSize,
       CancellationToken cancellationToken = default)
        where TEntity : class, ICreationTime
    {
        if (anchor != default) source = source.Where(x => x.CreationTime > anchor);
        source = source.OrderByDescending(x => x.CreationTime);
        return await source.PagingOffsetAsync(0, pageSize, cancellationToken);
    }

    /// <summary>
    /// 根据描点进行分页查询，调用此方法前 必须 进行排序 和 判断描点条件
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<PagedModel<TEntity>> PagingAnchorAsync<TEntity>(
        this IQueryable<TEntity> source,
        int pageSize = IPagedModel.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = source.Take(pageSize);
#if DEBUG
        var sqlString = query.ToQueryString();
#endif
        var dataSource = await query.ToArrayAsync(cancellationToken);
        var pagedModel = new PagedModel<TEntity>
        {
            PageSize = pageSize,
            DataSource = dataSource,
        };
        return pagedModel;
    }

    #endregion
}