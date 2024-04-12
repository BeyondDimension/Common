namespace BD.Common8.AspNetCore.Models;

public class SysMenuTreeItem
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SysMenuTreeItem>? Children { get; set; }
}
