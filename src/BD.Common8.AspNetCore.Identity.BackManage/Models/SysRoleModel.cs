namespace BD.Common8.AspNetCore.Models;

#pragma warning disable SA1600 // Elements should be documented

public sealed partial class SysRoleModel : KeyModel<Guid>
{
    public string? Name { get; set; }

#if !BLAZOR
    public static readonly Expression<Func<SysRole, SysRoleModel>> Expression = x => new()
    {
        Id = x.Id,
        Name = x.Name,
    };
#endif
}

public sealed class AddOrEditSysRoleModel : IValidatableObject
{
    public string? Name { get; set; }

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("角色名称不能为空或空白字符", new[] { nameof(Name) });
    }
}