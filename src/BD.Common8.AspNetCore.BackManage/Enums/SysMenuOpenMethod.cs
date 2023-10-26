namespace BD.Common8.AspNetCore.Enums;

/// <summary>
/// 系统菜单打开方式
/// </summary>
public enum SysMenuOpenMethod : byte
{
    /// <summary>
    /// 正常，在页面中打开。
    /// </summary>
    Normal = 0,

    /// <summary>
    /// 打开链接
    /// </summary>
    OpenLink = 1,
}