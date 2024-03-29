namespace BD.Common8.UnitTest.Models;

[MP2Obj(MP2SerializeLayout.Explicit)]
public sealed partial class X509CertificateModel
{
    [MP2Key(0)]
    [X509CertificateFormatter]
    public X509Certificate? X509Certificate { get; set; }

    [MP2Key(1)]
    [X509Certificate2Formatter]
    public X509Certificate2? X509Certificate2 { get; set; }

    [MP2Key(2)]
    [X509CertificatePackableNullableFormatter]
    public X509CertificatePackable? NullableX509CertificatePackable { get; set; }

    [MP2Key(3)]
    [X509CertificatePackableNullableFormatter]
    public X509CertificatePackable? NullableX509CertificatePackable2 { get; set; }

    [MP2Key(4)]
    [X509CertificatePackableFormatter]
    public X509CertificatePackable X509CertificatePackable { get; set; }
}