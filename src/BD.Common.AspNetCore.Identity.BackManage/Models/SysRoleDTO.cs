namespace BD.Common.Models;

public sealed class SysRoleDTO : KeyModel<Guid>
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

public sealed class AddOrEditSysRoleDTO
{
    public string? Name { get; set; }
}