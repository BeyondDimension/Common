using System.Security.Cryptography.X509Certificates;

// ReSharper disable once CheckNamespace
namespace System;

public static class X509CertificateExtensions
{
    static byte[] GetCertHashCompatImpl(this X509Certificate certificate, HashAlgorithmName hashAlgorithm)
    {
        // https://github.com/dotnet/runtime/blob/v6.0.4/src/libraries/System.Security.Cryptography.X509Certificates/src/System/Security/Cryptography/X509Certificates/X509Certificate.cs#L362
        using IncrementalHash hasher = IncrementalHash.CreateHash(hashAlgorithm);
        hasher.AppendData(certificate.GetRawCertData());
        return hasher.GetHashAndReset();
    }

    public static byte[] GetCertHashCompat(this X509Certificate certificate, HashAlgorithmName hashAlgorithm)
    {
        try
        {
            return certificate.GetCertHash(hashAlgorithm);
        }
        catch
        {
            return certificate.GetCertHashCompatImpl(hashAlgorithm);
        }
    }

    public static string GetCertHashStringCompat(this X509Certificate certificate, HashAlgorithmName hashAlgorithm)
    {
        try
        {
            return certificate.GetCertHashString(hashAlgorithm);
        }
        catch
        {
            // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Security.Cryptography/src/System/Security/Cryptography/X509Certificates/X509Certificate.cs#L408
            return certificate.GetCertHashCompatImpl(hashAlgorithm).ToHexString();
        }
    }

    const string BEGIN_CERTIFICATE_SIGIL = "-----BEGIN CERTIFICATE-----";
    const string END_CERTIFICATE_SIGIL = "-----END CERTIFICATE-----";

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

    //[Obsolete("warn AppContext.BaseDirectory ?", true)]
    //public static string GetPrivatePemCertificateString(this X509Certificate2 @this)
    //{
    //    if (@this.PrivateKey is RSACryptoServiceProvider pkey)
    //    {
    //        var keyPair = DotNetUtilities.GetRsaKeyPair(pkey);
    //        using TextWriter tw = new StreamWriter(Path.Combine(IOPath.BaseDirectory, @this.IssuerName.Name + ".key"), false, EncodingCache.UTF8NoBOM);
    //        PemWriter pw = new PemWriter(tw);
    //        pw.WriteObject(keyPair.Private);
    //        tw.Flush();
    //        return tw.ToString();
    //    }
    //    else
    //    {
    //        throw new InvalidCastException(nameof(X509Certificate2.PrivateKey));
    //    }
    //}

    public static IReadOnlyList<string> GetSubjectAlternativeNames(this X509Certificate2 certificate)
    {
        foreach (X509Extension extension in certificate.Extensions)
        {
            // Create an AsnEncodedData object using the extensions information.
            if (string.Equals(extension.Oid?.FriendlyName, "Subject Alternative Name"))
            {
                var asndata = new AsnEncodedData(extension.Oid, extension.RawData);
                return asndata.Format(true).Split(new string[] {
                    Environment.NewLine,
                    "DNS Name=",
                }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        return Array.Empty<string>();
    }

    public static void SaveCerCertificateFile(this X509Certificate2 certificate, string pathOrName)
    {
        var value = GetPublicPemCertificateString(certificate);
        File.WriteAllText(pathOrName, value, EncodingCache.UTF8NoBOM);
    }
}
