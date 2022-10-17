using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable once CheckNamespace
namespace BD.Common;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// 软删除的查询过滤表达式
    /// <para>https://docs.microsoft.com/zh-cn/ef/core/querying/filters#example</para>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static LambdaExpression SoftDeletedQueryFilter(Type type)
    {
        var parameter = Expression.Parameter(type);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var left = Expression.PropertyOrField(parameter, nameof(ISoftDeleted.SoftDeleted));
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var body = Expression.Not(left);
        return Expression.Lambda(body, parameter);
    }

    /// <summary>
    /// 根据实体模型继承的接口，生成列的 索引/默认值
    /// 在 <see cref="DbContext.OnModelCreating(ModelBuilder)"/> 中调用此函数，仅支持 SqlServer
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="action"></param>
    public static IEnumerable<IMutableEntityType> BuildEntities(this ModelBuilder modelBuilder, Action<ModelBuilder, IMutableEntityType, Type, Action<EntityTypeBuilder>?>? action = null)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes();
        if (entityTypes == null) throw new NullReferenceException(nameof(entityTypes));
        foreach (var entityType in entityTypes)
        {
            var type = entityType.ClrType;
            if (type == SharedType) continue;

            Action<EntityTypeBuilder>? buildAction = null;

            #region 继承自 排序(IOrder) 接口的要设置索引

            if (POrder.IsAssignableFrom(type))
            {
                // https://docs.microsoft.com/zh-cn/ef/core/modeling/sequences
                var sequenceOrderNumbers = $"{entityType.GetTableName() ?? type.Name}_OrderNumbers";
#pragma warning disable IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
                if (entityType.ClrType.GetProperty(IOrder.SequenceStartsAt, BindingFlags.Static | BindingFlags.Public)?.GetValue(null) is not long startValue) startValue = 1L;
#pragma warning restore IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
                modelBuilder.HasSequence(sequenceOrderNumbers)
                    .StartsAt(startValue)
                    .IncrementsBy(1);

                buildAction += p =>
                {
                    p.Property(nameof(IOrder.Order)).HasDefaultValueSql($"NEXT VALUE FOR {sequenceOrderNumbers}");
                    p.HasIndex(nameof(IOrder.Order));
                };

            }

            #endregion

            #region 继承自 软删除(IsSoftDeleted) 接口的要设置索引

            if (PSoftDeleted.IsAssignableFrom(type))
            {
                // https://docs.microsoft.com/zh-cn/ef/core/querying/filters
                buildAction += p =>
                {
                    p.HasIndex(nameof(ISoftDeleted.SoftDeleted));
                    p.HasQueryFilter(SoftDeletedQueryFilter(type));
                    softDeleted.Add(type);
                };
            }

            #endregion

            #region 继承自 创建时间(ICreationTime) 接口的要设置默认值使用数据库当前时间

            if (PCreationTime.IsAssignableFrom(type))
            {
                buildAction += p =>
                {
                    p.Property(nameof(ICreationTime.CreationTime)).HasDefaultValueSql(SqlConstants.SYSDATETIMEOFFSET).IsRequired();
                };
            }

            #endregion

            #region 继承自 更新时间(IUpdateTime) 接口的要设置默认值使用数据库当前时间与更新时间

            if (PUpdateTime.IsAssignableFrom(type))
            {
                buildAction += p =>
                {
                    p.Property(nameof(IUpdateTime.UpdateTime)).HasDefaultValueSql(SqlConstants.SYSDATETIMEOFFSET).IsRequired();
                    p.Property(nameof(IUpdateTime.UpdateTime)).ValueGeneratedOnAddOrUpdate().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
                };
            }

            #endregion

            if (PDisable.IsAssignableFrom(type))
            {
                buildAction += p =>
                {
                    p.Property(nameof(IDisable.Disable)).HasDefaultValue(false);
                };
            }

            if (action != null)
                action.Invoke(modelBuilder, entityType, type, buildAction);

            if (buildAction != null)
            {
                modelBuilder.Entity(type, p =>
                {
                    buildAction(p);
                });
            }
        }
        return entityTypes;
    }

    public static readonly Type POrder = typeof(IOrder);
    public static readonly Type PSoftDeleted = typeof(ISoftDeleted);
    public static readonly Type PCreationTime = typeof(ICreationTime);
    public static readonly Type PUpdateTime = typeof(IUpdateTime);
    public static readonly Type PDisable = typeof(IDisable);

    /// <summary>
    /// https://docs.microsoft.com/zh-cn/ef/core/modeling/shadow-properties#property-bag-entity-types
    /// </summary>
    public static readonly Type SharedType = typeof(Dictionary<string, object>);

    static readonly HashSet<Type> softDeleted = new();

    public static bool IsSoftDeleted(Type type) => softDeleted.Contains(type);

}