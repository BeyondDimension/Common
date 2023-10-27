namespace System.Net;

/// <summary>
/// 本地 Clash 代理
/// </summary>
public sealed class LocalClashWebProxy : WebProxy
{
    /// <summary>
    /// 代理地址 + 端口号
    /// </summary>
    public const string AddressWithPort = $"{AddressString}:{PortString}";

    /// <summary>
    /// 代理地址
    /// </summary>
    public const string AddressString = "socks5://127.0.0.1";

    /// <summary>
    /// 端口号 int
    /// </summary>
    public const int Port = 7890;

    /// <summary>
    /// 端口号 string
    /// </summary>
    public const string PortString = "7890";

    /// <summary>
    /// 使用默认的地址和端口号创建本地 Clash 代理
    /// </summary>
    public LocalClashWebProxy() : base(AddressString, Port) { }
}
