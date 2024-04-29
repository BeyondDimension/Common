namespace BD.Common8.AspNetCore.Models.Users;

/// <summary>
/// 当前登录的后台用户信息模型类
/// </summary>
public partial class CurrentBMUserInfoModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string UserName { get; set; } = "";

    /// <summary>
    /// 权限角色集合
    /// </summary>
    public IList<string>? Roles { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; } = "";

    /// <summary>
    /// 是否是管理员
    /// </summary>
    public bool IsAdministrator { get; set; }

    /// <summary>
    /// 菜单列表
    /// </summary>
    public BMMenuModel[]? Menus { get; set; }

    /// <summary>
    /// 租户 Id
    /// </summary>
    public Guid TenantId { get; set; }
}