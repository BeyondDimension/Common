namespace BD.Common8.Ipc.Models;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 应用程序连接字符串
/// </summary>
public readonly struct IpcAppConnectionString
{
    /// <inheritdoc cref="IpcAppConnectionStringType"/>
    public readonly IpcAppConnectionStringType Type { get; init; }

    public readonly string? StringValue { get; init; }

    public readonly int Int32Value { get; init; }

    public void Write(Stream stream)
    {
        var connectionStringType = Type;
        stream.WriteByte((byte)connectionStringType);
        switch (connectionStringType)
        {
            case IpcAppConnectionStringType.Https:
                stream.Write(BitConverter.GetBytes(Int32Value));
                break;
            case IpcAppConnectionStringType.UnixSocket:
                stream.Write(Encoding.UTF8.GetBytes(StringValue.ThrowIsNull()));
                break;
            case IpcAppConnectionStringType.NamedPipe:
                stream.Write(Encoding.UTF8.GetBytes(StringValue.ThrowIsNull()));
                break;
            default:
                throw ThrowHelper.GetArgumentOutOfRangeException(connectionStringType);
        }
    }

    public byte[] ToByteArray()
    {
        using var stream = new MemoryStream();
        Write(stream);
        var buffer = stream.ToArray();
        return buffer;
    }

    public override string? ToString() => Type switch
    {
        IpcAppConnectionStringType.Https => $"https://localhost:{Int32Value}",
        IpcAppConnectionStringType.UnixSocket or IpcAppConnectionStringType.NamedPipe => StringValue,
        _ => base.ToString(),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IpcAppConnectionString(byte[] buffer)
    {
        var connectionStringType = (IpcAppConnectionStringType)buffer[0];
        return connectionStringType switch
        {
            IpcAppConnectionStringType.Https => new()
            {
                Type = connectionStringType,
                Int32Value = BitConverter.ToInt32(buffer, 1),
            },
            IpcAppConnectionStringType.UnixSocket or IpcAppConnectionStringType.NamedPipe => new()
            {
                Type = connectionStringType,
                StringValue = Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1),
            },
            _ => throw ThrowHelper.GetArgumentOutOfRangeException(connectionStringType),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator byte[](IpcAppConnectionString connectionString)
        => connectionString.ToByteArray();
}
