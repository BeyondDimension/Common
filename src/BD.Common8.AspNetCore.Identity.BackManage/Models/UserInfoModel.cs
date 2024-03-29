namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// 后台用户，查询表格模型类
/// </summary>
public partial class UserInfoModel : KeyModel<Guid>
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
    /// 用于将 <see cref="SysUser"/> 实体映射为 <see cref="UserInfoModel"/> 实例
    /// </summary>
    public static readonly Expression<Func<SysUser, UserInfoModel>> Expression = x => new()
    {
        Id = x.Id,
        UserName = x.UserName,
    };
}

/// <summary>
/// 当前登录的后台用户信息模型类
/// </summary>
public partial class CurrentUserInfoModel
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
    public SysMenuModel[]? Menus { get; set; }

    /// <summary>
    /// 租户 Id
    /// </summary>
    public Guid TenantId { get; set; }
}

/// <summary>
/// 编辑当前登录的后台用户信息模型类
/// </summary>
public partial class EditCurrentUserInfoModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string UserName { get; set; } = "";
}

/// <summary>
/// 添加或编辑后台用户模型
/// </summary>
public partial interface IAddOrEditUserModel
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

/// <summary>
/// 新增后台用户模型类
/// </summary>
public partial class AddBMUserModel : IAddOrEditUserModel
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

/// <summary>
/// 编辑后台用户模型类
/// </summary>
public partial class EditUserModel : IAddOrEditUserModel
{
    /// <inheritdoc/>
    [Required]
    public string UserName { get; set; } = "";

    /// <inheritdoc/>
    public IList<string>? Roles { get; set; }

    /// <summary>
    /// 将用户信息模型 <see cref="UserInfoModel"/> 转换为 <see cref="IAddOrEditUserModel"/>
    /// </summary>
    public static IAddOrEditUserModel Parse(UserInfoModel value)
    {
        IAddOrEditUserModel result = new EditUserModel
        {
            UserName = value.UserName,
            Roles = value.Roles == null ? null : new List<string>(value.Roles),
        };
        return result;
    }
}