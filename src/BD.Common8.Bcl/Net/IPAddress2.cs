namespace System.Net;

/// <inheritdoc cref="IPAddress"/>
public static partial class IPAddress2
{
    /// <summary>
    /// 127.0.0.1
    /// </summary>
    public const string _127_0_0_1 = "127.0.0.1";

    /// <summary>
    /// localhost
    /// </summary>
    public const string localhost = "localhost";

    /// <summary>
    /// 尝试将 <see cref="string"/> 转换为 <see cref="IPAddress"/>，当返回值为 <see langword="true"/> 时，传入与传出的参数不为 <see langword="null"/>。
    /// </summary>
    /// <param name="ipString"></param>
    /// <param name="address"></param>
    /// <returns></returns>
    public static bool TryParse(
        [NotNullWhen(true)] string? ipString,
        [NotNullWhen(true)] out IPAddress? address)
    {
        if (ipString == _127_0_0_1 ||
            string.Equals(ipString, localhost, StringComparison.OrdinalIgnoreCase))
        {
            address = IPAddress.Loopback;
            return true;
        }
        else
        {
            return IPAddress.TryParse(ipString, out address);
        }
    }

    /// <summary>
    /// 将 <see cref="string"/> 转换为 <see cref="IPAddress"/>，当值为 <see langword="null"/> 或格式不正确时将抛出异常。
    /// </summary>
    /// <param name="ipString"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FormatException"></exception>
    public static IPAddress Parse(string ipString)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(ipString);
#else
        ipString = ipString ?? throw new ArgumentNullException(nameof(ipString));
#endif
        if (!TryParse(ipString, out var ip))
            throw new FormatException("ipString is not a valid IP address.");
        return ip;
    }

    /// <summary>
    /// 尝试将 <see cref="string"/> 转换为 <see cref="IPAddress"/>，如果转换失败返回值将为 <see langword="null"/>。
    /// </summary>
    /// <param name="ipString"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPAddress? ParseNullable(string? ipString)
        => TryParse(ipString, out var address) ? address : null;
}