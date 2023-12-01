namespace BD.Common8.Columns;

/// <inheritdoc cref="TenantId"/>
public interface ITenant
{
    /// <summary>
    /// 租户 Id
    /// </summary>
    Guid TenantId { get; set; }
}
