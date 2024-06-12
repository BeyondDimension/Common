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
    public static T DJSON<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(JsonImplType implType, string value)
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
    public static T DJSON<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string value) => DJSON<T>(DefaultJsonImplType, value);

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
    public static T DMP<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(byte[] buffer, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Deserialize<T>(buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="DMP{T}(byte[], CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMP([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, byte[] buffer, CancellationToken cancellationToken = default)
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
    public static T DMP<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Stream buffer, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Deserialize<T>(buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMP([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, Stream buffer, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Deserialize(type, buffer, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<T?> DMPAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Stream buffer, CancellationToken cancellationToken = default)
    {
        T? r = await MessagePackSerializer.DeserializeAsync<T>(buffer, options: Lz4Options(), cancellationToken: cancellationToken);
        return r;
    }

    /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<object?> DMPAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, Stream buffer, CancellationToken cancellationToken = default)
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
    public static T DMPB64U<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string? value, CancellationToken cancellationToken = default)
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
    public static object? DMPB64U([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, string? value, CancellationToken cancellationToken = default)
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
    public static T DMP2<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ReadOnlySpan<byte> buffer)
        => MemoryPackSerializer.Deserialize<T>(buffer);

    /// <inheritdoc cref="DMP2{T}(ReadOnlySpan{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? DMP2([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, ReadOnlySpan<byte> buffer)
     => MemoryPackSerializer.Deserialize(type, buffer);

    /// <summary>
    /// (Deserialize)MemoryPack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<T?> DMP2Async<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Stream buffer, CancellationToken cancellationToken = default)
    {
        T? r = await MemoryPackSerializer.DeserializeAsync<T>(buffer, cancellationToken: cancellationToken);
        return r;
    }

    /// <inheritdoc cref="DMP2Async{T}(Stream, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<object?> DMP2Async([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, Stream buffer, CancellationToken cancellationToken = default)
        => MemoryPackSerializer.DeserializeAsync(type, buffer, cancellationToken: cancellationToken);

    /// <summary>
    /// (Deserialize)MessagePack 反序列化 + Base64Url Decode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: MaybeNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DMP2B64U<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string? value)
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
    public static object? DMP2B64U([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var buffer = value.Base64UrlDecodeToByteArray();
            return DMP(type, buffer);
        }
        return default;
    }

#endif

    /// <summary>
    /// (Deserialize)Xml
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="textReader"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public static T? DXml<T>(TextReader textReader)
    {
        var xmlSerializer = GetXmlSerializer(typeof(T));
        var result = xmlSerializer.Deserialize(textReader);
        return (T?)result;
    }
}