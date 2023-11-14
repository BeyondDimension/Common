using static System.Serializable;

#pragma warning disable IDE0161 // 转换为文件范围限定的 namespace
namespace System
{
    /// <summary>
    /// 序列化、反序列化 助手类
    /// </summary>
    [Serializable]
    public static partial class Serializable
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
            NewtonsoftJson = ImplType.NewtonsoftJson,

            /// <inheritdoc cref="ImplType.SystemTextJson"/>
            SystemTextJson = ImplType.SystemTextJson,
        }

        #region DefaultJsonImplType

        /// <summary>
        /// JSON 序列化程式 实现，可设置使用 Newtonsoft.Json 或 System.Text.Json
        /// </summary>
        public static JsonImplType DefaultJsonImplType { get; set; } = GetDefaultJsonImplType();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static JsonImplType GetDefaultJsonImplType()
        {
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
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

        /// <summary>
        /// 使用序列化将对象克隆一份新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(obj))]
        public static T? Clone<T>(T? obj) where T : notnull
        {
            if (EqualityComparer<T?>.Default.Equals(obj, default))
                return default;

#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
            try
            {
                var bytes = MemoryPackSerializer.Serialize(obj);
                return MemoryPackSerializer.Deserialize<T>(bytes);
            }
            catch
            {
            }
#endif

#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
            try
            {
                var bytes = MessagePackSerializer.Serialize(obj);
                return MessagePackSerializer.Deserialize<T>(bytes);
            }
            catch
            {
            }
#endif

#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var json = SJSON(obj);
            return DJSON<T>(json);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        }
    }

    /// <summary>
    /// 用于序列化克隆对象，使用扩展函数 <see cref="SerializableExtensions.Clone{T}(T)"/>
    /// </summary>
    public interface ICloneableSerializable
    {
    }

    public static partial class SerializableExtensions
    {
        /// <inheritdoc cref="Serializable.Clone{T}(T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Clone<T>(this T obj) where T : ICloneableSerializable => Serializable.Clone(obj);

        /// <summary>
        /// 将 [序列化程式实现种类] 转换为 [JSON 序列化程式实现种类]
        /// </summary>
        /// <param name="enum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    }
}