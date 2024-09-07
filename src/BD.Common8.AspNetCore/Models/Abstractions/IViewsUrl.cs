namespace BD.Common8.AspNetCore.Models.Abstractions;

/// <summary>
/// 用于 appsettings.json | appsettings.Development.json 文件的设置项中配置允许跨域访问的 Web UI 地址
/// </summary>
public partial interface IViewsUrl
{
    /// <summary>
    /// 配置允许跨域访问的 Web UI 地址
    /// </summary>
    string? ViewsUrl { get; }

    /// <summary>
    /// 是否使用跨域访问，调用了 AddCors
    /// </summary>
    bool UseCors { set; }
}