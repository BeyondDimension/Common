namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// <see cref="BMRole"/> 模型类
/// </summary>
public sealed partial class BMRoleModel : KeyModel<Guid>
{
    /// <inheritdoc cref="BMRole.Name"/>
    public string? Name { get; set; }

#if !BLAZOR
    /// <summary>
    /// 表达式用于将 <see cref="BMRole"/> 对象转换为 <see cref="BMRoleModel"/> 对象
    /// </summary>
    public static readonly Expression<Func<BMRole, BMRoleModel>> Expression = x => new()
    {
        Id = x.Id,
        Name = x.Name,
    };
#endif
}

/// <summary>
/// 添加或编辑 <see cref="BMRole"/> 模型类
/// </summary>
public sealed class AddOrEditSysRoleModel : IValidatableObject
{
    /// <inheritdoc cref="BMRole.Name"/>
    public string? Name { get; set; }

    /// <summary>
    /// 验证模型的有效性
    /// </summary>
    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("角色名称不能为空或空白字符", new[] { nameof(Name) });
    }
}