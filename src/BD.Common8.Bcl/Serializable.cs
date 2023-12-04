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

        /// <summary>
        /// 使用序列化的服务
        /// </summary>
        public interface IService : Log.I
        {
            /// <summary>
            /// 使用的默认文本编码
            /// </summary>
            Encoding DefaultEncoding => Encoding.UTF8;

            /// <inheritdoc cref="Newtonsoft.Json.JsonSerializer"/>
            NewtonsoftJsonSerializer NewtonsoftJsonSerializer => null!;

            /// <summary>
            /// 序列化是否必须使用 <see cref="JsonTypeInfo"/>，即源生成的类型信息数据，避免运行时反射
            /// </summary>
            bool RequiredJsonTypeInfo => true;

            /// <summary>
            /// 用于序列化的类型信息，由 Json 源生成
            /// </summary>
            JsonTypeInfo? JsonTypeInfo => null;

            /// <summary>
            /// 当序列化出现错误时
            /// </summary>
            /// <param name="ex"></param>
            /// <param name="isSerializeOrDeserialize">是序列化还是反序列化</param>
            /// <param name="modelType">模型类类型</param>
            void OnSerializerError(Exception ex,
                bool isSerializeOrDeserialize,
                Type modelType)
                => DefaultOnSerializerError(Logger, ex, isSerializeOrDeserialize, modelType);

            /// <summary>
            /// <see cref="OnSerializerError(Exception, bool, Type)"/> 的默认实现
            /// </summary>
            /// <param name="logger"></param>
            /// <param name="ex"></param>
            /// <param name="isSerializeOrDeserialize"></param>
            /// <param name="modelType"></param>
            static void DefaultOnSerializerError(
                ILogger logger,
                Exception ex,
                bool isSerializeOrDeserialize,
                Type modelType)
            {
                // 记录错误时，不需要带上 requestUrl 等敏感信息
                if (isSerializeOrDeserialize)
                {
                    logger.LogError(ex,
                        "Error serializing request model class. (Parameter '{type}')",
                        modelType);
                }
                else
                {
                    logger.LogError(ex,
                        "Error reading and deserializing the response content into an instance. (Parameter '{type}')",
                        modelType);
                }
            }
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

#pragma warning disable SA1600 // Elements should be documented
        public const string Obsolete_GetNJsonContent = "无特殊情况下应使用 GetSJsonContent，即 System.Text.Json";
        public const string Obsolete_ReadFromNJsonAsync = "无特殊情况下应使用 ReadFromSJsonAsync，即 System.Text.Json";
        public const string Obsolete_UseAsync = "无特殊情况下应使用 Async 异步的函数版本";
#pragma warning restore SA1600 // Elements should be documented

        #region MemoryPack

        /// <summary>
        /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="inputValue"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static HttpContent? GetMemoryPackContent<T>(
            this IService s,
            T inputValue,
            MediaTypeHeaderValue? mediaType = null)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: true, typeof(T));
                return default;
            }
        }

        /// <summary>
        /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 MemoryPack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task<T?> ReadFromMemoryPackAsync<T>(
            this IService s,
            HttpContent content,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.CompletedTask;
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
                return default;
            }
        }

        #endregion

        #region Newtonsoft.Json

        /// <summary>
        /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="inputValue"></param>
        /// <param name="encoding"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        [Obsolete(Obsolete_GetNJsonContent)]
        public static HttpContent? GetNJsonContent<T>(
            this IService s,
            T inputValue,
            Encoding? encoding = null,
            MediaTypeHeaderValue? mediaType = null)
        {
            if (inputValue == null)
                return null;
            try
            {
                encoding ??= s.DefaultEncoding;
                var stream = new MemoryStream(); // 使用内存流，避免 byte[] 块，与字符串 utf16 开销
                using var streamWriter = new StreamWriter(stream, encoding, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(streamWriter);
                s.NewtonsoftJsonSerializer.Serialize(jsonWriter, inputValue, typeof(T));
                var content = new StreamContent(stream);
                content.Headers.ContentType = mediaType ?? new MediaTypeHeaderValue(MediaTypeNames.JSON, encoding.WebName);
                return content;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: true, typeof(T));
                return default;
            }
        }

        /// <summary>
        /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
        /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="NewtonsoftJsonObject"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Obsolete(Obsolete_ReadFromNJsonAsync)]
        public static async Task<T?> ReadFromNJsonAsync<T>(
            this IService s,
            HttpContent content,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                encoding ??= s.DefaultEncoding;
                using var contentStream = await content.ReadAsStreamAsync(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                using var contentStreamReader = new StreamReader(contentStream, encoding);
                using var contentJsonTextReader = new JsonTextReader(contentStreamReader);
                var result = s.NewtonsoftJsonSerializer.Deserialize<T>(contentJsonTextReader);
                return result;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
                return default;
            }
        }

        /// <summary>
        /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 <see cref="Newtonsoft.Json"/>，需要 <see cref="newtonsoftJsonSerializer"/>
        /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="NewtonsoftJsonObject"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Obsolete(Obsolete_UseAsync)]
        public static T? ReadFromNJson<T>(
            this IService s,
            HttpContent content,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                encoding ??= s.DefaultEncoding;
                using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                using var contentStreamReader = new StreamReader(contentStream, encoding);
                using var contentJsonTextReader = new JsonTextReader(contentStreamReader);
                var result = s.NewtonsoftJsonSerializer.Deserialize<T>(contentJsonTextReader);
                return result;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
                return default;
            }
        }

        #endregion

        #region System.Text.Json

        /// <summary>
        /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="inputValue"></param>
        /// <param name="jsonTypeInfo"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static HttpContent? GetSJsonContent<T>(
            this IService s,
            T inputValue,
            JsonTypeInfo<T>? jsonTypeInfo,
            MediaTypeHeaderValue? mediaType = null)
        {
            if (inputValue == null)
                return null;
            try
            {
                JsonContent content;
                jsonTypeInfo ??= s.JsonTypeInfo is JsonTypeInfo<T> jsonTypeInfo_ ? jsonTypeInfo_ : null;
                if (jsonTypeInfo == null)
                {
                    if (s.RequiredJsonTypeInfo) throw new ArgumentNullException(nameof(jsonTypeInfo));
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                    content = JsonContent.Create(inputValue, mediaType);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
                }
                else
                {
                    content = JsonContent.Create(inputValue, jsonTypeInfo, mediaType);
                }
                return content;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: true, typeof(T));
                return default;
            }
        }

        /// <summary>
        /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
        /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="content"></param>
        /// <param name="jsonTypeInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T?> ReadFromSJsonAsync<T>(
            this IService s,
            HttpContent content,
            JsonTypeInfo<T>? jsonTypeInfo,
            CancellationToken cancellationToken = default)
        {
            try
            {
                T? result;
                jsonTypeInfo ??= s.JsonTypeInfo is JsonTypeInfo<T> jsonTypeInfo_ ? jsonTypeInfo_ : null;
                if (jsonTypeInfo == null)
                {
                    if (s.RequiredJsonTypeInfo)
                    {
                        var modelType = typeof(T);
                        if (modelType != typeof(SystemTextJsonObject))
                        {
                            throw new ArgumentNullException(nameof(jsonTypeInfo));
                        }
                    }
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                    result = await content.ReadFromJsonAsync<T>(cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
                }
                else
                {
                    result = await content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken);
                }
                return result;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
                return default;
            }
        }

        /// <summary>
        /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
        /// <para>如果需要使用 Linq to Json 操作，则将泛型定义为 <see cref="SystemTextJsonObject"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="content"></param>
        /// <param name="jsonTypeInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete(Obsolete_UseAsync)]
        public static T? ReadFromSJson<T>(
            this IService s,
            HttpContent content,
            JsonTypeInfo<T>? jsonTypeInfo,
            CancellationToken cancellationToken = default)
        {
            try
            {
                T? result;
                jsonTypeInfo ??= s.JsonTypeInfo is JsonTypeInfo<T> jsonTypeInfo_ ? jsonTypeInfo_ : null;
                if (jsonTypeInfo == null)
                {
                    if (s.RequiredJsonTypeInfo)
                    {
                        var modelType = typeof(T);
                        if (modelType != typeof(SystemTextJsonObject))
                        {
                            throw new ArgumentNullException(nameof(jsonTypeInfo));
                        }
                    }
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                    using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                    result = SystemTextJsonSerializer.Deserialize<T>(contentStream);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
                }
                else
                {
                    using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                    result = SystemTextJsonSerializer.Deserialize(contentStream, jsonTypeInfo);
                }
                return result;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
                return default;
            }
        }

        #endregion

        #region Xml

        /// <summary>
        /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），使用 XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="inputValue"></param>
        /// <param name="encoding"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        public static HttpContent? GetXmlContent<T>(
            this IService s,
            T inputValue,
            Encoding? encoding = null,
            MediaTypeHeaderValue? mediaType = null)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: true, typeof(T));
                return default;
            }
        }

        /// <summary>
        /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        public static async Task<T?> ReadFromXmlAsync<T>(
            this IService s,
            HttpContent content,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                encoding ??= s.DefaultEncoding;
                using var contentStream = await content.ReadAsStreamAsync(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                using var contentStreamReader = new StreamReader(contentStream, encoding);
                var result = DXml<T>(contentStreamReader);
                return result;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
                return default;
            }
        }

        /// <summary>
        /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），使用 XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        [Obsolete(Obsolete_UseAsync)]
        public static T? ReadFromXml<T>(
            this IService s,
            HttpContent content,
            Encoding? encoding = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                encoding ??= s.DefaultEncoding;
                using var contentStream = content.ReadAsStream(cancellationToken); // 使用流，避免 byte[] 块，与字符串 utf16 开销
                using var contentStreamReader = new StreamReader(contentStream, encoding);
                var result = DXml<T>(contentStreamReader);
                return result;
            }
            catch (Exception ex)
            {
                s.OnSerializerError(ex, isSerializeOrDeserialize: false, typeof(T));
                return default;
            }
        }

        #endregion
    }
}