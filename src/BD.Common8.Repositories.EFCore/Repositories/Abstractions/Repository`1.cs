namespace BD.Common8.Repositories.EFCore.Repositories.Abstractions;

/// <summary>
/// 由 EntityFrameworkCore 实现的仓储层
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class Repository<TDbContext>(TDbContext dbContext) : IRepository where TDbContext : DbContext
{
    /// <summary>
    /// 数据库上下文实例
    /// </summary>
    protected readonly TDbContext db = dbContext;
}