namespace BD.Common8.AspNetCore.Models.Abstractions;

/// <summary>
/// 用于 AppSettings 中配置是否使用 Nginx 反向代理
/// </summary>
public partial interface INotUseForwardedHeaders
{
    /// <summary>
    /// 不启用反向代理
    /// </summary>
    bool NotUseForwardedHeaders { get; set; }
}