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
/// 添加租户的 DTO 模型
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
/// 编辑租户的 DTO 模型
/// </summary>
public sealed partial class EditSysTenantDTO : AddOrEditSysTenantDTOBase
{

}

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