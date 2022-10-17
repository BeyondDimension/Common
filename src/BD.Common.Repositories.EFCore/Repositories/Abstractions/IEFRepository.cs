namespace BD.Common.Repositories.Abstractions;

public interface IEFRepository
{
    DbContext DbContext { get; }

    string TableName { get; }

    /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)"/>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
       => DbContext.SaveChangesAsync(cancellationToken);

    /// <inheritdoc cref="DbContext.SaveChangesAsync(bool, CancellationToken)"/>
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
       => DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
}
