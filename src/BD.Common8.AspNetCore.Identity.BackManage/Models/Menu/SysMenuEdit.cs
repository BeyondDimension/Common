namespace BD.Common8.AspNetCore.Models;

public sealed class SysMenuEdit : IKeyModel<Guid>
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? IconUrl { get; set; }

    public string Key { get; set; } = string.Empty;

    public long Order { get; set; }

    public string? Remarks { get; set; }

    public string Url { get; set; } = string.Empty;

    public bool SoftDeleted { get; set; }
}