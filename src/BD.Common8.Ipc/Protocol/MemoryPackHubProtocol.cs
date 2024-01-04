namespace BD.Common8.Ipc.Protocol;

/// <summary>
/// Initializes a new instance of the <see cref="MemoryPackHubProtocol"/> class.
/// </summary>
/// <param name="options"></param>
public sealed class MemoryPackHubProtocol : IHubProtocol
{
    readonly MemoryPackHubProtocolWorker _worker
        = new(MemoryPackSerializerOptions.Default);

    public string Name => "memorypack";

    public TransferFormat TransferFormat => TransferFormat.Binary;

    public int Version => 1;

    public ReadOnlyMemory<byte> GetMessageBytes(HubMessage message)
    {
        return _worker.GetMessageBytes(message);
    }

    public bool IsVersionSupported(int version)
    {
        return version <= Version;
    }

    public bool TryParseMessage(ref ReadOnlySequence<byte> input, IInvocationBinder binder, [NotNullWhen(true)] out HubMessage? message)
    {
        return _worker.TryParseMessage(ref input, binder, out message);
    }

    public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
    {
        _worker.WriteMessage(message, output);
    }
}