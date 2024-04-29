namespace BD.Common8.AspNetCore.Models.Users;

/// <summary>
/// 新增后台用户模型类
/// </summary>
public partial class AddBMUserModel : IAddOrEditBMUserModel
{
    /// <inheritdoc/>
    [Required]
    public string UserName { get; set; } = "";

    /// <summary>
    /// 获取或设置密码，第一次输入
    /// </summary>
    [Required]
    public string Password1 { get; set; } = "";

    /// <summary>
    /// 获取或设置密码，第二次输入
    /// </summary>
    [Required]
    public string Password2 { get; set; } = "";

    /// <inheritdoc/>
    public IList<string>? Roles { get; set; }
}