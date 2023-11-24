namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// <see cref="SysButton"/> 模型类
/// </summary>
public sealed class SysButtonModel : KeyModel<Guid>
{
    /// <inheritdoc cref="SysButton.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysButton.Type"/>
    public SysButtonType Type { get; set; }

    /// <inheritdoc cref="SysButton.Style"/>
    public SysButtonStyle Style { get; set; }

    /// <inheritdoc cref="SysButton.Disable"/>
    public bool Disable { get; set; }

    /// <summary>
    /// 表达式用于将 <see cref="SysButton"/> 对象转换为 <see cref="SysButtonModel"/> 对象
    /// </summary>
    public static readonly Expression<Func<SysButton, SysButtonModel>> Expression = it => new()
    {
        Id = it.Id,
        Name = it.Name,
        Type = it.Type,
        Style = it.Style,
        Disable = it.Disable,
    };
}

/// <summary>
/// 提供对实体类 <see cref="SysButton"/> 的扩展方法
/// </summary>
public static partial class EntitiesExtensions
{
    /// <summary>
    /// 将 <see cref="SysButtonModel"/> 对象的属性值赋给 <see cref="SysButton"/> 对象
    /// </summary>
    /// <returns>赋值后的 <see cref="SysButton"/> 对象</returns>
    public static SysButton SetValue(
       this SysButton entity,
       SysButtonModel model)
    {
        entity.Name = model.Name;
        entity.Type = model.Type;
        entity.Style = model.Style;
        entity.Disable = model.Disable;
        return entity;
    }
}
