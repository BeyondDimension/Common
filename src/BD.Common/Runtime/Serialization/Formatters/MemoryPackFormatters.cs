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
#if !COMMON_NO_SPLAT_DRAWING
        Register(SplatColorFormatterAttribute.Formatter.Default);
#endif
        Register(NullableColorFormatterAttribute.Formatter.Default);
#if !COMMON_NO_SPLAT_DRAWING
        Register(NullableSplatColorFormatterAttribute.Formatter.Default);
#endif

        Register(X509CertificateFormatterAttribute.Formatter.Default);
        Register(X509Certificate2FormatterAttribute.Formatter.Default);
        Register(X509CertificatePackableNullableFormatterAttribute.Formatter.Default);
        Register(X509CertificatePackableFormatterAttribute.Formatter.Default);

        Register(ProcessStartInfoFormatterAttribute.Formatter.Default);
        Register(ProcessStartInfoPackableNullableFormatterAttribute.Formatter.Default);
        Register(ProcessStartInfoPackableFormatterAttribute.Formatter.Default);
    }
}
