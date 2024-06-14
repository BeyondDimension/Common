//namespace System.IO;

//public sealed partial class BinaryResource(byte[] buffer, bool reverse)
//{
//    public byte[] Buffer { get; private set; } = buffer;

//    public bool Reverse { get; private set; } = reverse;
//}

//partial class BinaryResource : IDisposable // IDisposable
//{
//    bool disposedValue;

//    void Dispose(bool disposing)
//    {
//        if (!disposedValue)
//        {
//            if (disposing)
//            {
//                if (Buffer != null)
//                {
//                    if (Buffer.Length != 0)
//                    {
//                        Buffer.AsSpan().Clear();
//                    }
//                    Buffer = null!;
//                }
//            }

//            disposedValue = true;
//        }
//    }

//    /// <inheritdoc/>
//    public void Dispose()
//    {
//        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
//        Dispose(disposing: true);
//        GC.SuppressFinalize(this);
//    }
//}

//partial class BinaryResource // Deserialize
//{
//    /// <inheritdoc cref="MemoryPackSerializer.Deserialize{T}(ReadOnlySpan{byte}, MemoryPackSerializerOptions?)"/>
//    public T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(MemoryPackSerializerOptions? options = null)
//    {
//        Span<byte> buffer = Buffer;
//        byte[]? shared = null;
//        try
//        {
//            if (Reverse)
//            {
//                shared = ArrayPool<byte>.Shared.Rent(buffer.Length);
//                var shared_span = buffer = shared.AsSpan(0, buffer.Length);
//                buffer.CopyTo(shared_span);
//                shared_span.Reverse();
//                buffer = shared_span;
//            }
//        }
//        finally
//        {
//            if (shared != null)
//                ArrayPool<byte>.Shared.Return(shared);
//        }
//        return MemoryPackSerializer.Deserialize<T>(buffer, options);
//    }
//}