// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

public class ChangePasswordRequestDTO
{
    [Required]
    public string OldPassword { get; set; } = "";

    [Required]
    public string NewPassword { get; set; } = "";
}
