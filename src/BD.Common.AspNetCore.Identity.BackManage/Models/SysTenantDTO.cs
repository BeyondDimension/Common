namespace BD.Common.Models;

/// <summary>
/// 租户表数据 DTO 模型
/// </summary>
public sealed partial class SysTenantDTO : KeyModel<Guid>
{
    public string Name { get; set; } = "";

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset CreationTime { get; set; }

    public DateTimeOffset UpdateTime { get; set; }

    public string? OperatorUser { get; set; }

    public string CreateUser { get; set; } = "";

#if !BLAZOR
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<SysTenant> Includes(IQueryable<SysTenant> query)
      => query
        .Include(x => x.OperatorUser)
        .Include(x => x.CreateUser);

    public static readonly Expression<Func<SysTenant, SysTenantDTO>> Expression = x => new()
    {
        Id = x.Id,
        OperatorUser = x.OperatorUser == null ? null : x.OperatorUser.UserName,
        CreateUser = x.CreateUser == null ? "" : x.CreateUser.UserName,
        CreationTime = x.CreationTime,
        UpdateTime = x.UpdateTime,

        Name = x.Name,
        ContactName = x.ContactName,
        ContactPhone = x.ContactPhone,
        Address = x.Address,
        Email = x.Email,
    };
#endif
}

/// <summary>
/// 添加或编辑租户数据基类
/// </summary>
public abstract partial class AddOrEditSysTenantDTOBase : IKeyModel<Guid>, IValidatableObject
{
    public Guid TenantId { get; set; }

    public string TenantName { get; set; } = "";

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    Guid IKeyModel<Guid>.Id { get => TenantId; set => TenantId = value; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(TenantName))
            yield return new ValidationResult("请输入租户名称", new[] { nameof(TenantName) });
    }
}

/// <summary>
/// 添加租户模型类
/// </summary>
public sealed partial class AddSysTenantDTO : AddOrEditSysTenantDTOBase
{
    public string? AdminUserName { get; set; }

    public string? AdminPassword1 { get; set; }

    public string? AdminPassword2 { get; set; }

    IEnumerable<ValidationResult> ValidateCore()
    {
        if (string.IsNullOrWhiteSpace(AdminUserName))
            yield return new ValidationResult("请输入租户管理员用户名", new[] { nameof(AdminUserName) });
        if (string.IsNullOrWhiteSpace(AdminPassword1))
            yield return new ValidationResult("请输入租户管理员密码", new[] { nameof(AdminPassword1) });
        if (string.IsNullOrWhiteSpace(AdminPassword2))
            yield return new ValidationResult("请输入租户管理员密码", new[] { nameof(AdminPassword2) });
        if (AdminPassword1 != AdminPassword2)
            yield return new ValidationResult("两次输入的租户管理员密码不一致", new[] { nameof(AdminPassword2) });
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return ValidateCore().Concat(base.Validate(validationContext));
    }
}

/// <summary>
/// 编辑租户模型类
/// </summary>
public sealed partial class EditSysTenantDTO : AddOrEditSysTenantDTOBase
{

}

#region 新租户实体

/// <summary>
/// 租户添加或编辑模型类
/// </summary>
public sealed class AddOrEditSysTenantDTO : KeyModel<Guid>, IValidatableObject
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
    public bool Disable { get; set; }

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
/// 租户查询表格项模型类
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
    public bool Disable { get; set; }

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

#endregion
#if !BLAZOR
public static partial class EntitiesExtensions
{
    public static SysTenant SetValue(
        this SysTenant entity,
        AddOrEditSysTenantDTOBase model)
    {
        entity.Name = model.TenantName;
        entity.ContactName = model.ContactName;
        entity.ContactPhone = model.ContactPhone;
        entity.Address = model.Address;
        entity.Email = model.Email;
        return entity;
    }
}
#else
public static partial class DTOExtensions
{
    public static void SetValue(
       this AddOrEditSysTenantDTOBase entity,
       SysTenantDTO? model)
    {
        entity.TenantId = model?.Id ?? default;
        entity.TenantName = model?.Name ?? "";
        entity.ContactName = model?.ContactName ?? "";
        entity.ContactPhone = model?.ContactPhone ?? "";
        entity.Address = model?.Address ?? "";
        entity.Email = model?.Email ?? "";
    }
}

#endif