namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 权限控制相关实体类 - 用户组织架构关系
/// </summary>
[Table("BM_UserOrganizations")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public class BMUserOrganization : ITenant
{
    /// <inheritdoc/>
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 组织架构 Id
    /// </summary>
    [Comment("组织架构 Id")]
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    [Comment("用户 Id")]
    public Guid UserId { get; set; }

    /// <inheritdoc cref="BMOrganization"/>
    public virtual BMOrganization? Organization { get; set; }

    /// <inheritdoc cref="BMUser"/>
    public virtual BMUser? User { get; set; }

    /// <inheritdoc cref="IEntityTypeConfiguration{SysUserOrganization}"/>
    public sealed class EntityTypeConfiguration : IEntityTypeConfiguration<BMUserOrganization>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<BMUserOrganization> builder)
        {
            builder.HasKey(x => new { x.UserId, x.OrganizationId, x.TenantId });
            builder.HasOne(x => x.User)
                .WithMany(x => x.Organizations)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Organization)
                .WithMany(x => x.UserOrganizations)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}