namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// <see cref="BMButton"/> 模型类
/// </summary>
public sealed class BMButtonModel : KeyModel<Guid>
{
    /// <inheritdoc cref="BMButton.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="BMButton.Type"/>
    public SysButtonType Type { get; set; }

    /// <inheritdoc cref="BMButton.Style"/>
    public SysButtonStyle Style { get; set; }

    /// <inheritdoc cref="BMButton.Disable"/>
    public bool Disable { get; set; }

    /// <summary>
    /// 表达式用于将 <see cref="BMButton"/> 对象转换为 <see cref="BMButtonModel"/> 对象
    /// </summary>
    public static readonly Expression<Func<BMButton, BMButtonModel>> Expression = it => new()
    {
        Id = it.Id,
        Name = it.Name,
        Type = it.Type,
        Style = it.Style,
        Disable = it.Disable,
    };
}

/// <summary>
/// 提供对实体类 <see cref="BMButton"/> 的扩展方法
/// </summary>
public static partial class EntitiesExtensions
{
    /// <summary>
    /// 将 <see cref="BMButtonModel"/> 对象的属性值赋给 <see cref="BMButton"/> 对象
    /// </summary>
    /// <returns>赋值后的 <see cref="BMButton"/> 对象</returns>
    public static BMButton SetValue(
       this BMButton entity,
       BMButtonModel model)
    {
        entity.Name = model.Name;
        entity.Type = model.Type;
        entity.Style = model.Style;
        entity.Disable = model.Disable;
        return entity;
    }
}
