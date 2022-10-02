// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <summary>
/// 后台用户，查询表格
/// </summary>
public partial class BMUserInfoDTO : KeyModel<Guid>
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

#if !BLAZOR
    public static readonly Expression<Func<BMUser, BMUserInfoDTO>> Expression = x => new()
    {
        Id = x.Id,
        UserName = x.UserName,
    };
#endif
}

/// <summary>
/// 当前登录的后台用户信息
/// </summary>
public partial class BMMeInfoDTO
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

    public string Avatar { get; set; } = "";

    public bool IsAdministrator { get; set; }
}

/// <summary>
/// 编辑当前登录的后台用户信息
/// </summary>
public partial class EditBMMeInfoDTO
{
    [Required]
    public string UserName { get; set; } = "";
}

public partial interface IAddOrEditBMUserDTO
{
    [Required]
    string UserName { get; set; }

    IList<string>? Roles { get; set; }

    bool IsAdministrator { get; set; }

    const bool DefaultIsAdministrator = true;

    void CalcRoles()
    {
        Roles = BMRoleEnumHelper.SetRole(Roles, nameof(BMRole.Administrator), IsAdministrator);
    }

    void AnalysisRoles()
    {
        IsAdministrator = BMRoleEnumHelper.IsRole(Roles, nameof(BMRole.Administrator));
    }
}

/// <summary>
/// 新增后台用户表单提交
/// </summary>
public partial class AddBMUserDTO : IAddOrEditBMUserDTO
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password1 { get; set; } = "";

    [Required]
    public string Password2 { get; set; } = "";

    public IList<string>? Roles { get; set; }

    [JsonIgnore]
    public bool IsAdministrator { get; set; } = IAddOrEditBMUserDTO.DefaultIsAdministrator;
}

public partial class EditBMUserDTO : IAddOrEditBMUserDTO
{
    [Required]
    public string UserName { get; set; } = "";

    public IList<string>? Roles { get; set; }

    [JsonIgnore]
    public bool IsAdministrator { get; set; } = IAddOrEditBMUserDTO.DefaultIsAdministrator;

    public static IAddOrEditBMUserDTO Parse(BMUserInfoDTO value)
    {
        IAddOrEditBMUserDTO result = new EditBMUserDTO
        {
            UserName = value.UserName,
            Roles = value.Roles,
        };
        result.AnalysisRoles();
        return result;
    }
}