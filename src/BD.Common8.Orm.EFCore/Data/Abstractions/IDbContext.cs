namespace BD.Common8.Orm.EFCore.Data.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public interface IDbContext
{
    DbContext Thiz { get; }

    void SetUpdateTime()
    {
        foreach (var entity in Thiz.ChangeTracker.Entries())
            if (entity.State == EntityState.Modified && entity.Entity is IUpdateTime u)
                u.UpdateTime = DateTimeOffset.Now;
    }
}
