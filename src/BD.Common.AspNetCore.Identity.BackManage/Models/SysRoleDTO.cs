namespace BD.Common.Models;

public sealed partial class SysRoleDTO : KeyModel<Guid>
{
    public string? Name { get; set; }

#if !BLAZOR
    public static readonly Expression<Func<SysRole, SysRoleDTO>> Expression = x => new()
    {
        Id = x.Id,
        Name = x.Name,
    };
#endif
}

public sealed class AddOrEditSysRoleDTO : IValidatableObject
{
    public string? Name { get; set; }

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("角色名称不能为空或空白字符", new[] { nameof(Name) });
    }
}