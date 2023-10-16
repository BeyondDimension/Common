#pragma warning disable SA1600 // Elements should be documented

namespace System.Extensions;

public static partial class X509CertificateExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte[] GetCertHashCompatImpl(this X509Certificate certificate, HashAlgorithmName hashAlgorithm)
    {
        // https://github.com/dotnet/runtime/blob/v6.0.4/src/libraries/System.Security.Cryptography.X509Certificates/src/System/Security/Cryptography/X509Certificates/X509Certificate.cs#L362
        using IncrementalHash hasher = IncrementalHash.CreateHash(hashAlgorithm);
        hasher.AppendData(certificate.GetRawCertData());
        return hasher.GetHashAndReset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetCertHashCompat(this X509Certificate certificate, HashAlgorithmName hashAlgorithm)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NET48_OR_GREATER
        try
        {
            return certificate.GetCertHash(hashAlgorithm);
        }
        catch
        {
            return certificate.GetCertHashCompatImpl(hashAlgorithm);
        }
#else
        return certificate.GetCertHashCompatImpl(hashAlgorithm);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetCertHashStringCompat(this X509Certificate certificate, HashAlgorithmName hashAlgorithm)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NET48_OR_GREATER
        try
        {
            return certificate.GetCertHashString(hashAlgorithm);
        }
        catch
        {
            // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Security.Cryptography/src/System/Security/Cryptography/X509Certificates/X509Certificate.cs#L408
            return certificate.GetCertHashCompatImpl(hashAlgorithm).ToHexString();
        }
#else
        return certificate.GetCertHashCompatImpl(hashAlgorithm).ToHexString();
#endif
    }

    const string BEGIN_CERTIFICATE_SIGIL = "-----BEGIN CERTIFICATE-----";
    const string END_CERTIFICATE_SIGIL = "-----END CERTIFICATE-----";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetPublicPemCertificateString(this X509Certificate2 certificate)
    {
        var value = certificate.Export(X509ContentType.Cert);
        var valueStr = Convert.ToBase64String(value, Base64FormattingOptions.InsertLineBreaks);
        StringBuilder builder = new();
        builder.AppendLine(BEGIN_CERTIFICATE_SIGIL);
        builder.AppendLine(valueStr);
        builder.AppendLine(END_CERTIFICATE_SIGIL);
        return builder.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<string> GetSubjectAlternativeNames(this X509Certificate2 certificate)
    {
        foreach (X509Extension extension in certificate.Extensions)
            // Create an AsnEncodedData object using the extensions information.
            if (string.Equals(extension.Oid?.FriendlyName, "Subject Alternative Name"))
            {
                var asndata = new AsnEncodedData(extension.Oid, extension.RawData);
                return asndata.Format(true).Split(new string[] {
                    Environment.NewLine,
                    "DNS Name=",
                }, StringSplitOptions.RemoveEmptyEntries);
            }
        return Array.Empty<string>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveCerCertificateFile(this X509Certificate2 certificate, string pathOrName)
    {
        var value = certificate.GetPublicPemCertificateString();
        File.WriteAllText(pathOrName, value, Encoding2.UTF8NoBOM);
    }
}
