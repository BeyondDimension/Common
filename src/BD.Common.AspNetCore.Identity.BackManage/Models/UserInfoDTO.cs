// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <summary>
/// 后台用户，查询表格
/// </summary>
public partial class UserInfoDTO : KeyModel<Guid>
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

#if !BLAZOR
    public static readonly Expression<Func<SysUser, UserInfoDTO>> Expression = x => new()
    {
        Id = x.Id,
        UserName = x.UserName,
    };
#endif
}

/// <summary>
/// 当前登录的后台用户信息
/// </summary>
public partial class CurrentUserInfoDTO
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

    public string Avatar { get; set; } = "";

    public bool IsAdministrator { get; set; }

    public SysMenuDTO[]? Menus { get; set; }

    public Guid TenantId { get; set; }
}

/// <summary>
/// 编辑当前登录的后台用户信息
/// </summary>
public partial class EditCurrentUserInfoDTO
{
    [Required]
    public string UserName { get; set; } = "";
}

public partial interface IAddOrEditUserDTO
{
    [Required]
    string UserName { get; set; }

    IList<string>? Roles { get; set; }
}

/// <summary>
/// 新增后台用户表单提交
/// </summary>
public partial class AddBMUserDTO : IAddOrEditUserDTO
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
/// 编辑后台用户表单提交
/// </summary>
public partial class EditUserDTO : IAddOrEditUserDTO
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

    public static IAddOrEditUserDTO Parse(UserInfoDTO value)
    {
        IAddOrEditUserDTO result = new EditUserDTO
        {
            UserName = value.UserName,
            Roles = value.Roles == null ? null : new List<string>(value.Roles),
        };
        return result;
    }
}