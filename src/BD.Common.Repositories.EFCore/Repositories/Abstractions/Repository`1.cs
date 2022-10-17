namespace BD.Common.Repositories.Abstractions;

/// <summary>
/// 由 EntityFrameworkCore 实现的仓储层
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class Repository<TDbContext> : IRepository where TDbContext : DbContext
{
    protected readonly TDbContext db;

    public Repository(TDbContext dbContext) => db = dbContext;
}