namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// <see cref="SysTenant"/> 模型类
/// </summary>
public sealed partial class SysTenantModel : KeyModel<Guid>
{
    /// <inheritdoc cref="SysTenant.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysTenant.Contact"/>
    public string? Contact { get; set; }

    /// <inheritdoc cref="SysTenant.ContactPhoneNumber"/>
    public string? ContactPhoneNumber { get; set; }

    /// <inheritdoc cref="SysTenant.Address"/>
    public string? Address { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterEmail"/>
    public string? RegisterEmail { get; set; }

    /// <inheritdoc cref="ICreationTime.CreationTime"/>
    public DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc cref="IUpdateTime.UpdateTime"/>
    public DateTimeOffset UpdateTime { get; set; }

    /// <inheritdoc cref="IOperatorUser.OperatorUser"/>
    public string? OperatorUser { get; set; }

    /// <inheritdoc cref="ICreateUser.CreateUser"/>
    public string? CreateUser { get; set; }

#if !BLAZOR
    /// <summary>
    /// 将操作用户和创建用户关联到查询中
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<SysTenant> Includes(IQueryable<SysTenant> query)
      => query
        .Include(x => x.OperatorUser)
        .Include(x => x.CreateUser);

    /// <summary>
    /// 表达式用于将 <see cref="SysTenant"/> 转换为 <see cref="SysTenantModel"/>
    /// </summary>
    public static readonly Expression<Func<SysTenant, SysTenantModel>> Expression = x => new()
    {
        Id = x.Id,
        OperatorUser = x.OperatorUser == null ? null : x.OperatorUser.UserName,
        CreateUser = x.CreateUser == null ? "" : x.CreateUser.UserName,
        CreationTime = x.CreationTime,
        UpdateTime = x.UpdateTime,

        Name = x.Name,
        Contact = x.Contact,
        ContactPhoneNumber = x.ContactPhoneNumber,
        Address = x.Address,
        RegisterEmail = x.RegisterEmail,
    };
#endif
}

/// <summary>
/// 添加或编辑 <see cref="SysTenant"/> 租户数据模型基类
/// </summary>
public abstract partial class AddOrEditSysTenantModelBase : IKeyModel<Guid>, IValidatableObject
{
    /// <summary>
    /// 租户 Id
    /// </summary>
    public Guid TenantId { get; set; }

    /// <inheritdoc cref="SysTenant.Name"/>
    public string TenantName { get; set; } = "";

    /// <inheritdoc cref="SysTenant.Contact"/>
    public string? Contact { get; set; }

    /// <inheritdoc cref="SysTenant.ContactPhoneNumber"/>
    public string? ContactPhoneNumber { get; set; }

    /// <inheritdoc cref="SysTenant.Address"/>
    public string? Address { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterEmail"/>
    public string? RegisterEmail { get; set; }

    /// <inheritdoc cref="TenantId"/>
    Guid IKeyModel<Guid>.Id { get => TenantId; set => TenantId = value; }

    /// <summary>
    /// 验证模型的有效性
    /// </summary>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(TenantName))
            yield return new ValidationResult("请输入租户名称", new[] { nameof(TenantName) });
    }
}

/// <summary>
/// 添加 <see cref="SysTenant"/> 租户数据模型
/// </summary>
public sealed partial class AddSysTenantModel : AddOrEditSysTenantModelBase
{
    /// <summary>
    /// 获取或设置租户管理员用户名
    /// </summary>
    public string? AdminUserName { get; set; }

    /// <summary>
    /// 获取或设置租户管理员密码，第一次输入
    /// </summary>
    public string? AdminPassword1 { get; set; }

    /// <summary>
    /// 获取或设置租户管理员密码，第二次输入
    /// </summary>
    public string? AdminPassword2 { get; set; }

    /// <summary>
    /// 验证添加租户模型的属性值是否符合规范
    /// </summary>
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

    /// <summary>
    /// 重写基类的验证方法，用于调用 <see cref="ValidateCore"/> 核心验证方法
    /// </summary>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return ValidateCore().Concat(base.Validate(validationContext));
    }
}

/// <summary>
/// 编辑租户模型类
/// </summary>
public sealed partial class EditSysTenantModel : AddOrEditSysTenantModelBase
{
}

#region 新租户实体

/// <summary>
/// <see cref="SysTenant"/> 租户添加或编辑模型类
/// </summary>
public sealed class AddOrEditSysTenantModel : KeyModel<Guid>, IValidatableObject
{
    /// <inheritdoc cref="SysTenant.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysTenant.UniqueCode"/>
    public string? UniqueCode { get; set; }

    /// <inheritdoc cref="SysTenant.Contact"/>
    public string? ContactName { get; set; }

    /// <inheritdoc cref="SysTenant.ContactPhoneNumber"/>
    public string? ContactPhone { get; set; }

    /// <inheritdoc cref="SysTenant.Address"/>
    public string? Address { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterEmail"/>
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

    /// <inheritdoc cref="SysTenant.Disable"/>
    public bool Disable { get; set; }

    /// <inheritdoc cref="SysTenant.Remarks"/>
    public string? Remarks { get; set; }

    /// <inheritdoc cref="SysTenant.IsPlatformAdministrator"/>
    public bool IsPlatformAdministrator { get; set; }

    /// <summary>
    /// 验证模型有效性
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("请输入租户名称", new[] { nameof(Name) });
    }
}

/// <summary>
/// <see cref="SysTenant"/> 租户查询表格项模型类
/// </summary>
public sealed class SysTenantTableItem : KeyModel<Guid>
{
    /// <inheritdoc cref="SysTenant.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysTenant.UniqueCode"/>
    public string? UniqueCode { get; set; }

    /// <inheritdoc cref="SysTenant.Contact"/>
    public string? ContactName { get; set; }

    /// <inheritdoc cref="SysTenant.ContactPhoneNumber"/>
    public string? ContactPhone { get; set; }

    /// <inheritdoc cref="SysTenant.Address"/>
    public string? Address { get; set; }

    /// <inheritdoc cref="SysTenant.RegisterEmail"/>
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

    /// <inheritdoc cref="SysTenant.Disable"/>
    public bool Disable { get; set; }

    /// <inheritdoc cref="SysTenant.Remarks"/>
    public string? Remarks { get; set; }

    /// <inheritdoc cref="SysTenant.IsPlatformAdministrator"/>
    public bool IsPlatformAdministrator { get; set; }

    /// <inheritdoc cref="CreationBaseEntity{T}.CreationTime"/>
    public DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc cref="CreationBaseEntity{T}.CreateUser"/>
    public string? CreateUserName { get; set; }

    /// <inheritdoc cref="OperatorBaseEntity{T}.UpdateTime"/>
    public DateTimeOffset UpdateTime { get; set; }

    /// <inheritdoc cref="OperatorBaseEntity{T}.OperatorUser"/>
    public string? OperatorUserName { get; set; }
}

#endregion

/// <summary>
/// 提供对实体类 <see cref="SysTenant"/> 的扩展方法
/// </summary>
public static partial class EntitiesExtensions
{
    /// <summary>
    /// 将 <see cref="AddOrEditSysTenantModelBase"/> 对象的属性值赋给 <see cref="SysTenant"/> 对象
    /// </summary>
    public static SysTenant SetValue(
        this SysTenant entity,
        AddOrEditSysTenantModelBase model)
    {
        entity.Name = model.TenantName;
        entity.Contact = model.Contact;
        entity.ContactPhoneNumber = model.ContactPhoneNumber;
        entity.Address = model.Address;
        entity.RegisterEmail = model.RegisterEmail;
        return entity;
    }
}

/// <summary>
/// 提供对模型类 <see cref="AddOrEditSysTenantModelBase"/> 的扩展方法
/// </summary>
public static partial class ModelExtensions
{
    /// <summary>
    /// 将 <see cref="SysTenantModel"/> 对象的属性值赋给 <see cref="AddOrEditSysTenantModelBase"/> 对象
    /// </summary>
    public static void SetValue(
       this AddOrEditSysTenantModelBase entity,
       SysTenantModel? model)
    {
        entity.TenantId = model?.Id ?? default;
        entity.TenantName = model?.Name ?? "";
        entity.Contact = model?.Contact ?? "";
        entity.ContactPhoneNumber = model?.ContactPhoneNumber ?? "";
        entity.Address = model?.Address ?? "";
        entity.RegisterEmail = model?.RegisterEmail ?? "";
    }
}