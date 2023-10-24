namespace BD.Common8.Repositories.EFCore.Repositories.Abstractions;

/// <inheritdoc cref="IEFRepository"/>
public interface IEFRepository<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : IEFRepository where TEntity : class
{
    /// <inheritdoc cref="DbSet{TEntity}"/>
    DbSet<TEntity> Entity { get; }

    /// <inheritdoc cref="EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution{TEntity}(IQueryable{TEntity})"/>
    IQueryable<TEntity> EntityNoTracking { get; }

    /// <summary>
    /// Notifies when the connection underlying this request is aborted and thus request operations should be cancelled.
    /// </summary>
    protected CancellationToken RequestAborted { get; }
}