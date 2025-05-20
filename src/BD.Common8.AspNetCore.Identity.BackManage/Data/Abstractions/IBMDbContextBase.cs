using BD.Common8.Orm.EFCore.Data.Abstractions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BD.Common8.AspNetCore.Data.Abstractions;

public interface IBMDbContextBase : IDbContext, IBMDbContext
{
    DatabaseFacade Database { get; }

    ChangeTracker ChangeTracker { get; }

    int SaveChanges();

    int SaveChanges(bool acceptAllChangesOnSuccess);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}
