namespace BD.Common8.AspNetCore.Models.Abstractions;

/// <summary>
/// 用于 appsettings.json | appsettings.Development.json 文件的设置项中配置是否使用 Nginx 反向代理
/// </summary>
public partial interface INotUseForwardedHeaders
{
    /// <summary>
    /// 不启用反向代理
    /// </summary>
    bool NotUseForwardedHeaders { get; }

    /// <summary>
    /// 配置 <see cref="ForwardedHeadersOptions.KnownProxies"/>，使用英文分号分隔
    /// </summary>
    string? ForwardedHeadersKnownProxies { get; }

    /// <summary>
    /// 获取用于 <see cref="ForwardedHeadersOptions.KnownProxies"/> 的 IP 地址列表
    /// </summary>
    /// <returns></returns>
    List<IPAddress> GetForwardedHeadersKnownProxies() => GetForwardedHeadersKnownProxies(ForwardedHeadersKnownProxies);

    static List<IPAddress> GetForwardedHeadersKnownProxies(string? knownProxies)
    {
        if (string.IsNullOrWhiteSpace(knownProxies))
        {
            return [IPAddress.Parse("::ffff:172.18.0.1")];
        }
        else
        {
            return new(knownProxies.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(static x => IPAddress2.TryParse(x, out var v) ? v : null!)
                .Where(static x => x != null));
        }
    }
}