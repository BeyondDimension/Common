namespace BD.Common8.AspNetCore.Models.Users;

/// <summary>
/// 后台用户，查询表格模型类
/// </summary>
public partial class BMUserInfoModel : KeyModel<Guid>
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
    /// 用于将 <see cref="BMUser"/> 实体映射为 <see cref="BMUserInfoModel"/> 实例
    /// </summary>
    public static readonly Expression<Func<BMUser, BMUserInfoModel>> Expression = x => new()
    {
        Id = x.Id,
        UserName = x.UserName,
    };
}