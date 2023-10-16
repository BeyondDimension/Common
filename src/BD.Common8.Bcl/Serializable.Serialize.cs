namespace System;

public static partial class Serializable // Serialize(序列化)
{
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
#pragma warning disable SA1600 // Elements should be documented
    // https://github.com/dotnet/runtime/blob/v8.0.0-rc.1.23419.4/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonSerializer.Helpers.cs#L13-L14
    public const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    public const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";
#pragma warning restore SA1600 // Elements should be documented
#endif

    [DebuggerDisplay("writeIndented={writeIndented}, ignoreNullValues={ignoreNullValues}")]
    readonly struct SharedJsonSerializerOptions(bool writeIndented, bool ignoreNullValues)
    {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SystemTextJsonSerializerOptions GetSystemTextJsonSerializerOptions()
        {
            var options = new SystemTextJsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = writeIndented,
            };
            if (ignoreNullValues)
            {
                options.DefaultIgnoreCondition = SystemTextJsonIgnoreCondition.WhenWritingNull;
            }
            return options;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NewtonsoftJsonSerializerSettings? GetNewtonsoftJsonSerializerSettings()
        {
            var settings = ignoreNullValues ? new NewtonsoftJsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            } : null;
            return settings;
        }
    }

#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    static readonly ConcurrentDictionary<SharedJsonSerializerOptions, SystemTextJsonSerializerOptions> SystemTextJsonSerializerOptionsDictionary = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static SystemTextJsonSerializerOptions GetSystemTextJsonSerializerOptions(bool writeIndented, bool ignoreNullValues)
    {
        SharedJsonSerializerOptions o = new(writeIndented, ignoreNullValues);
        if (!SystemTextJsonSerializerOptionsDictionary.TryGetValue(o, out var value))
        {
            value = o.GetSystemTextJsonSerializerOptions();
            SystemTextJsonSerializerOptionsDictionary.TryAdd(o, value);
        }
        return value;
    }

#endif
    static readonly ConcurrentDictionary<SharedJsonSerializerOptions, NewtonsoftJsonSerializerSettings?> NewtonsoftJsonSerializerSettingsDictionary = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static NewtonsoftJsonSerializerSettings? GetNewtonsoftJsonSerializerSettings(bool ignoreNullValues)
    {
        SharedJsonSerializerOptions o = new(default, ignoreNullValues);
        if (!NewtonsoftJsonSerializerSettingsDictionary.TryGetValue(o, out var value))
        {
            value = o.GetNewtonsoftJsonSerializerSettings();
            NewtonsoftJsonSerializerSettingsDictionary.TryAdd(o, value);
        }
        return value;
    }

    /// <summary>
    /// (Serialize)JSON 序列化
    /// </summary>
    /// <param name="implType"></param>
    /// <param name="value"></param>
    /// <param name="inputType"></param>
    /// <param name="writeIndented"></param>
    /// <param name="ignoreNullValues"></param>
    /// <returns></returns>
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    public static string SJSON(JsonImplType implType, object? value, Type? inputType = null, bool writeIndented = false, bool ignoreNullValues = false)
    {
        switch (implType)
        {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
            case JsonImplType.SystemTextJson:
                var options = GetSystemTextJsonSerializerOptions(writeIndented, ignoreNullValues);
                return SystemTextJsonSerializer.Serialize(value, inputType ?? value?.GetType() ?? typeof(object), options);
#endif
            default:
                var formatting = writeIndented ? NewtonsoftJsonFormatting.Indented : NewtonsoftJsonFormatting.None;
                var settings = GetNewtonsoftJsonSerializerSettings(ignoreNullValues);
                return NewtonsoftJsonConvert.SerializeObject(value, inputType, formatting, settings);
        }
    }

    /// <summary>
    /// (Serialize)JSON 序列化
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inputType"></param>
    /// <param name="writeIndented"></param>
    /// <param name="ignoreNullValues"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    public static string SJSON(object? value, Type? inputType = null, bool writeIndented = false, bool ignoreNullValues = false)
        => SJSON(DefaultJsonImplType, value, inputType, writeIndented, ignoreNullValues);

#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)

    /// <inheritdoc cref="MessagePackCompression.Lz4BlockArray"/>
    public static MessagePackSerializerOptions Lz4Options() => MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

    /// <summary>
    /// (Serialize)MessagePack 序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] SMP<T>(T value, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Serialize(value, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] SMP(Type type, object value, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Serialize(type, value, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SMP<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Serialize(stream, value, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SMP(Type type, Stream stream, object value, CancellationToken cancellationToken = default)
        => MessagePackSerializer.Serialize(type, stream, value, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SMPAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        => MessagePackSerializer.SerializeAsync(stream, value, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SMPAsync(Type type, Stream stream, object value, CancellationToken cancellationToken = default)
        => MessagePackSerializer.SerializeAsync(type, stream, value, options: Lz4Options(), cancellationToken: cancellationToken);

    /// <summary>
    /// (Serialize)MessagePack 序列化 + Base64Url Encode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? SMPB64U<T>(T value, CancellationToken cancellationToken = default)
    {
        if (value == null) return null;
        var byteArray = SMP(value, cancellationToken);
        return byteArray.Base64UrlEncode_Nullable();
    }

#endif

#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)

    /// <summary>
    /// (Serialize)MemoryPack 序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] SMP2<T>(T value)
        => MemoryPackSerializer.Serialize(value);

    /// <inheritdoc cref="SMP2{T}(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] SMP2(Type type, object value)
        => MemoryPackSerializer.Serialize(type, value);

    /// <inheritdoc cref="SMP2{T}(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask SMP2Async<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        => MemoryPackSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken);

    /// <inheritdoc cref="SMP2{T}(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask SMP2Async(Type type, Stream stream, object value, CancellationToken cancellationToken = default)
        => MemoryPackSerializer.SerializeAsync(type, stream, value, cancellationToken: cancellationToken);

    /// <summary>
    /// (Serialize)MemoryPack 序列化 + Base64Url Encode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? SMP2B64U<T>(T value)
    {
        if (value == null) return null;
        var byteArray = SMP2(value);
        return byteArray.Base64UrlEncode_Nullable();
    }

#endif
}
