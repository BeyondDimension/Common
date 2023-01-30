#if __HAVE_N_JSON__
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonConverter = Newtonsoft.Json.JsonConverter;
#endif
#if !__NOT_HAVE_S_JSON__
using SJsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;
using SJsonSerializer = System.Text.Json.JsonSerializer;
using SJsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
#endif
#if __HAVE_N_JSON__
using static Newtonsoft.Json.JsonConvert;
#endif
using static System.Serializable;

namespace System
{
    /// <summary>
    /// 序列化、反序列化 助手类
    /// </summary>
    [Serializable]
    public static class Serializable
    {
        /// <summary>
        /// 序列化程式实现种类
        /// </summary>
        public enum ImplType
        {
            /// <summary>
            /// Newtonsoft.Json
            /// <para>https://github.com/JamesNK/Newtonsoft.Json</para>
            /// </summary>
#if !__HAVE_N_JSON__
            [Obsolete("use SystemTextJson")]
#endif
            NewtonsoftJson,

            /// <summary>
            /// MessagePack is an efficient binary serialization format. It lets you exchange data among multiple languages like JSON. But it's faster and smaller. Small integers are encoded into a single byte, and typical short strings require only one extra byte in addition to the strings themselves.
            /// <para>https://msgpack.org/</para>
            /// <para>https://github.com/neuecc/MessagePack-CSharp</para>
            /// </summary>
            MessagePack,

            /// <summary>
            /// System.Text.Json
            /// <para>仅用于 .Net Core 3+ / Web，因 Emoji 字符被转义</para>
            /// <para>https://github.com/dotnet/corefx/tree/v3.1.5/src/System.Text.Json</para>
            /// <para>https://github.com/dotnet/runtime/tree/v5.0.0-preview.6.20305.6/src/libraries/System.Text.Json</para>
            /// </summary>
            SystemTextJson,

            /// <summary>
            /// MemoryPack is Zero encoding extreme performance binary serializer for C# and Unity.
            /// <para>https://github.com/Cysharp/MemoryPack</para>
            /// </summary>
            MemoryPack,
        }

        /// <summary>
        /// JSON 序列化程式实现种类
        /// </summary>
        public enum JsonImplType
        {
            /// <inheritdoc cref="ImplType.NewtonsoftJson"/>
#if !__HAVE_N_JSON__
            [Obsolete("use SystemTextJson")]
#endif
            NewtonsoftJson = ImplType.NewtonsoftJson,

            /// <inheritdoc cref="ImplType.SystemTextJson"/>
            SystemTextJson = ImplType.SystemTextJson,
        }

        #region DefaultJsonImplType

        /// <summary>
        /// JSON 序列化程式 实现，可设置使用 Newtonsoft.Json 或 System.Text.Json
        /// </summary>
        public static JsonImplType DefaultJsonImplType { get; set; } = GetDefaultJsonImplType();

        static JsonImplType GetDefaultJsonImplType()
        {
#if !__HAVE_N_JSON__
            return JsonImplType.SystemTextJson;
#else
            if (Environment.Version.Major <= 5)
            {
                return JsonImplType.NewtonsoftJson;
            }
            return JsonImplType.SystemTextJson;
#endif
        }

        #endregion

        #region Serialize(序列化)

        /// <summary>
        /// (Serialize)JSON 序列化
        /// </summary>
        /// <param name="implType"></param>
        /// <param name="value"></param>
        /// <param name="inputType"></param>
        /// <param name="writeIndented"></param>
        /// <param name="ignoreNullValues"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SJSON(JsonImplType implType, object? value, Type? inputType = null, bool writeIndented = false, bool ignoreNullValues = false)
        {
            switch (implType)
            {
                case JsonImplType.SystemTextJson:
                    var options = new SJsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = writeIndented,
                        //IgnoreNullValues = ignoreNullValues
                    };
                    if (ignoreNullValues)
                    {
                        options.DefaultIgnoreCondition = SJsonIgnoreCondition.WhenWritingNull;
                    }
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                    return SJsonSerializer.Serialize(value, inputType ?? value?.GetType() ?? typeof(object), options);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                default:
#if !__HAVE_N_JSON__
                    throw new NotSupportedException();
#else
                    var formatting = writeIndented ? Formatting.Indented : Formatting.None;
                    var settings = ignoreNullValues ? new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    }
                    : null;
                    return SerializeObject(value, inputType, formatting, settings);
#endif
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
        public static string SJSON(object? value, Type? inputType = null, bool writeIndented = false, bool ignoreNullValues = false)
            => SJSON(DefaultJsonImplType, value, inputType, writeIndented, ignoreNullValues);

#if !__NOT_HAVE_MESSAGEPACK__

#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
        public static readonly MessagePackSerializerOptions lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter

        /// <summary>
        /// (Serialize)MessagePack 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SMP<T>(T value, CancellationToken cancellationToken = default)
            => MessagePackSerializer.Serialize(value, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SMP(Type type, object value, CancellationToken cancellationToken = default)
            => MessagePackSerializer.Serialize(type, value, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SMP<T>(Stream stream, T value, CancellationToken cancellationToken = default)
            => MessagePackSerializer.Serialize(stream, value, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SMP(Type type, Stream stream, object value, CancellationToken cancellationToken = default)
            => MessagePackSerializer.Serialize(type, stream, value, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task SMPAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
            => MessagePackSerializer.SerializeAsync(stream, value, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="SMP{T}(T, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task SMPAsync(Type type, Stream stream, object value, CancellationToken cancellationToken = default)
            => MessagePackSerializer.SerializeAsync(type, stream, value, options: lz4Options, cancellationToken: cancellationToken);

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

        #endregion

        #region Deserialize(反序列化)

        /// <summary>
        /// (Deserialize)JSON 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [return: MaybeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DJSON<T>(JsonImplType implType, string value)
        {
            return implType switch
            {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                JsonImplType.SystemTextJson => SJsonSerializer.Deserialize<T>(value),
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                _ =>
#if !__HAVE_N_JSON__
                throw new NotSupportedException(),
#else
                DeserializeObject<T>(value),
#endif
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
        public static T DJSON<T>(string value) => DJSON<T>(DefaultJsonImplType, value);

#if !__NOT_HAVE_MESSAGEPACK__

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
            => MessagePackSerializer.Deserialize<T>(buffer, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="DMP{T}(byte[], CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? DMP(Type type, byte[] buffer, CancellationToken cancellationToken = default)
         => MessagePackSerializer.Deserialize(type, buffer, options: lz4Options, cancellationToken: cancellationToken);

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
            => MessagePackSerializer.Deserialize<T>(buffer, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? DMP(Type type, Stream buffer, CancellationToken cancellationToken = default)
            => MessagePackSerializer.Deserialize(type, buffer, options: lz4Options, cancellationToken: cancellationToken);

        /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<T?> DMPAsync<T>(Stream buffer, CancellationToken cancellationToken = default)
        {
            T? r = await MessagePackSerializer.DeserializeAsync<T>(buffer, options: lz4Options, cancellationToken: cancellationToken);
            return r;
        }

        /// <inheritdoc cref="DMP{T}(Stream, CancellationToken)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<object?> DMPAsync(Type type, Stream buffer, CancellationToken cancellationToken = default)
            => MessagePackSerializer.DeserializeAsync(type, buffer, options: lz4Options, cancellationToken: cancellationToken);

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
                var buffer = value.Base64UrlDecodeToByteArray();
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
                var buffer = value.Base64UrlDecodeToByteArray();
                return DMP(type, buffer, cancellationToken);
            }
            return default;
        }

#endif

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

        #endregion

        /// <summary>
        /// 将 Json 字符串格式化缩减后输出
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="implType"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull("jsonStr")]
        public static string? GetIndented(string? jsonStr, JsonImplType implType = JsonImplType.SystemTextJson)
        {
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                try
                {
                    switch (implType)
                    {
                        case JsonImplType.SystemTextJson:
                            var jsonDoc = JsonDocument.Parse(jsonStr);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                            return SJsonSerializer.Serialize(jsonDoc, jsonDoc.GetType(), JsonSerializerCompatOptions.WriteIndented);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                        default:
#if !__HAVE_N_JSON__
                            throw new NotSupportedException();
#else
                            var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(jsonStr);
                            return SerializeObject(jsonObj, Formatting.Indented);
#endif
                    }
                }
                catch
                {

                }
            }
            return jsonStr;
        }

        /// <summary>
        /// 使用序列化将对象克隆一份新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull("obj")]
        public static T? Clone<T>(T? obj) where T : notnull
        {
            if (EqualityComparer<T?>.Default.Equals(obj, default)) return default;
#if __NOT_HAVE_MESSAGEPACK__
            var jsonStr = SJSON(obj);
            return DJSON<T>(jsonStr);
#else
            try
            {
                var bytes = MessagePackSerializer.Serialize(obj);
                return MessagePackSerializer.Deserialize<T>(bytes);
            }
            catch
            {
                try
                {
                    var bytes = MemoryPackSerializer.Serialize(obj);
                    return MemoryPackSerializer.Deserialize<T>(bytes);
                }
                catch
                {
                    var jsonStr = SJSON(obj);
                    return DJSON<T>(jsonStr);
                }
            }
#endif
        }

#if __HAVE_N_JSON__
        static readonly Lazy<JsonSerializerSettings> mIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings = new(GetIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings);

        static JsonSerializerSettings GetIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings() => new()
        {
            ContractResolver = new IgnoreJsonPropertyContractResolver(),
            Converters = new List<NJsonConverter>
            {
                new StringEnumConverter(),
            },
        };

        public static JsonSerializerSettings IgnoreJsonPropertyContractResolverWithStringEnumConverterSettings => mIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings.Value;

        /// <summary>
        /// 序列化 JSON 模型，使用原键名
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SJSON_Original(object? value)
            => SerializeObject(value, Formatting.Indented, IgnoreJsonPropertyContractResolverWithStringEnumConverterSettings);

        /// <summary>
        /// 反序列化 JSON 模型，使用原键名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [return: MaybeNull]
        public static T DJSON_Original<T>(string value)
            => DeserializeObject<T>(value, IgnoreJsonPropertyContractResolverWithStringEnumConverterSettings);
#endif
    }

    public interface ICloneableSerializable
    {
    }

    public static partial class SerializableExtensions
    {
        /// <inheritdoc cref="Serializable.Clone{T}(T)"/>
        public static T Clone<T>(this T obj) where T : ICloneableSerializable => Serializable.Clone(obj);

        /// <summary>
        /// 将 [序列化程式实现种类] 转换为 [JSON 序列化程式实现种类]
        /// </summary>
        /// <param name="enum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryConvert(this ImplType @enum, out JsonImplType value)
        {
            switch (@enum)
            {
                case ImplType.NewtonsoftJson:
                    value = JsonImplType.NewtonsoftJson;
                    return true;

                case ImplType.SystemTextJson:
                    value = JsonImplType.SystemTextJson;
                    return true;

                default:
                    value = default;
                    return false;
            }
        }

        /// <summary>
        /// 将 [JSON 序列化程式实现种类] 转换为 [序列化程式实现种类]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ImplType Convert(this JsonImplType value)
        {
            return value switch
            {
                JsonImplType.SystemTextJson => ImplType.SystemTextJson,
                _ => ImplType.NewtonsoftJson,
            };
        }
    }
}

#if __HAVE_N_JSON__
namespace Newtonsoft.Json.Serialization
{
    /// <summary>
    /// 将忽略 <see cref="N_JsonProperty"/> 属性
    /// </summary>
    public sealed class IgnoreJsonPropertyContractResolver : DefaultContractResolver
    {
        readonly bool useCamelCase;

        public IgnoreJsonPropertyContractResolver(bool useCamelCase = false)
        {
            this.useCamelCase = useCamelCase;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperties(type, memberSerialization);
            foreach (var item in result)
            {
                item.PropertyName = item.UnderlyingName == null ? null :
                    (useCamelCase ?
                        JsonNamingPolicy.CamelCase.ConvertName(item.UnderlyingName) :
                        item.UnderlyingName);
            }
            return result;
        }
    }
}
#endif