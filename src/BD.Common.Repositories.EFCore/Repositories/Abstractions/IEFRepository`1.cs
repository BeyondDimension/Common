namespace BD.Common.Repositories.Abstractions;

public interface IEFRepository<TEntity> : IEFRepository where TEntity : class
{
    DbSet<TEntity> Entity { get; }

    protected CancellationToken RequestAborted { get; }
}