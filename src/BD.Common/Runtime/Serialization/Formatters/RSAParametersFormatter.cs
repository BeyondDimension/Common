namespace System.Runtime.Serialization.Formatters;

public sealed class RSAParametersFormatter : IMemoryPackFormatter<RSAParameters>
{
    public static readonly RSAParametersFormatter Default = new();

    void IMemoryPackFormatter<RSAParameters>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref RSAParameters value)
    {
        RSAParametersMemoryPackable packable = value;
        writer.WritePackable(packable);
    }

    void IMemoryPackFormatter<RSAParameters>.Deserialize(ref MemoryPackReader reader, scoped ref RSAParameters value)
    {
        var wrapped = reader.ReadPackable<RSAParametersMemoryPackable>();
        value = wrapped.RSAParameters;
    }
}

[MemoryPackable]
public readonly partial struct RSAParametersMemoryPackable
{
    [MemoryPackIgnore]
    public readonly RSAParameters RSAParameters;

    [MemoryPackInclude]
    byte[]? D => RSAParameters.D;

    [MemoryPackInclude]
    byte[]? DP => RSAParameters.DP;

    [MemoryPackInclude]
    byte[]? DQ => RSAParameters.DQ;

    [MemoryPackInclude]
    byte[]? Exponent => RSAParameters.Exponent;

    [MemoryPackInclude]
    byte[]? InverseQ => RSAParameters.InverseQ;

    [MemoryPackInclude]
    byte[]? Modulus => RSAParameters.Modulus;

    [MemoryPackInclude]
    byte[]? P => RSAParameters.P;

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

    public RSAParametersMemoryPackable(RSAParameters value) => RSAParameters = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator RSAParameters(RSAParametersMemoryPackable value) => value.RSAParameters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator RSAParametersMemoryPackable(RSAParameters value) => new(value);
}

public sealed class RSAParametersFormatterAttribute : MemoryPackCustomFormatterAttribute<RSAParametersFormatter, RSAParameters>
{
    public sealed override RSAParametersFormatter GetFormatter() => RSAParametersFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<RSAParameters>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref RSAParameters value)
        {
            IMemoryPackFormatter<RSAParameters> f = RSAParametersFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref RSAParameters value)
        {
            IMemoryPackFormatter<RSAParameters> f = RSAParametersFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}