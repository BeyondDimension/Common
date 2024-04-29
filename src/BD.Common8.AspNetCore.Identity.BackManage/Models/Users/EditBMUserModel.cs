namespace BD.Common8.AspNetCore.Models.Users;

/// <summary>
/// 编辑后台用户模型类
/// </summary>
public partial class EditBMUserModel : IAddOrEditBMUserModel
{
    /// <inheritdoc/>
    [Required]
    public string UserName { get; set; } = "";

    /// <inheritdoc/>
    public IList<string>? Roles { get; set; }

    /// <summary>
    /// 将用户信息模型 <see cref="BMUserInfoModel"/> 转换为 <see cref="IAddOrEditBMUserModel"/>
    /// </summary>
    public static IAddOrEditBMUserModel Parse(BMUserInfoModel value)
    {
        IAddOrEditBMUserModel result = new EditBMUserModel
        {
            UserName = value.UserName,
            Roles = value.Roles == null ? null : new List<string>(value.Roles),
        };
        return result;
    }
}