namespace BD.Common8.Ipc.Models;

/// <summary>
/// Ipc 应用程序连接字符串
/// </summary>
public readonly struct IpcAppConnectionString
{
    /// <inheritdoc cref="IpcAppConnectionStringType"/>
    public readonly IpcAppConnectionStringType Type { get; init; }

    /// <summary>
    /// 连接字符串中的内容
    /// </summary>
    public readonly string? StringValue { get; init; }

    /// <summary>
    /// 连接字符串中的内容
    /// </summary>
    public readonly int Int32Value { get; init; }

    /// <summary>
    /// 将连接字符串写入指定的流中
    /// </summary>
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

    /// <summary>
    /// 将连接字符串转换为字节数组形式
    /// </summary>
    public byte[] ToByteArray()
    {
        using var stream = new MemoryStream();
        Write(stream);
        var buffer = stream.ToArray();
        return buffer;
    }

    /// <summary>
    /// 根据连接字符串类型的不同返回不同的字符串表达形式
    /// </summary>
    /// <returns></returns>
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
