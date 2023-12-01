namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="RSAParameters"/> 的序列化与反序列化实现
/// </summary>
public sealed class RSAParametersFormatter : IMemoryPackFormatter<RSAParameters>
{
    /// <summary>
    /// 默认的 <see cref="RSAParametersFormatter"/> 实例
    /// </summary>
    public static readonly RSAParametersFormatter Default = new();

    /// <inheritdoc/>
    void IMemoryPackFormatter<RSAParameters>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref RSAParameters value)
    {
        RSAParametersMemoryPackable packable = value;
        writer.WritePackable(packable);
    }

    /// <inheritdoc/>
    void IMemoryPackFormatter<RSAParameters>.Deserialize(ref MemoryPackReader reader, scoped ref RSAParameters value)
    {
        var wrapped = reader.ReadPackable<RSAParametersMemoryPackable>();
        value = wrapped.RSAParameters;
    }
}

/// <summary>
/// 提供将 <see cref="RSAParameters"/> 转换为可序列化和反序列化的内存包装类型
/// </summary>
[MemoryPackable]
public readonly partial struct RSAParametersMemoryPackable
{
    /// <summary>
    /// 忽略的成员字段
    /// </summary>
    [MemoryPackIgnore]
    public readonly RSAParameters RSAParameters;

    /// <summary>
    /// 表示 RSA 算法的 D 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? D => RSAParameters.D;

    /// <summary>
    /// 表示 RSA 算法的 DP 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? DP => RSAParameters.DP;

    /// <summary>
    /// 表示 RSA 算法的 DQ 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? DQ => RSAParameters.DQ;

    /// <summary>
    /// 表示 RSA 算法的 Exponent 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? Exponent => RSAParameters.Exponent;

    /// <summary>
    /// 表示 RSA 算法的 InverseQ 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? InverseQ => RSAParameters.InverseQ;

    /// <summary>
    /// 表示 RSA 算法的 Modulus 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? Modulus => RSAParameters.Modulus;

    /// <summary>
    /// 表示 RSA 算法的 P 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? P => RSAParameters.P;

    /// <summary>
    /// 表示 RSA 算法的 Q 参数
    /// </summary>
    [MemoryPackInclude]
    byte[]? Q => RSAParameters.Q;

    [MemoryPackConstructor]
    RSAParametersMemoryPackable(byte[]? d, byte[]? dp, byte[]? dq, byte[]? exponent, byte[]? inverseQ, byte[]? modulus, byte[]? p, byte[]? q) => RSAParameters = new()
    {
        D = d,
        DP = dp,
        DQ = dq,
        Exponent = exponent,
        InverseQ = inverseQ,
        Modulus = modulus,
        P = p,
        Q = q,
    };

    /// <summary>
    /// 初始化 <see cref="RSAParametersMemoryPackable"/> 结构的新实例
    /// </summary>
    /// <param name="value"></param>
    public RSAParametersMemoryPackable(RSAParameters value) => RSAParameters = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator RSAParameters(RSAParametersMemoryPackable value) => value.RSAParameters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator RSAParametersMemoryPackable(RSAParameters value) => new(value);
}

/// <summary>
/// RSAParameters 格式化程序特性
/// </summary>
public sealed class RSAParametersFormatterAttribute : MemoryPackCustomFormatterAttribute<RSAParametersFormatter, RSAParameters>
{
    /// <summary>
    /// 获取 <see cref="RSAParametersFormatter.Default"/> 实例
    /// </summary>
    public sealed override RSAParametersFormatter GetFormatter() => RSAParametersFormatter.Default;

    /// <summary>
    /// <see cref="RSAParameters"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<RSAParameters>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="RSAParameters"/> 对象序列化到 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref RSAParameters value)
        {
            IMemoryPackFormatter<RSAParameters> f = RSAParametersFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为 <see cref="RSAParameters"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref RSAParameters value)
        {
            IMemoryPackFormatter<RSAParameters> f = RSAParametersFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}