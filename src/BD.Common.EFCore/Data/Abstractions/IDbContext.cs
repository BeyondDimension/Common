namespace BD.Common.Data.Abstractions;

public interface IDbContext
{
    DbContext Thiz { get; }

    void SetUpdateTime()
    {
        foreach (var entity in Thiz.ChangeTracker.Entries())
        {
            if (entity.State == EntityState.Modified && entity.Entity is IUpdateTime u)
            {
                u.UpdateTime = DateTimeOffset.Now;
            }
        }
    }
}
