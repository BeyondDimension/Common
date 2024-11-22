namespace BD.Common8.AspNetCore.Data.Abstractions;

public interface IBMDbContextBase : IDbContext, IBMDbContext
{
    DatabaseFacade Database { get; }

    ChangeTracker ChangeTracker { get; }

    int SaveChanges();

    int SaveChanges(bool acceptAllChangesOnSuccess);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}
