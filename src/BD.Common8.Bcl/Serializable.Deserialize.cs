namespace System;

public static partial class Serializable // Deserialize(反序列化)
{
    /// <summary>
    /// (Deserialize)JSON 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="implType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    public static T DJSON<T>(JsonImplType implType, string value)
    {
        return implType switch
        {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
            JsonImplType.SystemTextJson => SystemTextJsonSerializer.Deserialize<T>(value),
#endif
            _ => NewtonsoftJsonConvert.DeserializeObject<T>(value),
        };
    }

    /// <summary>
    /// (Deserialize)JSON 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    public static T DJSON<T>(string value) => DJSON<T>(DefaultJsonImplType, value);

#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)

    /// <summary>
    /// (Deserialize)MessagePack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DMP<T>(byte[] buffer, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Deserialize<T>(buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="DMP{T}(byte[], CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMP(Type type, byte[] buffer, CancellationToken cancellationToken = default)
     => MessagePackSerializer.Deserialize(type, buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <summary>
    /// (Deserialize)MessagePack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DMP<T>(Stream buffer, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Deserialize<T>(buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMP(Type type, Stream buffer, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Deserialize(type, buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<T?> DMPAsync<T>(Stream buffer, CancellationToken cancellationToken = default)
    {
        T? r = await MessagePackSerializer.DeserializeAsync<T>(buffer, options: Lz4Options(), cancellationToken: cancellationToken);
        return r;
    }

    /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<object?> DMPAsync(Type type, Stream buffer, CancellationToken cancellationToken = default)
        => MessagePackSerializer.DeserializeAsync(type, buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <summary>
    /// (Deserialize)MessagePack 反序列化 + Base64Url Decode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DMPB64U<T>(string? value, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var buffer = value!.Base64UrlDecodeToByteArray();
            return DMP<T>(buffer, cancellationToken);
        }
        return default;
    }

    /// <inheritdoc cref="DMPB64U{T}(string?, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMPB64U(Type type, string? value, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var buffer = value!.Base64UrlDecodeToByteArray();
            return DMP(type, buffer, cancellationToken);
        }
        return default;
    }

#endif

#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)

    /// <summary>
    /// (Deserialize)MemoryPack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DMP2<T>(byte[] buffer)
        => MemoryPackSerializer.Deserialize<T>(buffer);

    /// <inheritdoc cref="DMP2{T}(byte[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMP2(Type type, byte[] buffer)
     => MemoryPackSerializer.Deserialize(type, buffer);

    /// <summary>
    /// (Deserialize)MemoryPack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<T?> DMP2Async<T>(Stream buffer, CancellationToken cancellationToken = default)
    {
        T? r = await MemoryPackSerializer.DeserializeAsync<T>(buffer, cancellationToken: cancellationToken);
        return r;
    }

    /// <inheritdoc cref="DMP2Async{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<object?> DMP2Async(Type type, Stream buffer, CancellationToken cancellationToken = default)
        => MemoryPackSerializer.DeserializeAsync(type, buffer, cancellationToken: cancellationToken);

    /// <summary>
    /// (Deserialize)MessagePack 反序列化 + Base64Url Decode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DMP2B64U<T>(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var buffer = value.Base64UrlDecodeToByteArray();
            return DMP2<T>(buffer);
        }
        return default;
    }

    /// <inheritdoc cref="DMP2B64U{T}(string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMP2B64U(Type type, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var buffer = value.Base64UrlDecodeToByteArray();
            return DMP(type, buffer);
        }
        return default;
    }

#endif
}