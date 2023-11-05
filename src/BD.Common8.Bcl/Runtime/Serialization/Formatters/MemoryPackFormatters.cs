namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// MemoryPack 格式器
/// </summary>
public sealed class MemoryPackFormatters : IMemoryPackFormatterRegister
{
    /// <summary>
    /// 注册完成时的事件委托
    /// </summary>
    public static Action<Type>? OnRegister { get; set; }

    /// <summary>
    /// 注册指定类型的格式化器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Register<T>(MemoryPackFormatter<T> formatter)
    {
        MemoryPackFormatterProvider.Register(formatter);
        OnRegister?.Invoke(typeof(T));
    }

    /// <summary>
    /// 实现了 <see cref="IMemoryPackFormatterRegister"/> 接口，用于注册格式化器
    /// </summary>
    static void IMemoryPackFormatterRegister.RegisterFormatter()
    {
        Register(CookieFormatterAttribute.Formatter.Default);
        Register(CookieCollectionFormatterAttribute.Formatter.Default);
        Register(CookieContainerFormatterAttribute.Formatter.Default);

        Register(IPAddressFormatterAttribute.Formatter.Default);

        Register(RSAParametersFormatterAttribute.Formatter.Default);

        Register(ColorFormatterAttribute.Formatter.Default);
        //Register(SplatColorFormatterAttribute.Formatter.Default);
        Register(NullableColorFormatterAttribute.Formatter.Default);
        //Register(NullableSplatColorFormatterAttribute.Formatter.Default);

        Register(X509CertificateFormatterAttribute.Formatter.Default);
        Register(X509Certificate2FormatterAttribute.Formatter.Default);
        Register(X509CertificatePackableNullableFormatterAttribute.Formatter.Default);
        Register(X509CertificatePackableFormatterAttribute.Formatter.Default);

        Register(ProcessStartInfoFormatterAttribute.Formatter.Default);
        Register(ProcessStartInfoPackableNullableFormatterAttribute.Formatter.Default);
        Register(ProcessStartInfoPackableFormatterAttribute.Formatter.Default);
    }
}
