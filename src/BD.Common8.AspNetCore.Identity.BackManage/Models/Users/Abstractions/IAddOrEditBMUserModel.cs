namespace BD.Common8.AspNetCore.Models.Users.Abstractions;

/// <summary>
/// 添加或编辑后台用户模型
/// </summary>
public partial interface IAddOrEditBMUserModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    string UserName { get; set; }

    /// <summary>
    /// 权限角色集合
    /// </summary>
    IList<string>? Roles { get; set; }
}