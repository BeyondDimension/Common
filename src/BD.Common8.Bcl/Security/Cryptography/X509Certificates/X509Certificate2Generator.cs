namespace System.Security.Cryptography.X509Certificates;

/// <summary>
/// <see cref="X509Certificate2"/> 生成
/// </summary>
public static partial class X509Certificate2Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Oid GetTlsServerOid() => new("1.3.6.1.5.5.7.3.1");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Oid GetTlsClientOid() => new("1.3.6.1.5.5.7.3.2");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static X509AuthorityKeyIdentifierExtension GetAuthorityKeyIdentifierExtension(X509Certificate2 certificate)
    {
        var extension = new X509SubjectKeyIdentifierExtension(certificate.PublicKey, false);
        return X509AuthorityKeyIdentifierExtension.CreateFromSubjectKeyIdentifier(extension);
    }
#else
    static X509Extension GetAuthorityKeyIdentifierExtension(X509Certificate2 certificate)
    {
        var extension = new X509SubjectKeyIdentifierExtension(certificate.PublicKey, false);
        var subjectKeyIdentifier = extension.RawData.AsSpan(2);
        var rawData = new byte[subjectKeyIdentifier.Length + 4];
        rawData[0] = 0x30;
        rawData[1] = 0x16;
        rawData[2] = 0x80;
        rawData[3] = 0x14;
        subjectKeyIdentifier.CopyTo(rawData);
        return new X509Extension("2.5.29.35", rawData, false);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? GetSubjectAlternativeNameBySubjectName(string subjectName)
    {
        const string startsWithValue = "CN=";
        var query = from m in subjectName.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    let s = m.TrimStart()
                    where s.StartsWith(startsWithValue, StringComparison.OrdinalIgnoreCase)
                    select s;
        var subjectAlternativeName = query.FirstOrDefault()?[startsWithValue.Length..];
        return subjectAlternativeName;
    }

    /// <summary>
    /// DnsName 或 IpAddress
    /// </summary>
    public readonly record struct DnsNameOrIpAddress
    {
        /// <inheritdoc cref="Environment.MachineName"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DnsNameOrIpAddress GetMachineName()
            => new(Environment.MachineName, isDnsNameOrIpAddress: true);

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsNameOrIpAddress"/> class.
        /// </summary>
        /// <param name="iPAddress"></param>
        public DnsNameOrIpAddress(IPAddress iPAddress)
        {
            IPAddress = iPAddress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsNameOrIpAddress"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isDnsNameOrIpAddress"></param>
        public DnsNameOrIpAddress(string value, bool? isDnsNameOrIpAddress = null)
        {
            if (isDnsNameOrIpAddress.HasValue)
            {
                if (isDnsNameOrIpAddress.Value)
                {
                    DnsName = value;
                }
                else
                {
                    if (IPAddress2.TryParse(value, out var iPAddress))
                    {
                        IPAddress = iPAddress;
                    }
                    else
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(value);
                    }
                }
            }
            else
            {
                if (IPAddress2.TryParse(value, out var iPAddress))
                {
                    IPAddress = iPAddress;
                }
                else
                {
                    DnsName = value;
                }
            }
        }

        /// <inheritdoc cref="SubjectAlternativeNameBuilder.AddDnsName(string)"/>
        public readonly string? DnsName { get; }

        /// <inheritdoc cref="SubjectAlternativeNameBuilder.AddIpAddress(IPAddress)"/>
        public readonly IPAddress? IPAddress { get; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            if (DnsName != default)
                return DnsName;
            if (IPAddress != default)
                return IPAddress.ToString();
            return base.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DnsNameOrIpAddress(IPAddress iPAddress) => new(iPAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DnsNameOrIpAddress(string value) => new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AddDnsNameOrIpAddress(this SubjectAlternativeNameBuilder builder,
        DnsNameOrIpAddress value)
    {
        if (value.IPAddress != default)
        {
            builder.AddIpAddress(value.IPAddress);
        }
        else if (value.DnsName != default)
        {
            builder.AddDnsName(value.DnsName);
        }
    }

    /// <summary>
    /// 创建根证书 (CA) 配置选项
    /// </summary>
    public sealed partial class CreateRootCertificateOptions
    {
        /// <inheritdoc cref="X500DistinguishedName"/>
        public X500DistinguishedName? DistinguishedName { get; set; }

        /// <inheritdoc cref="X500DistinguishedName(string)"/>
        public string? DistinguishedNameString { get; set; }

        /// <inheritdoc cref="SubjectAlternativeNameBuilder"/>
        public string? SubjectAlternativeName { get; set; }

        /// <summary>
        /// 此证书被视为有效的最初日期和时间。 通常为 <see cref="DateTimeOffset.UtcNow"/>（可能有几秒钟的误差）。
        /// </summary>
        public DateTimeOffset NotBefore { get; set; }

        /// <summary>
        /// 此证书不再被视为有效的日期和时间。
        /// </summary>
        public DateTimeOffset NotAfter { get; set; }

        /// <inheritdoc cref="CertificateRequest.CreateSelfSigned(DateTimeOffset, DateTimeOffset)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal X509Certificate2 CreateSelfSigned(CertificateRequest certificateRequest)
        {
            DateTimeOffset notBefore = NotBefore;
            DateTimeOffset notAfter = NotAfter;
            if (notBefore == default)
            {
                notBefore = DateTimeOffset.UtcNow.AddMinutes(-1D);
            }
            if (notAfter == default)
            {
                notAfter = notBefore.AddDays(CertificateValidDays);
            }
            return certificateRequest.CreateSelfSigned(notBefore, notAfter);
        }

        /// <summary>
        /// 用于 <see cref="RSA.Create(int)"/> 的值。
        /// </summary>
        public int RSAKeySizeInBits { get; set; } = DefaultRSAKeySizeInBits;

        /// <summary>
        /// 自签名或通过 <see cref="X509Certificate2"/> 签名时要应用的 RSA 签名填充。
        /// </summary>
        public RSASignaturePadding RSAPadding { get; set; } = RSASignaturePadding.Pkcs1;

        /// <summary>
        /// 对证书或证书请求签名时要使用的哈希算法。
        /// </summary>
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        /// <summary>
        /// 用于 <see cref="X509BasicConstraintsExtension(bool, bool, int, bool)"/> 的值。
        /// </summary>
        public int PathLengthConstraint { get; set; } = DefaultPathLengthConstraint;
    }

    /// <summary>
    /// 创建根证书 (CA)
    /// </summary>
    /// <returns></returns>
    public static X509Certificate2 CreateRootCertificate(CreateRootCertificateOptions options)
    {
        X500DistinguishedName subjectName = options.DistinguishedName ??
            new(options.DistinguishedNameString.ThrowIsNull());
        using var rsa = RSA.Create(options.RSAKeySizeInBits);
        var request = new CertificateRequest(subjectName, rsa,
            options.HashAlgorithm, options.RSAPadding);

        var pathLengthConstraint = options.PathLengthConstraint;
        var hasPathLengthConstraint = pathLengthConstraint > 0;
        var basicConstraints = new X509BasicConstraintsExtension(true,
            hasPathLengthConstraint, pathLengthConstraint, true);
        request.CertificateExtensions.Add(basicConstraints);

        var keyUsages = X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign;
        var keyUsageEx = new X509KeyUsageExtension(keyUsages, true);
        request.CertificateExtensions.Add(keyUsageEx);

        var oids = new OidCollection { GetTlsServerOid(), GetTlsClientOid() };
        var enhancedKeyUsage = new X509EnhancedKeyUsageExtension(oids, true);
        request.CertificateExtensions.Add(enhancedKeyUsage);

        var subjectAlternativeName = options.SubjectAlternativeName;
        if (subjectAlternativeName == default)
        {
            subjectAlternativeName = GetSubjectAlternativeNameBySubjectName(subjectName.Name);
        }
        var subjectAlternativeNameBuilder = new SubjectAlternativeNameBuilder();
        subjectAlternativeNameBuilder.AddDnsNameOrIpAddress(subjectAlternativeName.ThrowIsNull());
        request.CertificateExtensions.Add(subjectAlternativeNameBuilder.Build());

        var subjectKeyId = new X509SubjectKeyIdentifierExtension(request.PublicKey, false);
        request.CertificateExtensions.Add(subjectKeyId);

        return options.CreateSelfSigned(request);
    }

    /// <summary>
    /// 创建服务器证书 (TLS/SSL) 配置选项
    /// </summary>
    public sealed partial class CreateServerCertificateOptions
    {
        /// <inheritdoc cref="X509Certificate2"/>
        public required X509Certificate2 RootCertificate { get; init; }

        /// <inheritdoc cref="X500DistinguishedName"/>
        public X500DistinguishedName? DistinguishedName { get; set; }

        /// <inheritdoc cref="X500DistinguishedName(string)"/>
        public string? DistinguishedNameString { get; set; }

        /// <inheritdoc cref="SubjectAlternativeNameBuilder"/>
        public string? SubjectAlternativeName { get; set; }

        /// <summary>
        /// 此证书被视为有效的最初日期和时间。 通常为 <see cref="DateTimeOffset.UtcNow"/>（可能有几秒钟的误差）。
        /// </summary>
        public DateTimeOffset NotBefore { get; set; }

        /// <summary>
        /// 此证书不再被视为有效的日期和时间。
        /// </summary>
        public DateTimeOffset NotAfter { get; set; }

        /// <summary>
        /// 额外的多个 <see cref="DnsNameOrIpAddress"/>
        /// </summary>
        public IEnumerable<DnsNameOrIpAddress>? ExtraDnsNameOrIpAddresses { get; set; }

        /// <summary>
        /// 用于 <see cref="RSA.Create(int)"/> 的值。
        /// </summary>
        public int RSAKeySizeInBits { get; set; } = DefaultRSAKeySizeInBits;

        /// <summary>
        /// 自签名或通过 <see cref="X509Certificate2"/> 签名时要应用的 RSA 签名填充。
        /// </summary>
        public RSASignaturePadding RSAPadding { get; set; } = RSASignaturePadding.Pkcs1;

        /// <summary>
        /// 对证书或证书请求签名时要使用的哈希算法。
        /// </summary>
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;
    }

    /// <summary>
    /// 创建服务器证书 (TLS/SSL)
    /// </summary>
    /// <returns></returns>
    public static X509Certificate2 CreateServerCertificate(CreateServerCertificateOptions options)
    {
        X500DistinguishedName subjectName = options.DistinguishedName ??
           new(options.DistinguishedNameString.ThrowIsNull());
        using var rsa = RSA.Create(options.RSAKeySizeInBits);
        var request = new CertificateRequest(subjectName, rsa,
            options.HashAlgorithm, options.RSAPadding);

        var basicConstraints = new X509BasicConstraintsExtension(false, false, 0, true);
        request.CertificateExtensions.Add(basicConstraints);

        var keyUsages = X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;
        var keyUsageEx = new X509KeyUsageExtension(keyUsages, true);
        request.CertificateExtensions.Add(keyUsageEx);

        var oids = new OidCollection { GetTlsServerOid(), GetTlsClientOid() };
        var enhancedKeyUsage = new X509EnhancedKeyUsageExtension(oids, true);
        request.CertificateExtensions.Add(enhancedKeyUsage);

        var authorityKeyId = GetAuthorityKeyIdentifierExtension(options.RootCertificate);
        request.CertificateExtensions.Add(authorityKeyId);

        var subjectKeyId = new X509SubjectKeyIdentifierExtension(request.PublicKey, false);
        request.CertificateExtensions.Add(subjectKeyId);

        var subjectAlternativeName = options.SubjectAlternativeName;
        if (subjectAlternativeName == default)
        {
            subjectAlternativeName = GetSubjectAlternativeNameBySubjectName(subjectName.Name);
        }
        var subjectAlternativeNameBuilder = new SubjectAlternativeNameBuilder();
        subjectAlternativeNameBuilder.AddDnsNameOrIpAddress(subjectAlternativeName.ThrowIsNull());

        if (options.ExtraDnsNameOrIpAddresses != null)
        {
            foreach (var item in options.ExtraDnsNameOrIpAddresses)
            {
                subjectAlternativeNameBuilder.AddDnsNameOrIpAddress(item);
            }
        }

        request.CertificateExtensions.Add(subjectAlternativeNameBuilder.Build());

#if NET6_0_OR_GREATER
        long randomInt64 = Random.Shared.NextInt64();
#else
        long randomInt64 = Random2.Shared().NextInt64();
#endif
        var serialNumber = BitConverter.GetBytes(randomInt64);
        DateTimeOffset notBefore = options.NotBefore;
        DateTimeOffset notAfter = options.NotAfter;
        if (notBefore == default)
            notBefore = options.RootCertificate.NotBefore;
        if (notAfter == default)
            notAfter = options.RootCertificate.NotAfter;
        using var serverCertificate = request.Create(options.RootCertificate,
            notBefore, notAfter, serialNumber); // 创建证书
        using var serverCertificateCopyWithPrivateKey = serverCertificate.CopyWithPrivateKey(rsa); // 将私钥复制进去
        var serverCertificateCopyWithPrivateKeyExport = serverCertificateCopyWithPrivateKey.Export(X509ContentType.Pfx); // 导出 Pfx 数据
        return new X509Certificate2(serverCertificateCopyWithPrivateKeyExport); // 重新创建证书，否则无法应用在 Kestrel 中
    }
}
