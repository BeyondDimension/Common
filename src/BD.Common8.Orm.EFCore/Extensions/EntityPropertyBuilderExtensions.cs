using BD.Common8.Orm.EFCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.CompilerServices;

namespace BD.Common8.Orm.EFCore.Extensions;

public static partial class EntityPropertyBuilderExtensions
{
    /// <inheritdoc cref="NpgsqlPropertyBuilderExtensions.UseHiLo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyBuilder<TProperty> UseHiLo2<TProperty>(this PropertyBuilder<TProperty> propertyBuilder,
        string? name = null,
        string? schema = null) => SqlConstants.DatabaseProvider switch
        {
#if HAS_SQLSERVER
            SqlConstants.SqlServer => SqlServerPropertyBuilderExtensions.UseHiLo(propertyBuilder, name, schema),
#endif
            SqlConstants.PostgreSQL => NpgsqlPropertyBuilderExtensions.UseHiLo(propertyBuilder, name, schema),
            _ => throw ThrowHelper.GetArgumentOutOfRangeException(SqlConstants.DatabaseProvider),
        };
}
