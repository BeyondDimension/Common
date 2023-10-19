namespace BD.Common8.Orm.EFCore.Extensions;

public static partial class EntityPropertyBuilderExtensions
{
    /// <inheritdoc cref="SqlServerPropertyBuilderExtensions.UseHiLo{TProperty}(PropertyBuilder{TProperty}, string?, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyBuilder<TProperty> UseHiLo2<TProperty>(this PropertyBuilder<TProperty> propertyBuilder,
        string? name = null,
        string? schema = null) => SqlConstants.DatabaseProvider switch
        {
            SqlConstants.SqlServer => SqlServerPropertyBuilderExtensions.UseHiLo(propertyBuilder, name, schema),
            SqlConstants.PostgreSQL => NpgsqlPropertyBuilderExtensions.UseHiLo(propertyBuilder, name, schema),
            _ => throw ThrowHelper.GetArgumentOutOfRangeException(SqlConstants.DatabaseProvider),
        };
}
