namespace BD.Common8.SourceGenerator.Repositories.Enums;

public enum EntityBaseClassType : byte
{
    TenantBaseEntity = 1,
    OperatorBaseEntity,
    CreationBaseEntity,
    Entity,
}

public static partial class EntityBaseClassTypeEnumExtensions
{
    static readonly ImmutableDictionary<EntityBaseClassType, ImmutableArray<FixedProperty>> dictionary
        = new Dictionary<EntityBaseClassType, ImmutableArray<FixedProperty>>
        {
            {
                EntityBaseClassType.TenantBaseEntity, new FixedProperty[]
            {
                FixedProperty.TenantId,
                FixedProperty.SoftDeleted,
                FixedProperty.UpdateTime,
                FixedProperty.OperatorUserId,
                FixedProperty.CreationTime,
                FixedProperty.CreateUserId,
                FixedProperty.Id,
            }.ToImmutableArray()
            },
            {
                EntityBaseClassType.OperatorBaseEntity, new FixedProperty[]
            {
                FixedProperty.UpdateTime,
                FixedProperty.OperatorUserId,
                FixedProperty.CreationTime,
                FixedProperty.CreateUserId,
                FixedProperty.Id,
            }.ToImmutableArray()
            },
            {
                EntityBaseClassType.CreationBaseEntity, new FixedProperty[]
            {
                FixedProperty.CreationTime,
                FixedProperty.CreateUserId,
                FixedProperty.Id,
            }.ToImmutableArray()
            },
            {
                EntityBaseClassType.Entity, new FixedProperty[]
            {
                FixedProperty.Id,
            }.ToImmutableArray()
            },
        }.ToImmutableDictionary();

    public static EntityBaseClassType GetEntityBaseClassType(this HashSet<FixedProperty> fixedProperties)
    {
        foreach (var item in dictionary)
            if (item.Value.All(fixedProperties.Contains))
                return item.Key;
        return default;
    }

    public static bool IsBaseProperty(this EntityBaseClassType baseClassType, FixedProperty fixedProperty)
    {
        if (dictionary.TryGetValue(baseClassType, out var value))
            return value.Contains(fixedProperty);
        return false;
    }
}
