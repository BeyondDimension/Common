namespace BD.Common8.AspNetCore.Models.Users;

public sealed class BMUserTableItem
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string[]? Roles { get; set; }

    public bool LockoutEnabled { get; set; }
}
