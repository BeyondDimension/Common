namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="X509Certificate"/>，<see cref="X509Certificate2"/> 的序列化与反序列化实现
/// </summary>
public sealed class X509CertificateFormatter :
    IMemoryPackFormatter<X509Certificate?>,
    IMemoryPackFormatter<X509Certificate2?>,
    IMemoryPackFormatter<X509CertificatePackable>,
    IMemoryPackFormatter<X509CertificatePackable?>
{
    /// <summary>
    /// 默认的 <see cref="X509CertificateFormatter"/> 实例
    /// </summary>
    public static readonly X509CertificateFormatter Default = new();

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    void IMemoryPackFormatter<X509CertificatePackable>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509CertificatePackable value)
    {
        writer.WritePackable(value);
    }

    /// <inheritdoc/>
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
    /// <inheritdoc cref="X509CertificatePackable(byte, byte, byte[], string? , string? , X509KeyStorageFlags?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate(string fileName, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate, Ctor_FileName, null, fileName.ThrowIsNull(), password, keyStorageFlags);

    /// <inheritdoc cref="X509CertificatePackable(byte, byte, byte[], string? , string? , X509KeyStorageFlags?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate2(string fileName, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate2, Ctor_FileName, null, fileName.ThrowIsNull(), password, keyStorageFlags);

    /// <inheritdoc cref="X509CertificatePackable(byte, byte, byte[], string? , string? , X509KeyStorageFlags?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate(byte[] data, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate, Ctor_ByteArray, data.ThrowIsNull(), null, password, keyStorageFlags);

    /// <inheritdoc cref="X509CertificatePackable(byte, byte, byte[], string? , string? , X509KeyStorageFlags?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static X509CertificatePackable CreateX509Certificate2(byte[] data, string? password = null, X509KeyStorageFlags? keyStorageFlags = null)
        => new(Type_X509Certificate2, Ctor_ByteArray, data.ThrowIsNull(), null, password, keyStorageFlags);

    [MemoryPackIgnore]
    Lazy<X509Certificate?>? X509Certificate { get; set; }

    [MemoryPackIgnore]
    Lazy<X509Certificate2?>? X509Certificate2 { get; set; }

    /// <summary>
    /// 获取 X509 证书
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public X509Certificate? GetX509Certificate()
    {
        X509Certificate ??= new(GetX509CertificateImpl);
        return X509Certificate.Value;
    }

    /// <summary>
    /// 获取 X509 证书2
    /// </summary>
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
                return X509Certificate2.Value;
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
#if NET9_0_OR_GREATER
                                return X509CertificateLoader.LoadCertificate(Data);
#else
                                return new X509Certificate(Data);
#endif
                            }
                            else
                            {
                                if (KeyStorageFlags.HasValue)
                                {
#if NET9_0_OR_GREATER
                                    return X509CertificateLoader.LoadPkcs12(Data, Password, KeyStorageFlags.Value);
#else
                                    return new X509Certificate(Data, Password, KeyStorageFlags.Value);
#endif
                                }
                                else
                                {
#if NET9_0_OR_GREATER
                                    return X509CertificateLoader.LoadPkcs12(Data, Password);
#else
                                    return new X509Certificate(Data, Password);
#endif
                                }
                            }
                        }
                        break;
                    case Ctor_FileName:
                        if (FileName != null)
                        {
                            if (Password == null)
                            {
#if NET9_0_OR_GREATER
                                return X509CertificateLoader.LoadCertificateFromFile(FileName);
#else
                                return new X509Certificate(FileName);
#endif
                            }
                            else
                            {
                                if (KeyStorageFlags.HasValue)
                                {
#if NET9_0_OR_GREATER
                                    return X509CertificateLoader.LoadPkcs12FromFile(FileName, Password, KeyStorageFlags.Value);
#else
                                    return new X509Certificate(FileName, Password, KeyStorageFlags.Value);
#endif
                                }
                                else
                                {
#if NET9_0_OR_GREATER
                                    return X509CertificateLoader.LoadPkcs12FromFile(FileName, Password);
#else
                                    return new X509Certificate(FileName, Password);
#endif
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
#if NET9_0_OR_GREATER
                        return X509CertificateLoader.LoadCertificate(Data);
#else
                        return new X509Certificate2(Data);
#endif
                    }
                    else
                    {
                        if (KeyStorageFlags.HasValue)
                        {
#if NET9_0_OR_GREATER
                            return X509CertificateLoader.LoadPkcs12(Data, Password, KeyStorageFlags.Value);
#else
                            return new X509Certificate2(Data, Password, KeyStorageFlags.Value);
#endif
                        }
                        else
                        {
#if NET9_0_OR_GREATER
                            return X509CertificateLoader.LoadPkcs12(Data, Password);
#else
                            return new X509Certificate2(Data, Password);
#endif
                        }
                    }
                }
                break;
            case Ctor_FileName:
                if (FileName != null)
                {
                    if (Password == null)
                    {
#if NET9_0_OR_GREATER
                        return X509CertificateLoader.LoadCertificateFromFile(FileName);
#else
                        return new X509Certificate2(FileName);
#endif
                    }
                    else
                    {
                        if (KeyStorageFlags.HasValue)
                        {
#if NET9_0_OR_GREATER
                            return X509CertificateLoader.LoadPkcs12FromFile(FileName, Password, KeyStorageFlags.Value);
#else
                            return new X509Certificate2(FileName, Password, KeyStorageFlags.Value);
#endif
                        }
                        else
                        {
#if NET9_0_OR_GREATER
                            return X509CertificateLoader.LoadPkcs12FromFile(FileName, Password);
#else
                            return new X509Certificate2(FileName, Password);
#endif
                        }
                    }
                }
                break;
        }
        return null;
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// 表示 X509Certificate 类型的常量
    /// </summary>
    public const byte Type_X509Certificate = 1;

    /// <summary>
    /// 表示 X509Certificate2 类型的常量
    /// </summary>
    public const byte Type_X509Certificate2 = 2;

    /// <summary>
    /// 表示使用字节数组作为参数的构造函数
    /// </summary>
    public const byte Ctor_ByteArray = 1;

    /// <summary>
    /// 表示使用文件名作为参数的构造函数
    /// </summary>
    public const byte Ctor_FileName = 2;

    /// <summary>
    /// 初始化 <see cref="X509CertificatePackable"/> 结构的新实例
    /// </summary>
    /// <param name="type"></param>
    /// <param name="constructor"></param>
    /// <param name="data"></param>
    /// <param name="fileName"></param>
    /// <param name="password"></param>
    /// <param name="keyStorageFlags"></param>
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

/// <summary>
/// X509Certificate 格式化程序特性
/// </summary>
public sealed class X509CertificateFormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509Certificate?>
{
    /// <summary>
    /// 获取 <see cref="X509CertificateFormatter.Default"/> 实例
    /// </summary>
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    /// <summary>
    /// 可空 <see cref="X509Certificate"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<X509Certificate?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="X509Certificate"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509Certificate? value)
        {
            IMemoryPackFormatter<X509Certificate?> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="X509Certificate"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509Certificate? value)
        {
            IMemoryPackFormatter<X509Certificate?> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

/// <summary>
/// X509Certificate2 格式化程序特性
/// </summary>
public sealed class X509Certificate2FormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509Certificate2?>
{
    /// <summary>
    /// 获取 <see cref="X509CertificateFormatter.Default"/> 实例
    /// </summary>
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    /// <summary>
    /// 可空 <see cref="X509Certificate2"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<X509Certificate2?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="X509Certificate2"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509Certificate2? value)
        {
            IMemoryPackFormatter<X509Certificate2?> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="X509Certificate2"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509Certificate2? value)
        {
            IMemoryPackFormatter<X509Certificate2?> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

/// <summary>
/// 可空 X509CertificatePackable 格式化程序特性
/// </summary>
public sealed class X509CertificatePackableNullableFormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509CertificatePackable?>
{
    /// <summary>
    /// 获取 <see cref="CookieFormatter.Default"/> 实例
    /// </summary>
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    /// <summary>
    /// 可空 <see cref="X509CertificatePackable"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<X509CertificatePackable?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="X509CertificatePackable"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509CertificatePackable? value)
        {
            IMemoryPackFormatter<X509CertificatePackable?> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="X509CertificatePackable"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509CertificatePackable? value)
        {
            IMemoryPackFormatter<X509CertificatePackable?> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

/// <summary>
/// X509CertificatePackable 格式化程序特性
/// </summary>
public sealed class X509CertificatePackableFormatterAttribute : MemoryPackCustomFormatterAttribute<X509CertificateFormatter, X509CertificatePackable>
{
    /// <summary>
    /// 获取 <see cref="CookieFormatter.Default"/> 实例
    /// </summary>
    public sealed override X509CertificateFormatter GetFormatter() => X509CertificateFormatter.Default;

    /// <summary>
    /// <see cref="X509CertificatePackable"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<X509CertificatePackable>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将 <see cref="X509CertificatePackable"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref X509CertificatePackable value)
        {
            IMemoryPackFormatter<X509CertificatePackable> f = X509CertificateFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为 <see cref="X509CertificatePackable"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref X509CertificatePackable value)
        {
            IMemoryPackFormatter<X509CertificatePackable> f = X509CertificateFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}
