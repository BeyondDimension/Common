namespace System.Security.Cryptography.X509Certificates;

public static partial class X509Certificate2Generator // 常量
{
    /// <summary>
    /// 用于 <see cref="RSA.Create(int)"/> 的默认值。
    /// </summary>
    public const int DefaultRSAKeySizeInBits = 2048;

    /// <summary>
    /// 用于 <see cref="X509BasicConstraintsExtension(bool, bool, int, bool)"/> 的默认值。
    /// </summary>
    public const int DefaultPathLengthConstraint = 1;

    /// <summary>
    /// 证书有效期时间，太长会导致浏览器不受信任，默认为 300 天。
    /// </summary>
    public const ushort CertificateValidDays = 300;
}