namespace BD.Common.Models;

/// <inheritdoc cref="SysButton"/>
public sealed class SysButtonDTO : KeyModel<Guid>
{
    /// <inheritdoc cref="SysButton.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysButton.Type"/>
    public SysButtonType Type { get; set; }

    /// <inheritdoc cref="SysButton.Style"/>
    public SysButtonStyle Style { get; set; }

    /// <inheritdoc cref="SysButton.Disable"/>
    public bool Disable { get; set; }

#if !BLAZOR
    public static readonly Expression<Func<SysButton, SysButtonDTO>> Expression = it => new()
    {
        Id = it.Id,
        Name = it.Name,
        Type = it.Type,
        Style = it.Style,
        Disable = it.Disable,
    };
#endif
}

#if !BLAZOR
public static partial class EntitiesExtensions
{
    public static SysButton SetValue(
       this SysButton entity,
       SysButtonDTO model)
    {
        entity.Name = model.Name;
        entity.Type = model.Type;
        entity.Style = model.Style;
        entity.Disable = model.Disable;
        return entity;
    }
}
#endif
