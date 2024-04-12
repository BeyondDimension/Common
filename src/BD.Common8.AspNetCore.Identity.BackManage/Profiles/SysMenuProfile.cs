namespace BD.Common8.AspNetCore.Profiles;

/// <summary>
/// 用于系统菜单的 AutoMapperProfile
/// </summary>
public sealed class SysMenuProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SysMenuProfile"/> class.
    /// </summary>
    public SysMenuProfile()
    {
        CreateMap<SysMenu, SysMenuTreeItem>();
    }
}