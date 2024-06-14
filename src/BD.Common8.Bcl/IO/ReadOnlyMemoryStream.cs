//namespace System.IO;

//// https://github.com/CommunityToolkit/dotnet/blob/v8.2.2/src/CommunityToolkit.HighPerformance/Streams/MemoryStream.cs
//// https://github.com/dotnet/runtime/blob/v8.0.6/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/ReadBufferState.cs#L40

//public sealed partial class ReadOnlyMemoryStream : Stream
//{
//    byte[] _buffer;

//    /// <summary>
//    /// The current position within <see cref="_buffer"/>.
//    /// </summary>
//    int position;

//    /// <summary>
//    /// Indicates whether or not the current instance has been disposed
//    /// </summary>
//    bool disposed;

//    readonly bool reverse;

//    /// <summary>
//    /// Initializes a new instance of the <see cref="ReadOnlyMemoryStream"/> class.
//    /// </summary>
//    /// <param name="buffer"></param>
//    /// <param name="reverse"></param>
//#pragma warning disable IDE0290 // 使用主构造函数
//    public ReadOnlyMemoryStream(byte[] buffer, bool reverse = false)
//#pragma warning restore IDE0290 // 使用主构造函数
//    {
//        if (buffer == null || buffer.Length == 0)
//        {
//            _buffer = [];
//            this.reverse = false;
//        }
//        else
//        {
//            _buffer = buffer;
//            this.reverse = reverse;
//        }
//    }

//    /// <inheritdoc/>
//    public sealed override bool CanRead
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        get => !disposed;
//    }

//    /// <inheritdoc/>
//    public sealed override bool CanSeek
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        get => !disposed;
//    }

//    /// <inheritdoc/>
//    public sealed override bool CanWrite
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        get => false;
//    }

//    /// <inheritdoc/>
//    public sealed override long Length
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        get
//        {
//            ValidateDisposed(disposed);
//            return _buffer.Length;
//        }
//    }

//    int GetLogicPosition()
//    {
//        if (reverse)
//        {
//            return _buffer.Length - position;
//        }
//        else
//        {
//            return position;
//        }
//    }

//    void AddPosition(int value)
//    {
//        if (reverse)
//        {
//            position -= value;
//        }
//        else
//        {
//            position += value;
//        }
//    }

//    /// <inheritdoc/>
//    public sealed override long Position
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        get
//        {
//            ValidateDisposed(disposed);
//            return position;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        set
//        {
//            ValidateDisposed(disposed);
//            ValidatePosition(value, _buffer.Length);
//            position = unchecked((int)value);
//        }
//    }

//    /// <inheritdoc/>
//    public sealed override void Flush()
//    {
//    }

//    /// <inheritdoc/>
//    public sealed override Task FlushAsync(CancellationToken cancellationToken)
//    {
//        if (cancellationToken.IsCancellationRequested)
//        {
//            return Task.FromCanceled(cancellationToken);
//        }
//        return Task.CompletedTask;
//    }

//    /// <inheritdoc/>
//    public sealed override Task<int> ReadAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
//    {
//        if (cancellationToken.IsCancellationRequested)
//        {
//            return Task.FromCanceled<int>(cancellationToken);
//        }

//        try
//        {
//            int result = Read(buffer!, offset, count);

//            return Task.FromResult(result);
//        }
//        catch (OperationCanceledException e)
//        {
//            return Task.FromCanceled<int>(e.CancellationToken);
//        }
//        catch (Exception e)
//        {
//            return Task.FromException<int>(e);
//        }
//    }

//#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
//    /// <inheritdoc/>
//    public sealed override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
//    {
//        if (cancellationToken.IsCancellationRequested)
//        {
//            return new(Task.FromCanceled<int>(cancellationToken));
//        }

//        try
//        {
//            int result = Read(buffer.Span);

//            return new(result);
//        }
//        catch (OperationCanceledException e)
//        {
//            return new(Task.FromCanceled<int>(e.CancellationToken));
//        }
//        catch (Exception e)
//        {
//            return new(Task.FromException<int>(e));
//        }
//    }
//#endif

//    /// <inheritdoc/>
//    public sealed override Task WriteAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
//    {
//        if (cancellationToken.IsCancellationRequested)
//        {
//            return Task.FromCanceled(cancellationToken);
//        }

//        throw new NotSupportedException("Stream does not support writing.");
//    }

//#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
//    /// <inheritdoc/>
//    public sealed override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
//    {
//        if (cancellationToken.IsCancellationRequested)
//        {
//            return new(Task.FromCanceled(cancellationToken));
//        }

//        throw new NotSupportedException("Stream does not support writing.");
//    }
//#endif

//    /// <inheritdoc/>
//    public sealed override int Read(byte[]? buffer, int offset, int count)
//    {
//        ValidateDisposed(disposed);
//        ValidateBuffer(buffer, offset, count);

//        var position = GetLogicPosition();
//        int bytesAvailable = _buffer.Length - this.position;
//        int bytesCopied = Math.Min(bytesAvailable, count);

//        Span<byte> source = _buffer.AsSpan();
//        int start;
//        if (reverse)
//        {
//            start = position - bytesCopied;
//        }
//        else
//        {
//            start = position;
//        }
//        source = source.Slice(start, bytesCopied);
//        Span<byte> destination = buffer.AsSpan(offset, bytesCopied);

//        byte[]? shared = null;
//        try
//        {
//            if (reverse)
//            {
//                shared = ArrayPool<byte>.Shared.Rent(source.Length);
//                var shared_span = shared.AsSpan(0, source.Length);
//                source.CopyTo(shared_span);
//                shared_span.Reverse();
//                source = shared_span;
//            }
//            source.CopyTo(destination);

//            AddPosition(bytesCopied);

//            return bytesCopied;
//        }
//        finally
//        {
//            if (shared != null)
//                ArrayPool<byte>.Shared.Return(shared);
//        }
//    }

//    /// <inheritdoc/>
//    public sealed override void CopyTo(Stream destination, int bufferSize)
//    {
//        ValidateDisposed(disposed);

//        var position = GetLogicPosition();
//        int bytesAvailable = _buffer.Length - this.position;
//        int bytesCopied = bytesAvailable;

//        Span<byte> source = _buffer.AsSpan();
//        int start;
//        if (reverse)
//        {
//            start = position - bytesCopied;
//        }
//        else
//        {
//            start = position;
//        }
//        source = source.Slice(start, bytesCopied);

//        byte[]? shared = null;
//        try
//        {
//            if (reverse)
//            {
//                shared = ArrayPool<byte>.Shared.Rent(source.Length);
//                var shared_span = shared.AsSpan(0, source.Length);
//                source.CopyTo(shared_span);
//                shared_span.Reverse();
//                source = shared_span;
//            }
//            destination.Write(source);

//            AddPosition(bytesCopied);
//        }
//        finally
//        {
//            if (shared != null)
//                ArrayPool<byte>.Shared.Return(shared);
//        }
//    }

//    /// <inheritdoc/>
//    public sealed override long Seek(long offset, SeekOrigin origin)
//    {
//        ValidateDisposed(disposed);

//        long index = origin switch
//        {
//            SeekOrigin.Begin => offset,
//            SeekOrigin.Current => position + offset,
//            SeekOrigin.End => _buffer.Length + offset,
//            _ => ThrowArgumentExceptionForSeekOrigin(),
//        };

//        ValidatePosition(index, _buffer.Length);

//        position = unchecked((int)index);

//        return index;
//    }

//    /// <inheritdoc/>
//    public sealed override void SetLength(long value)
//    {
//        throw new NotSupportedException("The requested operation is not supported for this stream.");
//    }

//    /// <inheritdoc/>
//    public sealed override void Write(byte[] buffer, int offset, int count)
//    {
//        throw new NotSupportedException("Stream does not support writing.");
//    }
//}

//partial class ReadOnlyMemoryStream // IDisposable
//{
//    /// <inheritdoc/>
//    protected sealed override void Dispose(bool disposing)
//    {
//        if (!disposed)
//        {
//            if (disposing)
//            {
//                if (_buffer != null)
//                {
//                    if (_buffer.Length != 0)
//                    {
//                        _buffer.AsSpan().Clear();
//                    }
//                    _buffer = null!;
//                }
//            }

//            disposed = true;
//        }
//    }
//}

//partial class ReadOnlyMemoryStream // Deserialize
//{
//    /// <inheritdoc cref="MemoryPackSerializer.Deserialize{T}(ReadOnlySpan{byte}, MemoryPackSerializerOptions?)"/>
//    public T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(MemoryPackSerializerOptions? options = null)
//    {
//        ValidateDisposed(disposed);

//        Span<byte> buffer = _buffer;
//        byte[]? shared = null;
//        try
//        {
//            if (reverse)
//            {
//                shared = ArrayPool<byte>.Shared.Rent(_buffer.Length);
//                var shared_span = buffer = shared.AsSpan(0, _buffer.Length);
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

//    /// <inheritdoc cref="SystemTextJsonSerializer.Deserialize{TValue}(Stream, JsonTypeInfo{TValue})"/>
//    public T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(JsonTypeInfo<T> jsonTypeInfo)
//    {
//        ValidateDisposed(disposed);

//        return SystemTextJsonSerializer.Deserialize(this, jsonTypeInfo);
//    }

//    /// <inheritdoc cref="SystemTextJsonSerializer.Deserialize{TValue}(Stream, SystemTextJsonSerializerOptions?)"/>
//#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
//    [RequiresUnreferencedCode(Serializable.SerializationUnreferencedCodeMessage)]
//    [RequiresDynamicCode(Serializable.SerializationRequiresDynamicCodeMessage)]
//#endif
//    public T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(SystemTextJsonSerializerOptions? options)
//    {
//        ValidateDisposed(disposed);

//        return SystemTextJsonSerializer.Deserialize<T>(this, options);
//    }
//}

//partial class ReadOnlyMemoryStream // Validate
//{
//    /// <summary>
//    /// Validates that a given <see cref="Stream"/> instance hasn't been disposed.
//    /// </summary>
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    static void ValidateDisposed(bool disposed)
//    {
//        if (disposed)
//        {
//            ThrowObjectDisposedException();
//        }
//    }

//    /// <summary>
//    /// Validates the <see cref="Stream.Position"/> argument (it needs to be in the [0, length]) range.
//    /// </summary>
//    /// <param name="position">The new <see cref="Stream.Position"/> value being set.</param>
//    /// <param name="length">The maximum length of the target <see cref="Stream"/>.</param>
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    static void ValidatePosition(long position, int length)
//    {
//        if ((ulong)position > (ulong)length)
//        {
//            ThrowArgumentOutOfRangeExceptionForPosition();
//        }
//    }

//    /// <summary>
//    /// Validates the <see cref="Stream.Read(byte[],int,int)"/> or <see cref="Stream.Write(byte[],int,int)"/> arguments.
//    /// </summary>
//    /// <param name="buffer">The target array.</param>
//    /// <param name="offset">The offset within <paramref name="buffer"/>.</param>
//    /// <param name="count">The number of elements to process within <paramref name="buffer"/>.</param>
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    static void ValidateBuffer(byte[]? buffer, int offset, int count)
//    {
//        if (buffer is null)
//        {
//            ThrowArgumentNullExceptionForBuffer();
//        }

//        if (offset < 0)
//        {
//            ThrowArgumentOutOfRangeExceptionForOffset();
//        }

//        if (count < 0)
//        {
//            ThrowArgumentOutOfRangeExceptionForCount();
//        }

//        if (offset + count > buffer!.Length)
//        {
//            ThrowArgumentExceptionForLength();
//        }
//    }
//}

//partial class ReadOnlyMemoryStream // ThrowExceptions
//{
//    /// <summary>
//    /// Throws an <see cref="ArgumentOutOfRangeException"/> when setting the <see cref="Stream.Position"/> property.
//    /// </summary>
//    [DoesNotReturn]
//    static void ThrowArgumentOutOfRangeExceptionForPosition()
//    {
//        throw new ArgumentOutOfRangeException(nameof(Stream.Position), "The value for the property was not in the valid range.");
//    }

//    /// <summary>
//    /// Throws an <see cref="ObjectDisposedException"/> when using a disposed <see cref="Stream"/> instance.
//    /// </summary>
//    [DoesNotReturn]
//    static void ThrowObjectDisposedException()
//    {
//        throw new ObjectDisposedException(nameof(_buffer), "The current stream has already been disposed");
//    }

//    /// <summary>
//    /// Throws an <see cref="ArgumentException"/> when using an invalid seek mode.
//    /// </summary>
//    /// <returns>Nothing, as this method throws unconditionally.</returns>
//    [DoesNotReturn]
//    static long ThrowArgumentExceptionForSeekOrigin()
//    {
//        throw new ArgumentException("The input seek mode is not valid.", "origin");
//    }

//    /// <summary>
//    /// Throws an <see cref="ArgumentNullException"/> when an input buffer is <see langword="null"/>.
//    /// </summary>
//    [DoesNotReturn]
//    static void ThrowArgumentNullExceptionForBuffer()
//    {
//        throw new ArgumentNullException(nameof(_buffer), "The buffer is null.");
//    }

//    /// <summary>
//    /// Throws an <see cref="ArgumentOutOfRangeException"/> when the input count is negative.
//    /// </summary>
//    [DoesNotReturn]
//    static void ThrowArgumentOutOfRangeExceptionForOffset()
//    {
//        throw new ArgumentOutOfRangeException("offset", "Offset can't be negative.");
//    }

//    /// <summary>
//    /// Throws an <see cref="ArgumentOutOfRangeException"/> when the input count is negative.
//    /// </summary>
//    [DoesNotReturn]
//    static void ThrowArgumentOutOfRangeExceptionForCount()
//    {
//        throw new ArgumentOutOfRangeException("count", "Count can't be negative.");
//    }

//    /// <summary>
//    /// Throws an <see cref="ArgumentException"/> when the sum of offset and count exceeds the length of the target buffer.
//    /// </summary>
//    [DoesNotReturn]
//    static void ThrowArgumentExceptionForLength()
//    {
//        throw new ArgumentException("The sum of offset and count can't be larger than the buffer length.", "buffer");
//    }
//}

//partial class ReadOnlyMemoryStream : IReadOnlyList<byte>
//{
//    /// <inheritdoc/>
//    public int Count
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        get
//        {
//            ValidateDisposed(disposed);
//            return _buffer.Length;
//        }
//    }

//    /// <inheritdoc/>
//    public IEnumerator<byte> GetEnumerator()
//    {
//        return ((IEnumerable<byte>)_buffer).GetEnumerator();
//    }

//    /// <inheritdoc/>
//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return _buffer.GetEnumerator();
//    }
//}

//partial class ReadOnlyMemoryStream : IList<byte>
//{
//    /// <inheritdoc/>
//    public byte this[int index]
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        get
//        {
//            ValidateDisposed(disposed);
//            return _buffer[index];
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        set
//        {
//            ValidateDisposed(disposed);
//            if (reverse)
//            {
//                _buffer[^index] = value;
//            }
//            else
//            {
//                _buffer[index] = value;
//            }
//        }
//    }

//    /// <inheritdoc/>
//    bool ICollection<byte>.IsReadOnly => true;

//    /// <inheritdoc/>
//    void ICollection<byte>.Add(byte item) => throw new NotSupportedException("The requested operation is not supported for this stream.");

//    /// <inheritdoc/>
//    void ICollection<byte>.Clear() => Dispose();

//    /// <inheritdoc/>
//    public bool Contains(byte item)
//    {
//        ValidateDisposed(disposed);

//        var result = _buffer.Contains(item);
//        return result;
//    }

//    /// <inheritdoc/>
//    public void CopyTo(byte[] array, int arrayIndex)
//    {
//        ValidateDisposed(disposed);

//        Span<byte> buffer = _buffer;
//        byte[]? shared = null;
//        try
//        {
//            if (reverse)
//            {
//                shared = ArrayPool<byte>.Shared.Rent(_buffer.Length);
//                var shared_span = buffer = shared.AsSpan(0, _buffer.Length);
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

//        buffer.CopyTo(array.AsSpan(arrayIndex));
//    }

//    /// <inheritdoc/>
//    public int IndexOf(byte item)
//    {
//        ValidateDisposed(disposed);

//        var result = Array.IndexOf(_buffer, item);
//        if (reverse)
//        {
//            result = _buffer.Length - result;
//        }
//        return result;
//    }

//    /// <inheritdoc/>
//    void IList<byte>.Insert(int index, byte item) => throw new NotSupportedException("The requested operation is not supported for this stream.");

//    /// <inheritdoc/>
//    bool ICollection<byte>.Remove(byte item) => throw new NotSupportedException("The requested operation is not supported for this stream.");

//    /// <inheritdoc/>
//    void IList<byte>.RemoveAt(int index) => throw new NotSupportedException("The requested operation is not supported for this stream.");
//}