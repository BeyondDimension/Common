namespace BD.Common8.Orm.EFCore.Data.Abstractions;

/// <summary>
/// 数据库上下文接口
/// <list type="bullet">
/// <item>用于在数据库操作过程中跟踪和记录对象的最后更新时间</item>
/// </list>
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// 获取 DbContext 实例
    /// </summary>
    DbContext Thiz { get; }

    /// <summary>
    /// 设置更新时间
    /// </summary>
    void SetUpdateTime()
    {
        foreach (var entity in Thiz.ChangeTracker.Entries())
            if (entity.State == EntityState.Modified && entity.Entity is IUpdateTime u)
                u.UpdateTime = DateTimeOffset.Now;
    }
}
