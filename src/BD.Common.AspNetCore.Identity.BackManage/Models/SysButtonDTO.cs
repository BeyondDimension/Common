namespace BD.Common.Models;

/// <inheritdoc cref="SysButton"/>
public sealed class SysButtonDTO : KeyModel<Guid>
{
    /// <inheritdoc cref="SysButton.Name"/>
    public string Name { get; set; } = "";

    /// <inheritdoc cref="SysButton.Type"/>
    public SysButtonType Type { get; set; }

#if !BLAZOR
    public static readonly Expression<Func<SysButton, SysButtonDTO>> Expression = it => new()
    {
        Id = it.Id,
        Name = it.Name,
        Type = it.Type,
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
        return entity;
    }
}
#endif
