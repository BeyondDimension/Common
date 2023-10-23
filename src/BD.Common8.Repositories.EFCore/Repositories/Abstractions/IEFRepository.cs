namespace BD.Common8.Repositories.Repositories.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 由 EFCore 实现的 仓储接口
/// </summary>
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
