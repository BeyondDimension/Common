#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="SystemTextJsonObject"/> 类型的扩展函数
/// </summary>
public static partial class JsonObjectExtensions
{
    /// <summary>
    /// 通过自定义委托从 <see cref="SystemTextJsonObject"/> 中读取值，使用此函数允许读取带有重复键的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="func"></param>
    /// <param name="checknull"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(Serializable.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(Serializable.SerializationRequiresDynamicCodeMessage)]
    public static T? GetValue<T>(this SystemTextJsonObject? obj, Func<T?> func, bool checknull = true) where T : notnull
    {
        if (checknull && obj == null)
            return default;
        try
        {
            return func();
        }
        catch (ArgumentException)
        {
            /* 消息:
        System.ArgumentException : An item with the same key has already been added. Key: test1 (Parameter 'propertyName')

      堆栈跟踪:
        ThrowHelper.ThrowArgumentException_DuplicateKey(String paramName, String propertyName)
        JsonObject.InitializeIfRequired()
        JsonNode.get_Item(String propertyName)
             */
            InternalReflection.InitializeDictionary(obj);
            return func();
        }
    }

    [RequiresUnreferencedCode(Serializable.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(Serializable.SerializationRequiresDynamicCodeMessage)]
    static partial class InternalReflection
    {
        static readonly Lazy<MethodInfo> _GetUnderlyingRepresentation = new(() =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var methodGetUnderlyingRepresentation = typeOfJObject.GetMethod("GetUnderlyingRepresentation", BindingFlags.Instance | BindingFlags.NonPublic);
            object?[] parameters = [null, null];
            return methodGetUnderlyingRepresentation.ThrowIsNull();
        });

        static void GetUnderlyingRepresentation(SystemTextJsonObject? obj, out object? dictionary, out JsonElement? jsonElement)
        {
            object?[] parameters = [null, null];
            _GetUnderlyingRepresentation.Value.Invoke(obj, parameters);
            dictionary = parameters[0];
            jsonElement = (JsonElement?)parameters[1];
        }

        static readonly Lazy<Type> _CreateJsonPropertyDictionaryJsonNode = new(() =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var typeOfJsonPropertyDictionary = typeOfJObject.Assembly.GetType("System.Text.Json.JsonPropertyDictionary`1", true);
            var typeOfJsonPropertyDictionaryMakeGeneric = typeOfJsonPropertyDictionary.ThrowIsNull().MakeGenericType(typeof(JsonNode));
            return typeOfJsonPropertyDictionaryMakeGeneric.ThrowIsNull();
        });

        static object? CreateJsonPropertyDictionaryJsonNode(bool caseInsensitive) => Activator.CreateInstance(_CreateJsonPropertyDictionaryJsonNode.Value, [caseInsensitive]);

        /// <summary>
        /// https://github.com/dotnet/runtime/blob/v8.0.0-rc.2.23479.6/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Node/JsonNodeConverter.cs#L59
        /// </summary>
        static readonly Lazy<MethodInfo> _JsonNodeConverterCreate = new(() =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var typeOfJsonNodeConverter = typeOfJObject.Assembly.GetType("System.Text.Json.Serialization.Converters.JsonNodeConverter", true);
            var methodCreateJsonNode = typeOfJsonNodeConverter.ThrowIsNull().GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
            return methodCreateJsonNode.ThrowIsNull();
        });

        static JsonNode? JsonNodeConverterCreate(JsonElement element, JsonNodeOptions? options) => (JsonNode?)_JsonNodeConverterCreate.Value.Invoke(null, [element, options]);

        static readonly Lazy<PropertyInfo> _SetParent = new(() =>
        {
            var propertyParent = typeof(JsonNode).GetProperty(nameof(JsonNode.Parent));
            return propertyParent.ThrowIsNull();
        });

        static void SetPropertyParent(JsonNode jsonNode, JsonNode parent) => _SetParent.Value.SetValue(jsonNode, parent);

        static readonly Lazy<FieldInfo> _SetFieldDictionary = new(() =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var fieldDictionary = typeOfJObject.GetField("_dictionary", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldDictionary.ThrowIsNull();
        });

        static void SetFieldDictionary(SystemTextJsonObject obj, object? value) => _SetFieldDictionary.Value.SetValue(obj, value);

        static readonly Lazy<FieldInfo> _SetFieldJsonElement = new(() =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var fieldDictionary = typeOfJObject.GetField("_jsonElement", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldDictionary.ThrowIsNull();
        });

        static void SetFieldJsonElement(SystemTextJsonObject obj, object? value) => _SetFieldJsonElement.Value.SetValue(obj, value);

        static readonly Lazy<MethodInfo> _JsonPropertyDictionaryJsonNodeTryAddValue = new(() =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var typeOfJsonPropertyDictionary = typeOfJObject.Assembly.GetType("System.Text.Json.JsonPropertyDictionary`1", true);
            var typeOfJsonPropertyDictionaryMakeGeneric = typeOfJsonPropertyDictionary.ThrowIsNull().MakeGenericType(typeof(JsonNode));
            var methodTryAddValue = typeOfJsonPropertyDictionaryMakeGeneric.GetMethod("TryAddValue", BindingFlags.Instance | BindingFlags.NonPublic);
            return methodTryAddValue.ThrowIsNull();
        });

        static void JsonPropertyDictionaryJsonNodeTryAddValue(object? dictionary, string? propertyName, JsonNode? value) => _JsonPropertyDictionaryJsonNodeTryAddValue.Value.Invoke(dictionary, [propertyName, value]);

        /// <summary>
        /// fix https://github.com/dotnet/runtime/issues/71784
        /// </summary>
        /// <param name="obj"></param>
        public static void InitializeDictionary(SystemTextJsonObject? obj)
        {
            if (obj == null) return;
            // https://github.com/dotnet/runtime/blob/v8.0.0-rc.2.23479.6/src/libraries/System.Text.Json/src/System/Text/Json/Nodes/JsonObject.IDictionary.cs#L196-L225
            GetUnderlyingRepresentation(obj, out var dictionary, out var jsonElement);
            if (dictionary == null)
            {
                bool caseInsensitive = obj.Options.HasValue && obj.Options.Value.PropertyNameCaseInsensitive;
                dictionary = CreateJsonPropertyDictionaryJsonNode(caseInsensitive);
                if (jsonElement.HasValue)
                {
                    IEnumerable<SystemTextJsonPropertyStruct> items = jsonElement.Value.EnumerateObject();
                    foreach (var item in items.Reverse()) // 倒序循环以重复时取最后一个的值
                    {
                        var jsonNode = JsonNodeConverterCreate(item.Value, obj.Options);
                        if (jsonNode != null)
                        {
                            SetPropertyParent(jsonNode, obj);
                        }
                        JsonPropertyDictionaryJsonNodeTryAddValue(dictionary, item.Name, jsonNode);
                    }
                }
                SetFieldDictionary(obj, dictionary);
                Interlocked.MemoryBarrier();
                SetFieldJsonElement(obj, null);
            }
        }
    }
}
#endif