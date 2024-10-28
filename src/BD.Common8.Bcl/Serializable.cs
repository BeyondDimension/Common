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
#if !NO_SYSTEM_TEXT_JSON

#if !NO_MEMORYPACK && (!NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER))
        /// <summary>
        /// 根据已有 Json 设置项添加或重新创建带有预设的配置
        /// </summary>
        /// <param name="baseOptions"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public static SystemTextJsonSerializerOptions CreateOptions(
            SystemTextJsonSerializerOptions? baseOptions = null,
            bool isReadOnly = false)
        {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            baseOptions ??= SystemTextJsonSerializerOptions.Default;
            baseOptions = baseOptions.AddDefaultJsonTypeInfoResolver(isReadOnly);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            if (!isReadOnly && baseOptions.IsReadOnly)
            {
                baseOptions = new(baseOptions);
            }

            // https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/configure-options?pivots=dotnet-8-0#web-defaults-for-jsonserializeroptions
            baseOptions.PropertyNameCaseInsensitive = true;
            baseOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            baseOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;

            baseOptions.Converters.Add(new CookieConverter());
            baseOptions.Converters.Add(new CookieCollectionConverter());
            baseOptions.Converters.Add(new CookieContainerConverter());
            baseOptions.Converters.Add(new IntPtrConverter());
            baseOptions.Converters.Add(new UIntPtrConverter());
            baseOptions.Converters.Add(new ValueTupleConverter());

            return baseOptions;
        }
#endif

        /// <summary>
        /// 用于组合多个源生成器
        /// <para>https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/source-generation#combine-source-generators</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CombineTypeInfoResolvers(this SystemTextJsonSerializerOptions left, SystemTextJsonSerializerOptions right)
            => left.CombineTypeInfoResolvers(right.TypeInfoResolverChain);

        /// <summary>
        /// 用于组合多个源生成器
        /// <para>https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/source-generation#combine-source-generators</para>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CombineTypeInfoResolvers(this SystemTextJsonSerializerOptions options, IEnumerable<IJsonTypeInfoResolver> resolvers)
        {
            foreach (var resolver in resolvers)
            {
                if (resolver is DefaultJsonTypeInfoResolver)
                    continue;

                if (!options.TypeInfoResolverChain.Contains(resolver))
                {
                    options.TypeInfoResolverChain.Insert(0, resolver);
                }
            }
        }

        /// <summary>
        /// 用于组合多个源生成器
        /// <para>https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/source-generation#combine-source-generators</para>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CombineTypeInfoResolvers(this SystemTextJsonSerializerOptions options, params IJsonTypeInfoResolver[] resolvers)
            => options.CombineTypeInfoResolvers(resolvers.AsEnumerable());
#endif

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

            Xml,
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
#if !NO_SYSTEM_TEXT_JSON
            return JsonImplType.SystemTextJson;
#else
#if !NO_NEWTONSOFT_JSON
            if (Environment.Version.Major <= 5)
            {
                return JsonImplType.NewtonsoftJson;
            }
#endif
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
        public static T? Clone<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(T? obj) where T : notnull
        {
            if (EqualityComparer<T?>.Default.Equals(obj, default))
                return default;

#if !NO_MEMORYPACK && (!NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER))
            try
            {
                var bytes = MemoryPackSerializer.Serialize(obj);
                return MemoryPackSerializer.Deserialize<T>(bytes);
            }
            catch
            {
            }
#endif

#if !NO_MESSAGEPACK
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
        public static T Clone<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(this T obj) where T : ICloneableSerializable => Serializable.Clone(obj);

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