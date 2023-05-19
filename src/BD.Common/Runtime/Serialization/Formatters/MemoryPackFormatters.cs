namespace System.Runtime.Serialization.Formatters;

public sealed class MemoryPackFormatters : IMemoryPackFormatterRegister
{
    static void IMemoryPackFormatterRegister.RegisterFormatter()
    {
        MemoryPackFormatterProvider.Register(CookieFormatterAttribute.Formatter.Default);
        MemoryPackFormatterProvider.Register(CookieCollectionFormatterAttribute.Formatter.Default);
        MemoryPackFormatterProvider.Register(CookieContainerFormatterAttribute.Formatter.Default);

        MemoryPackFormatterProvider.Register(IPAddressFormatterAttribute.Formatter.Default);

        MemoryPackFormatterProvider.Register(RSAParametersFormatterAttribute.Formatter.Default);

        MemoryPackFormatterProvider.Register(ColorFormatterAttribute.Formatter.Default);
        MemoryPackFormatterProvider.Register(SplatColorFormatterAttribute.Formatter.Default);
        MemoryPackFormatterProvider.Register(NullableColorFormatterAttribute.Formatter.Default);
        MemoryPackFormatterProvider.Register(NullableSplatColorFormatterAttribute.Formatter.Default);
    }
}
