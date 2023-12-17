namespace BD.Common8.Ipc.Models;

/// <summary>
/// Ipc 应用程序连接字符串
/// </summary>
public readonly struct IpcAppConnectionString
{
    /// <summary>
    /// The name of this scheme.
    /// </summary>
    public const string AuthenticationScheme = "Ipc";

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

    /// <inheritdoc cref="Environment.TickCount64"/>
    public readonly long TickCount64 { get; init; }

    /// <inheritdoc cref="Environment.ProcessId"/>
    public readonly int ProcessId { get; init; }

    /// <summary>
    /// 将连接字符串写入指定的流中
    /// </summary>
    public void Write(Stream stream)
    {
        const byte formatType = 0;
        stream.WriteByte(formatType);
        var connectionStringType = Type;
        stream.WriteByte((byte)connectionStringType);
        stream.Write(BitConverter.GetBytes(TickCount64));
        stream.Write(BitConverter.GetBytes(ProcessId));
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
    public static implicit operator IpcAppConnectionString?(byte[] buffer)
    {
        try
        {
            //var formatType = buffer[0];
            var connectionStringType = (IpcAppConnectionStringType)buffer[1];
            int startIndex = 2;
            var tickCount64 = BitConverter.ToInt64(buffer, startIndex);
            startIndex += sizeof(long);
            var processId = BitConverter.ToInt32(buffer, startIndex);
            startIndex += sizeof(int);
            return connectionStringType switch
            {
                IpcAppConnectionStringType.Https => new()
                {
                    Type = connectionStringType,
                    Int32Value = BitConverter.ToInt32(buffer, startIndex),
                    TickCount64 = tickCount64,
                    ProcessId = processId,
                },
                IpcAppConnectionStringType.UnixSocket or IpcAppConnectionStringType.NamedPipe => new()
                {
                    Type = connectionStringType,
                    StringValue = Encoding.UTF8.GetString(buffer, startIndex, buffer.Length - startIndex),
                    TickCount64 = tickCount64,
                    ProcessId = processId,
                },
                _ => throw ThrowHelper.GetArgumentOutOfRangeException(connectionStringType),
            };
        }
        catch
        {
            return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator byte[](IpcAppConnectionString connectionString)
        => connectionString.ToByteArray();

    /// <summary>
    /// 写入身份验证令牌
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="tickCount64"></param>
    /// <param name="processId"></param>
    public static void WriteAccessToken(Stream stream, long tickCount64, int processId)
    {
        stream.Write(BitConverter.GetBytes(tickCount64));
        stream.Write(BitConverter.GetBytes(processId));
        stream.Write("-----"u8);
        stream.WriteUtf16StrToUtf8OrCustom(Environment.OSVersion.VersionString);
        stream.Position = 0;
    }

    /// <summary>
    /// 获取身份验证令牌
    /// </summary>
    /// <returns></returns>
    public string GetAccessToken()
    {
        using var stream = new MemoryStream();
        WriteAccessToken(stream, TickCount64, ProcessId);
        var accessToken = Hashs.String.SHA256(stream, false);
        return $"{AuthenticationScheme} {accessToken}";
    }
}
