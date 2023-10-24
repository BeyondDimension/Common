namespace BD.Common8.Repositories.EFCore.Repositories.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 由 EntityFrameworkCore 实现的仓储层
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class Repository<TDbContext>(TDbContext dbContext) : IRepository where TDbContext : DbContext
{
    protected readonly TDbContext db = dbContext;
}