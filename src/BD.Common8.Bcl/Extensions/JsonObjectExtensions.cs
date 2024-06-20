#if !NO_SYSTEM_TEXT_JSON
namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="SystemTextJsonObject"/> 类型的扩展函数
/// </summary>
public static partial class JsonObjectExtensions
{
    /// <summary>
    /// Gets or sets the element with the specified property name.
    /// If the property is not found, <see langword="null"/> is returned.
    /// </summary>
    /// <param name="jsonNode"></param>
    /// <param name="propertyName">The name of the property to return.</param>
    /// <returns></returns>
    public static JsonNode? GetItem(this JsonNode? jsonNode, string propertyName)
    {
        if (jsonNode == null)
            return null;
        try
        {
            var jsonObj = jsonNode.AsObject();
            try
            {
                return jsonObj[propertyName];
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
                InternalReflection.InitializeDictionary(jsonObj);
                return jsonObj[propertyName];
            }
        }
        catch (InvalidOperationException)
        {
            // The current JsonNode is not a JsonObject.
            return null;
        }
    }

    static partial class InternalReflection
    {
        static readonly Lazy<MethodInfo> _GetUnderlyingRepresentation = new([DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SystemTextJsonObject))] () =>
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

        const string typeName_JsonPropertyDictionary1 = "System.Text.Json.JsonPropertyDictionary`1";
        const string assemblyName_SystemTextJson = "System.Text.Json";

        static readonly Lazy<Type> _CreateJsonPropertyDictionaryJsonNode = new([DynamicDependency(DynamicallyAccessedMemberTypes.All, typeName_JsonPropertyDictionary1, assemblyName_SystemTextJson)] () =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var typeOfJsonPropertyDictionary = typeOfJObject.Assembly.GetType(typeName_JsonPropertyDictionary1, true);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            var typeOfJsonPropertyDictionaryMakeGeneric = typeOfJsonPropertyDictionary.ThrowIsNull().MakeGenericType(typeof(JsonNode));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return typeOfJsonPropertyDictionaryMakeGeneric.ThrowIsNull();
        });

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeName_JsonPropertyDictionary1, assemblyName_SystemTextJson)]
#pragma warning disable IL2072 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
        static object? CreateJsonPropertyDictionaryJsonNode(bool caseInsensitive) => Activator.CreateInstance(_CreateJsonPropertyDictionaryJsonNode.Value, [caseInsensitive]);
#pragma warning restore IL2072 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.

        /// <summary>
        /// https://github.com/dotnet/runtime/blob/v8.0.0-rc.2.23479.6/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Node/JsonNodeConverter.cs#L59
        /// </summary>
        static readonly Lazy<MethodInfo> _JsonNodeConverterCreate = new([DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SystemTextJsonObject))] () =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var typeOfJsonNodeConverter = typeOfJObject.Assembly.GetType("System.Text.Json.Serialization.Converters.JsonNodeConverter", true);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var methodCreateJsonNode = typeOfJsonNodeConverter.ThrowIsNull().GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
            return methodCreateJsonNode.ThrowIsNull();
        });

        static JsonNode? JsonNodeConverterCreate(JsonElement element, JsonNodeOptions? options) => (JsonNode?)_JsonNodeConverterCreate.Value.Invoke(null, [element, options]);

        static readonly Lazy<PropertyInfo> _SetParent = new([DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonNode))] () =>
        {
            var propertyParent = typeof(JsonNode).GetProperty(nameof(JsonNode.Parent));
            return propertyParent.ThrowIsNull();
        });

        static void SetPropertyParent(JsonNode jsonNode, JsonNode parent) => _SetParent.Value.SetValue(jsonNode, parent);

        static readonly Lazy<FieldInfo> _SetFieldDictionary = new([DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SystemTextJsonObject))] () =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var fieldDictionary = typeOfJObject.GetField("_dictionary", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldDictionary.ThrowIsNull();
        });

        static void SetFieldDictionary(SystemTextJsonObject obj, object? value) => _SetFieldDictionary.Value.SetValue(obj, value);

        static readonly Lazy<FieldInfo> _SetFieldJsonElement = new([DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SystemTextJsonObject))] () =>
        {
            var typeOfJObject = typeof(SystemTextJsonObject);
            var fieldDictionary = typeOfJObject.GetField("_jsonElement", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldDictionary.ThrowIsNull();
        });

        static void SetFieldJsonElement(SystemTextJsonObject obj, object? value) => _SetFieldJsonElement.Value.SetValue(obj, value);

        static readonly Lazy<MethodInfo> _JsonPropertyDictionaryJsonNodeTryAddValue = new(() =>
        {
            var methodTryAddValue = _CreateJsonPropertyDictionaryJsonNode.Value.GetMethod("TryAddValue", BindingFlags.Instance | BindingFlags.NonPublic);
            return methodTryAddValue.ThrowIsNull();
        });

        static void JsonPropertyDictionaryJsonNodeTryAddValue(object? dictionary, string? propertyName, JsonNode? value) => _JsonPropertyDictionaryJsonNodeTryAddValue.Value.Invoke(dictionary, [propertyName, value]);

        /// <summary>
        /// fix https://github.com/dotnet/runtime/issues/71784
        /// </summary>
        /// <param name="obj"></param>
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SystemTextJsonObject))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonNode))]
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