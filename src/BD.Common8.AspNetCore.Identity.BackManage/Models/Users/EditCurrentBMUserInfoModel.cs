namespace BD.Common8.AspNetCore.Models.Users;

/// <summary>
/// 编辑当前登录的后台用户信息模型类
/// </summary>
public partial class EditCurrentBMUserInfoModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string UserName { get; set; } = "";
}