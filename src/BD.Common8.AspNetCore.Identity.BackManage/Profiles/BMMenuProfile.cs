namespace BD.Common8.AspNetCore.Profiles;

/// <summary>
/// 用于系统菜单的 AutoMapperProfile
/// </summary>
public sealed class BMMenuProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BMMenuProfile"/> class.
    /// </summary>
    public BMMenuProfile()
    {
        CreateMap<BMMenu, BMMenuTreeItem>();
    }
}