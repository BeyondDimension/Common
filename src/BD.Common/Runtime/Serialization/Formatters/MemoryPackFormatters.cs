namespace System.Runtime.Serialization.Formatters;

public sealed class MemoryPackFormatters : IMemoryPackFormatterRegister
{
    public static Action<Type>? OnRegister { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Register<T>(MemoryPackFormatter<T> formatter)
    {
        MemoryPackFormatterProvider.Register(formatter);
        OnRegister?.Invoke(typeof(T));
    }

    static void IMemoryPackFormatterRegister.RegisterFormatter()
    {
        Register(CookieFormatterAttribute.Formatter.Default);
        Register(CookieCollectionFormatterAttribute.Formatter.Default);
        Register(CookieContainerFormatterAttribute.Formatter.Default);

        Register(IPAddressFormatterAttribute.Formatter.Default);

        Register(RSAParametersFormatterAttribute.Formatter.Default);

        Register(ColorFormatterAttribute.Formatter.Default);
        Register(SplatColorFormatterAttribute.Formatter.Default);
        Register(NullableColorFormatterAttribute.Formatter.Default);
        Register(NullableSplatColorFormatterAttribute.Formatter.Default);

        Register(X509CertificateFormatterAttribute.Formatter.Default);
        Register(X509Certificate2FormatterAttribute.Formatter.Default);
        Register(X509CertificatePackableNullableFormatterAttribute.Formatter.Default);
        Register(X509CertificatePackableFormatterAttribute.Formatter.Default);
    }
}
