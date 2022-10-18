// ReSharper disable once CheckNamespace
namespace System;

public static partial class Browser2
{
    public const string Prefix_HTTPS = "https://";

    public const string Prefix_HTTP = "http://";

    public const string Prefix_MSStore = "ms-windows-store://";

    public const string Prefix_Email = "mailto:";

    public const string Prefix_File = "file:///";

    /// <summary>
    /// 判断字符串是否为 Http Url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="httpsOnly">是否仅Https</param>
    /// <returns></returns>
    public static bool IsHttpUrl([NotNullWhen(true)] string? url, bool httpsOnly = false) => url != null &&
        (url.StartsWith(Prefix_HTTPS, StringComparison.OrdinalIgnoreCase) ||
              (!httpsOnly && url.StartsWith(Prefix_HTTP, StringComparison.OrdinalIgnoreCase)));
}
