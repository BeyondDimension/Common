// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <summary>
/// 请求模型 - 更改当前登录用户密码
/// </summary>
public class ChangePasswordRequestDTO
{
    /// <summary>
    /// 旧密码
    /// </summary>
    [Required]
    public string OldPassword { get; set; } = "";

    /// <summary>
    /// 新密码
    /// </summary>
    [Required]
    public string NewPassword { get; set; } = "";
}
