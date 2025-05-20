using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;

namespace BD.Common8.UnitTest.Models;

[global::MemoryPack.MemoryPackable(global::MemoryPack.SerializeLayout.Explicit)]
public sealed partial class X509CertificateModel
{
    [global::MemoryPack.MemoryPackOrder(0)]
    [X509CertificateFormatter]
    public X509Certificate? X509Certificate { get; set; }

    [global::MemoryPack.MemoryPackOrder(1)]
    [X509Certificate2Formatter]
    public X509Certificate2? X509Certificate2 { get; set; }

    [global::MemoryPack.MemoryPackOrder(2)]
    [X509CertificatePackableNullableFormatter]
    public X509CertificatePackable? NullableX509CertificatePackable { get; set; }

    [global::MemoryPack.MemoryPackOrder(3)]
    [X509CertificatePackableNullableFormatter]
    public X509CertificatePackable? NullableX509CertificatePackable2 { get; set; }

    [global::MemoryPack.MemoryPackOrder(4)]
    [X509CertificatePackableFormatter]
    public X509CertificatePackable X509CertificatePackable { get; set; }
}