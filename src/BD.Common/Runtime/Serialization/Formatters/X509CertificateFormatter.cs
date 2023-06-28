namespace System.Runtime.Serialization.Formatters;

public sealed class X509CertificateFormatter :
    IMemoryPackFormatter<X509Certificate?>,
    IMemoryPackFormatter<X509Certificate2?>,
    IMemoryPackFormatter<X509CertificatePackable>,
    IMemoryPackFormatter<X509CertificatePackable?>
{
    public static readonly X509CertificateFormatter Default = new();

    void IMemoryPackFormatter<X509Certificate?>.Deserialize(ref MemoryPackReader reader, scoped ref X509Certificate? value)
    {
        if (reader.PeekIsNull())
        {
            value = null;
        }
        else
        {
            value = reader.ReadPackable<X509CertificatePackable>();
        }
    }

    void IMemoryPackFormatter<X509Certificate2?>.Deserialize(ref MemoryPackReader reader, scoped ref X509Certificate2? value)
    {
        if (reader.PeekIsNull())
        {
            value = null;
        }
        else
        {
            value = reader.ReadPackable<X509CertificatePackable>();
        }
    }

    void IMemoryPackFormatter<X509CertificatePackable>.Deserialize(ref MemoryPackReader reader, scoped ref X509CertificatePackable value)
    {
        if (reader.PeekIsNull())
        {
            value = default;
        }
        else
        {
            value = reader.ReadPackable<X509CertificatePackable>();
        }
    }

    void IMemoryPackFormatter<X509CertificatePackable?>.Deserialize(ref MemoryPackReader reader, scoped ref X509CertificatePackable? value)
    {
        if (reader.PeekIsNull())
        {
            value = default;
        }
        else
        {
            value = reader.ReadPackable<X509CertificatePackable>();
        }
    }

    void IMemoryPackFormatter<X509Certificate?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509Certificate? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
        }
        else
        {
            var data = value.GetRawCertData();
            X509CertificatePackable packable = X509CertificatePackable.CreateX509Certificate(data);
            writer.WritePackable(packable);
        }
    }

    void IMemoryPackFormatter<X509Certificate2?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509Certificate2? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
        }
        else
        {
            var data = value.GetRawCertData();
            X509CertificatePackable packable = X509CertificatePackable.CreateX509Certificate2(data);
            writer.WritePackable(packable);
        }
    }

    void IMemoryPackFormatter<X509CertificatePackable>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509CertificatePackable value)
    {
        writer.WritePackable(value);
    }

    void IMemoryPackFormatter<X509CertificatePackable?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509CertificatePackable? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
        }
        else
        {
            writer.WritePackable(value.Value);
        }
    }
}

[MemoryPackable]
public partial struct X509CertificatePackable : IDisposable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate(string fileName, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate, Ctor_FileName, null, fileName.ThrowIsNull(), password, keyStorageFlags);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate2(string fileName, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate2, Ctor_FileName, null, fileName.ThrowIsNull(), password, keyStorageFlags);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate(byte[] data, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate, Ctor_ByteArray, data.ThrowIsNull(), null, password, keyStorageFlags);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate2(byte[] data, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate2, Ctor_ByteArray, data.ThrowIsNull(), null, password, keyStorageFlags);

    [MemoryPackIgnore]
    Lazy<X509Certificate?>? X509Certificate { get; set; }

    [MemoryPackIgnore]
    Lazy<X509Certificate2?>? X509Certificate2 { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public X509Certificate? GetX509Certificate()
    {
        X509Certificate ??= new(GetX509CertificateImpl);
        return X509Certificate.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public X509Certificate2? GetX509Certificate2()
    {
        switch (Type)
        {
            case Type_X509Certificate2:
                if (X509Certificate2 != null)
                    return X509Certificate2.Value;
                if (X509Certificate != null)
                {
                    if (X509Certificate.Value is X509Certificate2 certificate2)
                    {
                        X509Certificate2 = new(certificate2);
                        return certificate2;
                    }
                }
                X509Certificate2 = new(GetX509Certificate2Impl);
                break;
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly X509Certificate? GetX509CertificateImpl()
    {
        switch (Type)
        {
            case Type_X509Certificate:
                switch (Constructor)
                {
                    case Ctor_ByteArray:
                        if (Data != null)
                        {
                            if (Password == null)
                            {
                                return new X509Certificate(Data);
                            }
                            else
                            {
                                if (KeyStorageFlags.HasValue)
                                {
                                    return new X509Certificate(Data, Password, KeyStorageFlags.Value);
                                }
                                else
                                {
                                    return new X509Certificate(Data, Password);
                                }
                            }
                        }
                        break;
                    case Ctor_FileName:
                        if (FileName != null)
                        {
                            if (Password == null)
                            {
                                return new X509Certificate(FileName);
                            }
                            else
                            {
                                if (KeyStorageFlags.HasValue)
                                {
                                    return new X509Certificate(FileName, Password, KeyStorageFlags.Value);
                                }
                                else
                                {
                                    return new X509Certificate(FileName, Password);
                                }
                            }
                        }
                        break;
                }
                break;
            case Type_X509Certificate2:
                return GetX509Certificate2Impl();
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly X509Certificate2? GetX509Certificate2Impl()
    {
        switch (Constructor)
        {
            case Ctor_ByteArray:
                if (Data != null)
                {
                    if (Password == null)
                    {
                        return new X509Certificate2(Data);
                    }
                    else
                    {
                        if (KeyStorageFlags.HasValue)
                        {
                            return new X509Certificate2(Data, Password, KeyStorageFlags.Value);
                        }
                        else
                        {
                            return new X509Certificate2(Data, Password);
                        }
                    }
                }
                break;
            case Ctor_FileName:
                if (FileName != null)
                {
                    if (Password == null)
                    {
                        return new X509Certificate2(FileName);
                    }
                    else
                    {
                        if (KeyStorageFlags.HasValue)
                        {
                            return new X509Certificate2(FileName, Password, KeyStorageFlags.Value);
                        }
                        else
                        {
                            return new X509Certificate2(FileName, Password);
                        }
                    }
                }
                break;
        }
        return null;
    }

    public void Dispose()
    {
        if (X509Certificate != null && X509Certificate.IsValueCreated)
        {
            X509Certificate.Value?.Dispose();
            X509Certificate = null;
        }
        if (X509Certificate2 != null && X509Certificate2.IsValueCreated)
        {
            X509Certificate2.Value?.Dispose();
            X509Certificate2 = null;
        }
    }

    public const byte Type_X509Certificate = 1;
    public const byte Type_X509Certificate2 = 2;

    public const byte Ctor_ByteArray = 1;
    public const byte Ctor_FileName = 2;

    [MemoryPackConstructor]
    X509CertificatePackable(byte type, byte constructor, byte[]? data, string? fileName, string? password, X509KeyStorageFlags? keyStorageFlags)
    {
        Type = type;
        Constructor = constructor;
        Data = data;
        FileName = fileName;
        Password = password;
        KeyStorageFlags = keyStorageFlags;
    }

    [MemoryPackInclude]
    byte Type { get; }

    [MemoryPackInclude]
    byte Constructor { get; }

    [MemoryPackInclude]
    byte[]? Data { get; }

    [MemoryPackInclude]
    string? FileName { get; }

    [MemoryPackInclude]
    string? Password { get; }

    [MemoryPackInclude]
    X509KeyStorageFlags? KeyStorageFlags { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator X509Certificate?(X509CertificatePackable value) => value.GetX509Certificate();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator X509Certificate2?(X509CertificatePackable value) => value.GetX509Certificate2();
}

public sealed class X509CertificateFormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509Certificate?>
{
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<X509Certificate?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509Certificate? value)
        {
            IMemoryPackFormatter<X509Certificate?> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509Certificate? value)
        {
            IMemoryPackFormatter<X509Certificate?> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class X509Certificate2FormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509Certificate2?>
{
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<X509Certificate2?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509Certificate2? value)
        {
            IMemoryPackFormatter<X509Certificate2?> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509Certificate2? value)
        {
            IMemoryPackFormatter<X509Certificate2?> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class X509CertificatePackableNullableFormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509CertificatePackable?>
{
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<X509CertificatePackable?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509CertificatePackable? value)
        {
            IMemoryPackFormatter<X509CertificatePackable?> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509CertificatePackable? value)
        {
            IMemoryPackFormatter<X509CertificatePackable?> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class X509CertificatePackableFormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509CertificatePackable>
{
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<X509CertificatePackable>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509CertificatePackable value)
        {
            IMemoryPackFormatter<X509CertificatePackable> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509CertificatePackable value)
        {
            IMemoryPackFormatter<X509CertificatePackable> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}
