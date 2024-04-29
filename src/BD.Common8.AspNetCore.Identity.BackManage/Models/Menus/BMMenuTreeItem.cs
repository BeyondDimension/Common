namespace BD.Common8.AspNetCore.Models.Menus;

public sealed class BMMenuTreeItem
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<BMMenuTreeItem>? Children { get; set; }
}
