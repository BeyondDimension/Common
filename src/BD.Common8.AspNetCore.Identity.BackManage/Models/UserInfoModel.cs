namespace BD.Common8.AspNetCore.Models;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 后台用户，查询表格模型类
/// </summary>
public partial class UserInfoModel : KeyModel<Guid>
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

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
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

    public string Avatar { get; set; } = "";

    public bool IsAdministrator { get; set; }

    public SysMenuModel[]? Menus { get; set; }

    public Guid TenantId { get; set; }
}

/// <summary>
/// 编辑当前登录的后台用户信息模型类
/// </summary>
public partial class EditCurrentUserInfoModel
{
    [Required]
    public string UserName { get; set; } = "";
}

public partial interface IAddOrEditUserModel
{
    [Required]
    string UserName { get; set; }

    IList<string>? Roles { get; set; }
}

/// <summary>
/// 新增后台用户表单提交模型类
/// </summary>
public partial class AddBMUserModel : IAddOrEditUserModel
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password1 { get; set; } = "";

    [Required]
    public string Password2 { get; set; } = "";

    public IList<string>? Roles { get; set; }
}

/// <summary>
/// 编辑后台用户表单提交模型类
/// </summary>
public partial class EditUserModel : IAddOrEditUserModel
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

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