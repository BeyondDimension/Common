namespace BD.Common8.AspNetCore.Models;

#pragma warning disable SA1600 // Elements should be documented

/// <inheritdoc cref="SysButton"/>
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

    public static readonly Expression<Func<SysButton, SysButtonModel>> Expression = it => new()
    {
        Id = it.Id,
        Name = it.Name,
        Type = it.Type,
        Style = it.Style,
        Disable = it.Disable,
    };
}

public static partial class EntitiesExtensions
{
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
