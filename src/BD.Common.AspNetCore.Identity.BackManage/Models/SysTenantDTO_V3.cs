// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <summary>
/// 添加或编辑租户DTO
/// </summary>
public sealed class AddOrEditSysTenantDTO_V3 : KeyModel<Guid>, IValidatableObject
{
    /// <inheritdoc cref="SysTenant.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysTenant.UniqueCode"/>
    public string? UniqueCode { get; set; }

    /// <inheritdoc cref="SysTenant.ContactName"/>
    public string? ContactName { get; set; }

    /// <inheritdoc cref="SysTenant.ContactPhone"/>
    public string? ContactPhone { get; set; }

    /// <inheritdoc cref="SysTenant.Address"/>
    public string? Address { get; set; }

    /// <inheritdoc cref="SysTenant.Email"/>
    public string? Email { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterPhoneNumber"/>
    public string? RegisterPhoneNumber { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterEmail"/>
    public string? RegisterEmail { get; set; }

    /// <inheritdoc cref="SysTenant.AuditorId"/>
    public Guid? AuditorId { get; set; }

    /// <inheritdoc cref="SysTenant.Auditor"/>
    public string? Auditor { get; set; }

    /// <inheritdoc cref="SysTenant.AuditTime"/>
    public DateTimeOffset? AuditTime { get; set; }

    /// <inheritdoc cref="SysTenant.AuditStatus"/>
    public SysTenantAuditStatus? AuditStatus { get; set; }

    /// <inheritdoc cref="SysTenant.AuditRemarks"/>
    public string? AuditRemarks { get; set; }

    /// <inheritdoc cref="SysTenant.AuthorizationStartTime"/>
    public DateTimeOffset AuthorizationStartTime { get; set; }

    /// <inheritdoc cref="SysTenant.AuthorizationEndTime"/>
    public DateTimeOffset AuthorizationEndTime { get; set; }

    /// <inheritdoc cref="SysTenant.Status"/>
    public SysTenantStatus Status { get; set; }

    /// <inheritdoc cref="SysTenant.Remarks"/>
    public string? Remarks { get; set; }

    /// <inheritdoc cref="SysTenant.IsPlatformAdministrator"/>
    public bool IsPlatformAdministrator { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("请输入租户名称", new[] { nameof(Name) });
    }
}

/// <summary>
/// 查询租户表格项DTO
/// </summary>
public sealed class SysTenantTableItem : KeyModel<Guid>
{
    /// <inheritdoc cref="SysTenant.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysTenant.UniqueCode"/>
    public string? UniqueCode { get; set; }

    /// <inheritdoc cref="SysTenant.ContactName"/>
    public string? ContactName { get; set; }

    /// <inheritdoc cref="SysTenant.ContactPhone"/>
    public string? ContactPhone { get; set; }

    /// <inheritdoc cref="SysTenant.Address"/>
    public string? Address { get; set; }

    /// <inheritdoc cref="SysTenant.Email"/>
    public string? Email { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterPhoneNumber"/>
    public string? RegisterPhoneNumber { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterEmail"/>
    public string? RegisterEmail { get; set; }

    /// <inheritdoc cref="SysTenant.AuditorId"/>
    public Guid? AuditorId { get; set; }

    /// <inheritdoc cref="SysTenant.Auditor"/>
    public string? Auditor { get; set; }

    /// <inheritdoc cref="SysTenant.AuditTime"/>
    public DateTimeOffset? AuditTime { get; set; }

    /// <inheritdoc cref="SysTenant.AuditStatus"/>
    public SysTenantAuditStatus? AuditStatus { get; set; }

    /// <inheritdoc cref="SysTenant.AuditRemarks"/>
    public string? AuditRemarks { get; set; }

    /// <inheritdoc cref="SysTenant.AuthorizationStartTime"/>
    public DateTimeOffset AuthorizationStartTime { get; set; }

    /// <inheritdoc cref="SysTenant.AuthorizationEndTime"/>
    public DateTimeOffset AuthorizationEndTime { get; set; }

    /// <inheritdoc cref="SysTenant.Status"/>
    public SysTenantStatus Status { get; set; }

    /// <inheritdoc cref="SysTenant.Remarks"/>
    public string? Remarks { get; set; }

    /// <inheritdoc cref="SysTenant.IsPlatformAdministrator"/>
    public bool IsPlatformAdministrator { get; set; }

    /// <inheritdoc cref="CreationBaseEntityV2{T}.CreationTime"/>
    public DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc cref="CreationBaseEntityV2{T}.CreateUser"/>
    public string? CreateUserName { get; set; }

    /// <inheritdoc cref="OperatorBaseEntityV2{T}.UpdateTime"/>
    public DateTimeOffset UpdateTime { get; set; }

    /// <inheritdoc cref="OperatorBaseEntityV2{T}.OperatorUser"/>
    public string? OperatorUserName { get; set; }
}