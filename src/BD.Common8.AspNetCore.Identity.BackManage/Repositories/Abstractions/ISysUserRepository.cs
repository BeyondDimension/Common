namespace BD.Common8.AspNetCore.Repositories.Abstractions;

public interface ISysUserRepository
{
    /// <summary>
    /// 表格查询
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="current"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<PagedModel<SysUserTableItem>> QueryAsync(
             string? userName,
             int current = IPagedModel.DefaultCurrent,
             int pageSize = IPagedModel.DefaultPageSize);
}
