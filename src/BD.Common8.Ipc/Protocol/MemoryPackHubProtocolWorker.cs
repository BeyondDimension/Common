namespace BD.Common8.Ipc.Protocol;

/// <summary>
/// Initializes a new instance of the <see cref="MemoryPackHubProtocolWorker"/> class.
/// </summary>
/// <param name="memoryPackSerializerOptions"></param>
sealed class MemoryPackHubProtocolWorker(MemoryPackSerializerOptions memoryPackSerializerOptions)
{
    const int ErrorResult = 1;
    const int VoidResult = 2;
    const int NonVoidResult = 3;

    const int MaxLengthPrefixSize = 5;

    readonly MemoryPackSerializerOptions _memoryPackSerializerOptions = memoryPackSerializerOptions;

    public bool TryParseMessage(
        ref ReadOnlySequence<byte> input,
        IInvocationBinder binder,
        [NotNullWhen(true)] out HubMessage? message
        )
    {
        if (!TryParseBinaryMessage(ref input, out ReadOnlySequence<byte> payload))
        {
            message = null;
            return false;
        }

        var state = MemoryPackReaderOptionalStatePool.Rent(_memoryPackSerializerOptions);

        MemoryPackReader memoryPackReader = new MemoryPackReader(in payload, state);

        message = ParseHubMessage(ref memoryPackReader, binder);

        return message != null;
    }

    public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
    {
        var memoryBufferWriter = MemoryBufferWriter.Get();

        try
        {
            MemoryPackWriter<MemoryBufferWriter> writer =
                new(ref memoryBufferWriter, MemoryPackWriterOptionalStatePool.Rent(_memoryPackSerializerOptions));

            WriteMessageCore(message, ref writer);

            WriteLengthPrefix(memoryBufferWriter.Length, output);

            memoryBufferWriter.CopyTo(output);
        }
        finally
        {
            MemoryBufferWriter.Return(memoryBufferWriter);
        }
    }

    public ReadOnlyMemory<byte> GetMessageBytes(HubMessage message)
    {
        var arrayBufferWriter = new ArrayBufferWriter<byte>();

        MemoryPackWriter<ArrayBufferWriter<byte>> writer =
            new(ref arrayBufferWriter, MemoryPackWriterOptionalStatePool.Rent(_memoryPackSerializerOptions));

        WriteMessageCore(message, ref writer);

        int dataLength = arrayBufferWriter.WrittenSpan.Length;
        int prefixLength = LengthPrefixLength(arrayBufferWriter.WrittenSpan.Length);

        var array = new byte[prefixLength + dataLength];
        var span = array.AsSpan();

        var written = WriteLengthPrefix(dataLength, span);

        arrayBufferWriter.WrittenSpan.CopyTo(span[prefixLength..]);

        return array;
    }

    static bool TryParseBinaryMessage(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> payload)
    {
        if (buffer.IsEmpty)
        {
            payload = default;
            return false;
        }

        var length = 0U;
        var numBytes = 0;

        var lengthPrefixBuffer = buffer.Slice(0, Math.Min(MaxLengthPrefixSize, buffer.Length));
        var span = GetSpan(lengthPrefixBuffer);

        byte byteRead;
        do
        {
            byteRead = span[numBytes];
            length |= ((uint)(byteRead & 0x7f)) << (numBytes * 7);
            numBytes++;
        }
        while (numBytes < lengthPrefixBuffer.Length && ((byteRead & 0x80) != 0));

        // size bytes are missing
        if ((byteRead & 0x80) != 0 && (numBytes < MaxLengthPrefixSize))
        {
            payload = default;
            return false;
        }

        if ((byteRead & 0x80) != 0 || (numBytes == MaxLengthPrefixSize && byteRead > 7))
        {
            throw new FormatException("Messages over 2GB in size are not supported.");
        }

        // We don't have enough data
        if (buffer.Length < length + numBytes)
        {
            payload = default;
            return false;
        }

        // Get the payload
        payload = buffer.Slice(numBytes, (int)length);

        // Skip the payload
        buffer = buffer.Slice(numBytes + (int)length);
        return true;
    }

    static HubMessage? ParseHubMessage(ref MemoryPackReader reader, IInvocationBinder binder)
    {
        int itemCount;
        try
        {
            if (!reader.TryReadCollectionHeader(out itemCount))
            {
                return null;
            }
        }
        catch
        {
            return null;
        }

        int messageType = ReadValue<byte>(ref reader, "messageType");

        HubMessage? parsedMessage = messageType switch
        {
            HubProtocolConstants.InvocationMessageType => CreateInvocationMessage(ref reader, binder, itemCount),
            HubProtocolConstants.StreamInvocationMessageType => CreateStreamInvocationMessage(ref reader, binder, itemCount),
            HubProtocolConstants.StreamItemMessageType => CreateStreamItemMessage(ref reader, binder),
            HubProtocolConstants.CompletionMessageType => CreateCompletionMessage(ref reader, binder),
            HubProtocolConstants.CancelInvocationMessageType => CreateCancelInvocationMessage(ref reader),
            HubProtocolConstants.PingMessageType => PingMessage.Instance,
            HubProtocolConstants.CloseMessageType => CreateCloseMessage(ref reader, itemCount),
            _ => null,
        };

        return parsedMessage;
    }

    static ReadOnlySpan<byte> GetSpan(in ReadOnlySequence<byte> lengthPrefixBuffer)
    {
        if (lengthPrefixBuffer.IsSingleSegment)
        {
            return lengthPrefixBuffer.First.Span;
        }

        // Should be rare
        return lengthPrefixBuffer.ToArray();
    }

    static int LengthPrefixLength(long length)
    {
        var lenNumBytes = 0;
        do
        {
            length >>= 7;
            lenNumBytes++;
        }
        while (length > 0);

        return lenNumBytes;
    }

    static void WriteLengthPrefix(long length, IBufferWriter<byte> output)
    {
        Span<byte> lenBuffer = stackalloc byte[5];

        var lenNumBytes = WriteLengthPrefix(length, lenBuffer);

        output.Write(lenBuffer[..lenNumBytes]);
    }

    static int WriteLengthPrefix(long length, Span<byte> output)
    {
        // This code writes length prefix of the message as a VarInt. Read the comment in
        // the BinaryMessageParser.TryParseMessage for details.
        var lenNumBytes = 0;
        do
        {
            ref var current = ref output[lenNumBytes];
            current = (byte)(length & 0x7f);
            length >>= 7;
            if (length > 0)
            {
                current |= 0x80;
            }
            lenNumBytes++;
        }
        while (length > 0);

        return lenNumBytes;
    }

    #region WriteMessages

    static void WriteMessageCore<TWriter>(HubMessage message, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        switch (message)
        {
            case InvocationMessage invocationMessage:
                WriteInvocationMessage(invocationMessage, ref writer);
                break;

            case StreamInvocationMessage streamInvocationMessage:
                WriteStreamInvocationMessage(streamInvocationMessage, ref writer);
                break;

            case StreamItemMessage streamItemMessage:
                WriteStreamingItemMessage(streamItemMessage, ref writer);
                break;

            case CompletionMessage completionMessage:
                WriteCompletionMessage(completionMessage, ref writer);
                break;

            case CancelInvocationMessage cancelInvocationMessage:
                WriteCancelInvocationMessage(cancelInvocationMessage, ref writer);
                break;

            case PingMessage:
                WritePingMessage(ref writer);
                break;

            default:
                break;
        }

        writer.Flush();
    }

    static void WriteInvocationMessage<TWriter>(InvocationMessage message, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        writer.WriteCollectionHeader(6);

        writer.WriteValue<byte>(HubProtocolConstants.InvocationMessageType);

        PackHeaders(message.Headers, ref writer);

        if (string.IsNullOrEmpty(message.InvocationId))
        {
            writer.WriteString(null);
        }
        else
        {
            writer.WriteString(message.InvocationId);
        }
        writer.WriteString(message.Target);

        if (message.Arguments is null)
        {
            writer.WriteCollectionHeader(0);
        }
        else
        {
            writer.WriteCollectionHeader(message.Arguments.Length);
            foreach (var arg in message.Arguments)
            {
                WriteArgument(arg, ref writer);
            }
        }

        WriteStreamIds(message.StreamIds, ref writer);
    }

    static void WriteStreamInvocationMessage<TWriter>(StreamInvocationMessage message, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        writer.WriteCollectionHeader(6);

        writer.WriteValue<byte>(HubProtocolConstants.StreamInvocationMessageType);

        PackHeaders(message.Headers, ref writer);

        writer.WriteString(message.InvocationId);
        writer.WriteString(message.Target);

        if (message.Arguments is null)
        {
            writer.WriteCollectionHeader(0);
        }
        else
        {
            writer.WriteCollectionHeader(message.Arguments.Length);
            foreach (var arg in message.Arguments)
            {
                WriteArgument(arg, ref writer);
            }
        }

        WriteStreamIds(message.StreamIds, ref writer);
    }

    static void WriteStreamingItemMessage<TWriter>(StreamItemMessage message, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        writer.WriteCollectionHeader(4);
        writer.WriteValue<byte>(HubProtocolConstants.StreamItemMessageType);

        PackHeaders(message.Headers, ref writer);

        writer.WriteString(message.InvocationId);

        WriteArgument(message.Item, ref writer);
    }

    static void WriteCompletionMessage<TWriter>(CompletionMessage message, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        var resultKind =
            message.Error != null ? ErrorResult :
            message.HasResult ? NonVoidResult :
            VoidResult;

        writer.WriteCollectionHeader(4 + (resultKind != VoidResult ? 1 : 0));
        writer.WriteValue<byte>(HubProtocolConstants.CompletionMessageType);

        PackHeaders(message.Headers, ref writer);

        writer.WriteString(message.InvocationId);
        writer.WriteValue(resultKind);

        switch (resultKind)
        {
            case ErrorResult:
                writer.WriteString(message.Error);
                break;

            case NonVoidResult:
                WriteArgument(message.Result, ref writer);
                break;
        }
    }

    static void WriteCancelInvocationMessage<TWriter>(CancelInvocationMessage message, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        writer.WriteCollectionHeader(3);
        writer.WriteValue<byte>(HubProtocolConstants.CancelInvocationMessageType);

        PackHeaders(message.Headers, ref writer);

        writer.WriteString(message.InvocationId);
    }

    static void WritePingMessage<TWriter>(ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        writer.WriteCollectionHeader(1);
        writer.WriteValue<byte>(HubProtocolConstants.PingMessageType);
    }

    static void PackHeaders<TWriter>(IDictionary<string, string>? headers, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        if (headers != null)
        {
            writer.WriteObjectHeader((byte)headers.Count);
            if (headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    writer.WriteString(header.Key);
                    writer.WriteString(header.Value);
                }
            }
        }
        else
        {
            writer.WriteObjectHeader(0);
        }
    }

    #endregion WriteMessages

    #region CreateHubMessages

    static HubMessage CreateInvocationMessage(
        ref MemoryPackReader reader,
        IInvocationBinder binder,
        int itemCount)
    {
        Dictionary<string, string>? headers = ReadHeaders(ref reader);

        // 不一定需要有头
        //ArgumentNullException.ThrowIfNull(headers);

        string? invocationId = ReadString(ref reader, "invocationId");

        string? target = ReadString(ref reader, "target");

        ArgumentNullException.ThrowIfNull(target);

        object?[]? arguments;
        try
        {
            var parameterTypes = binder.GetParameterTypes(target);
            arguments = BindArguments(ref reader, parameterTypes);
        }
        catch (Exception ex)
        {
            return new InvocationBindingFailureMessage(invocationId, target, ExceptionDispatchInfo.Capture(ex));
        }

        // TODO: 不明白这个是干啥的 ?
        string[]? streams = null;
        // Previous clients will send 5 items, so we check if they sent a stream array or not
        if (itemCount > 5)
        {
            streams = ReadStreamIds(ref reader);
        }

        return ApplyHeaders(headers, new InvocationMessage(invocationId, target, arguments, streams));
    }

    static HubMessage CreateStreamInvocationMessage(
        ref MemoryPackReader reader,
        IInvocationBinder binder,
        int itemCount)
    {
        Dictionary<string, string>? headers = ReadHeaders(ref reader);

        //ArgumentNullException.ThrowIfNull(headers);

        string? invocationId = ReadString(ref reader, "invocationId");

        string? target = ReadString(ref reader, "target");

        ArgumentNullException.ThrowIfNull(target);

        object?[]? arguments;
        try
        {
            var parameterTypes = binder.GetParameterTypes(target);
            arguments = BindArguments(ref reader, parameterTypes);
        }
        catch (Exception ex)
        {
            return new InvocationBindingFailureMessage(invocationId, target, ExceptionDispatchInfo.Capture(ex));
        }

        // TODO: 不明白这个是干啥的 ?
        string[]? streams = null;
        // Previous clients will send 5 items, so we check if they sent a stream array or not
        if (itemCount > 5)
        {
            streams = ReadStreamIds(ref reader);
        }

        return ApplyHeaders(headers, new StreamInvocationMessage(invocationId!, target, arguments, streams));
    }

    static HubMessage CreateStreamItemMessage(ref MemoryPackReader reader, IInvocationBinder binder)
    {
        var headers = ReadHeaders(ref reader);
        var invocationId = ReadString(ref reader, "invocationId");

        ArgumentException.ThrowIfNullOrEmpty(invocationId, "invocation ID for StreamItem message");

        object? value;
        try
        {
            var itemType = binder.GetStreamItemType(invocationId);

            value = ReadValue(ref reader, itemType, "item");
        }
        catch (Exception ex)
        {
            return new StreamBindingFailureMessage(invocationId, ExceptionDispatchInfo.Capture(ex));
        }

        return ApplyHeaders(headers, new StreamItemMessage(invocationId, value));
    }

    static CompletionMessage CreateCompletionMessage(ref MemoryPackReader reader, IInvocationBinder binder)
    {
        var headers = ReadHeaders(ref reader);
        var invocationId = ReadString(ref reader, "invocationId");

        ArgumentException.ThrowIfNullOrEmpty(invocationId, "invocation ID for Completion message");

        var resultKind = ReadValue<int>(ref reader, "resultKind");

        string? error = null;
        object? result = null;
        var hasResult = false;

        switch (resultKind)
        {
            case ErrorResult:
                error = ReadString(ref reader, "error");
                break;

            case NonVoidResult:
                hasResult = true;
                var itemType = binder.GetReturnType(invocationId);
                if (itemType is null)
                {
                    //reader.Skip();
                }
                else
                {
                    if (itemType == typeof(RawResult))
                    {
                        //result = new RawResult(reader.ReadRaw());
                    }
                    else
                    {
                        try
                        {
                            result = ReadValue(ref reader, itemType, "argument");
                        }
                        catch (Exception ex)
                        {
                            error = $"Error trying to deserialize result to {itemType.Name}. {ex.Message}";
                            hasResult = false;
                        }
                    }
                }
                break;

            case VoidResult:
                hasResult = false;
                break;

            default:
                throw new InvalidDataException("Invalid invocation result kind.");
        }

        return ApplyHeaders(headers, new CompletionMessage(invocationId, error, result, hasResult));
    }

    static CancelInvocationMessage CreateCancelInvocationMessage(ref MemoryPackReader reader)
    {
        var headers = ReadHeaders(ref reader);
        var invocationId = ReadString(ref reader, "invocationId");

        ArgumentException.ThrowIfNullOrEmpty(invocationId, "invocation ID for CancelInvocation message");

        return ApplyHeaders(headers, new CancelInvocationMessage(invocationId));
    }

    static CloseMessage CreateCloseMessage(ref MemoryPackReader reader, int itemCount)
    {
        var error = ReadString(ref reader, "error");
        var allowReconnect = false;

        if (itemCount > 2)
        {
            allowReconnect = ReadValue<bool>(ref reader, "allowReconnect");
        }

        // An empty string is still an error
        if (error == null && !allowReconnect)
        {
            return CloseMessage.Empty;
        }

        return new CloseMessage(error, allowReconnect);
    }

    static T ApplyHeaders<T>(IDictionary<string, string>? source, T destination) where T : HubInvocationMessage
    {
        if (source != null && source.Count > 0)
        {
            destination.Headers = source;
        }

        return destination;
    }

    static object?[] BindArguments(ref MemoryPackReader reader, IReadOnlyList<Type> parameterTypes)
    {
        if (!reader.TryReadCollectionHeader(out int argumentCount))
            throw new InvalidOperationException("TryReadCollectionHeader failed.");

        if (parameterTypes.Count != argumentCount)
            throw new InvalidDataException($"Invocation provides {argumentCount} argument(s) but target expects {parameterTypes.Count}.");

        try
        {
            var arguments = new object?[argumentCount];

            for (int index = 0; index < argumentCount; ++index)
            {
                // MessagePack 中会交给抽象方法 DeserializeObject 交由子类去实现，这里直接 MemoryPack 的实现
                arguments[index] = reader.ReadValue(parameterTypes[index]);
            }

            return arguments;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Error binding arguments. Make sure that the types of the provided values match the types of the hub method being invoked.", ex);
        }
    }

    #endregion CreateHubMessages

    #region Read

    static Dictionary<string, string>? ReadHeaders(ref MemoryPackReader reader)
    {
        if (!reader.TryReadObjectHeader(out byte memberCount))
            return null;

        if (memberCount <= 0)
            return null;

        var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);

        for (int index = 0; index < memberCount; ++index)
        {
            string key = reader.ReadValue<string>()!;
            string value = reader.ReadValue<string>() ?? string.Empty;

            dictionary.Add(key, value);
        }

        return dictionary;
    }

    static string[]? ReadStreamIds(ref MemoryPackReader reader)
    {
        if (!reader.TryReadCollectionHeader(out int streamCount))
            return null;

        List<string> list = new();
        for (int index = 0; index < streamCount; ++index)
        {
            string? streamId = ReadString(ref reader, "streamId");

            ArgumentException.ThrowIfNullOrEmpty(streamId, "streamIds received");

            list.Add(streamId);
        }
        return [.. list];
    }

    static string? ReadString(ref MemoryPackReader reader, string filedName)
    {
        try
        {
            return reader.ReadString();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Reading '{filedName}' as string failed.", ex);
        }
    }

    static T? ReadValue<T>(ref MemoryPackReader reader, string filedName)
    {
        try
        {
            return reader.ReadValue<T>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Reading '{filedName}' as {typeof(T).Name} failed.", ex);
        }
    }

    static object? ReadValue(ref MemoryPackReader reader, Type type, string filedName)
    {
        try
        {
            return reader.ReadValue(type);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Reading '{filedName}' as {type?.Name} failed.", ex);
        }
    }

    #endregion Read

    #region Write

    static void WriteStreamIds<TWriter>(string[]? streamIds, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        if (streamIds != null)
        {
            writer.WriteCollectionHeader(streamIds.Length);
            foreach (var streamId in streamIds)
            {
                writer.WriteString(streamId);
            }
        }
        else
        {
            writer.WriteCollectionHeader(0);
        }
    }

    static void WriteArgument<TWriter>(object? argument, ref MemoryPackWriter<TWriter> writer) where TWriter : IBufferWriter<byte>
    {
        if (argument == null)
        {
            writer.WriteValue<string>(null);
        }
        else
        {
            MemoryPackSerializer.Serialize(argument.GetType(), ref writer, argument);
        }
    }

    #endregion Write
}