using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace BD.Common8.Settings5.Services.Implementation;

partial class SettingsLoadServiceImpl // Json 序列化
{
    static void AddConverters(JsonSerializerOptions options, params IEnumerable<JsonConverter>? converters)
    {
        if (converters != null)
        {
            foreach (var converter in converters)
            {
                options.Converters.Add(converter);
            }
        }
    }

    /// <inheritdoc/>
    public void AddConverters(params IEnumerable<JsonConverter> converters)
    {
        if (options.IsReadOnly)
        {
            var options2 = new JsonSerializerOptions(options);
            AddConverters(options2, converters);
            Interlocked.CompareExchange(ref options, options2, null);
        }
        else
        {
            AddConverters(options, converters);
        }
    }

    static void AddResolvers(JsonSerializerOptions options, params IEnumerable<IJsonTypeInfoResolver>? resolvers)
    {
        if (resolvers != null)
        {
            var reflectionResolvers = options.TypeInfoResolverChain.OfType<DefaultJsonTypeInfoResolver>().ToArray();
            var hasReflectionResolver = reflectionResolvers.Length != 0;
            if (hasReflectionResolver)
            {
                // 存在反射解析器时，先将其移除
                foreach (var reflectionResolver in reflectionResolvers)
                {
                    options.TypeInfoResolverChain.Remove(reflectionResolver);
                }
            }
            foreach (var resolver in resolvers)
            {
                // 添加传入的解析器
                options.TypeInfoResolverChain.Add(resolver);
            }
            if (hasReflectionResolver)
            {
                // 在 List 末尾再添加反射解析器，保持反射解析器顺序在最后
                options.TypeInfoResolverChain.Add(reflectionResolvers.First());
            }
        }
    }

    /// <inheritdoc/>
    public void AddResolvers(params IEnumerable<IJsonTypeInfoResolver> resolvers)
    {
        if (options.IsReadOnly)
        {
            var options2 = new JsonSerializerOptions(options);
            AddResolvers(options2, resolvers);
            Interlocked.CompareExchange(ref options, options2, null);
        }
        else
        {
            AddResolvers(options, resolvers);
        }
    }

    /// <summary>
    /// 创建 Json 序列化选项，当使用不使用 AOT 时，converters 与 resolvers 可传入 <see langword="null"/>，否则应传入模型类中包含枚举的 <see cref="JsonStringEnumConverter{TEnum}"/> 与源生成模型类的 <see cref="JsonSerializerContext"/>
    /// </summary>
    /// <param name="converters"></param>
    /// <param name="resolvers"></param>
    /// <returns></returns>
    static JsonSerializerOptions GetOptions(
        IEnumerable<JsonConverter>? converters = null,
        IEnumerable<IJsonTypeInfoResolver>? resolvers = null)
    {
        // 将 converters 传入为泛型的 JsonStringEnumConverter<TEnum> 实现枚举序列化为字符串的与 AOT 或裁剪兼容
        // 将 resolvers 传入 JsonSerializerContext 对模型类的源生成解析器
        // 在初始化时传入，随后使用 AddConverters 与 AddResolvers 动态追加

        JsonSerializerOptions o = new();

        if (converters != null)
        {
            foreach (var converter in converters)
            {
                o.Converters.Add(converter);
            }
        }

        if (JsonSerializer.IsReflectionEnabledByDefault)
        {
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            // 当使用 AOT 时，应 converters 传入或调用 AddConverters 函数添加 JsonStringEnumConverter<TEnum>
            o.Converters.Add(new JsonStringEnumConverter());
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        }

        if (resolvers != null)
        {
            foreach (var resolver in resolvers)
            {
                o.TypeInfoResolverChain.Add(resolver);
            }
        }

        const bool camelCase = false; // 与旧版本保持一致

        // Serializable.CreateOptions 有判断 JsonSerializer.IsReflectionEnabledByDefault 为 false 时不使用反射实现的 DefaultJsonTypeInfoResolver
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        o = Serializable.CreateOptions(o, camelCase: camelCase);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        // 默认用于设置项的 Json 序列化选项
        o.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        o.IgnoreReadOnlyFields = false;
        o.IgnoreReadOnlyProperties = false;
        o.IncludeFields = false;
        o.WriteIndented = true;
        o.AllowTrailingCommas = true;
        o.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Serialize(Stream utf8Json, object? value, Type inputType)
    {
        // 由 GetOptions 配置的 JsonSerializerOptions 允许使用源生成的 IJsonTypeInfoResolver 以兼容 AOT 与 裁剪
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        JsonSerializer.Serialize(utf8Json, value, inputType, options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TValue? Deserialize<TValue>(Stream utf8Json)
    {
        // 由 GetOptions 配置的 JsonSerializerOptions 允许使用源生成的 IJsonTypeInfoResolver 以兼容 AOT 与 裁剪
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var result = JsonSerializer.Deserialize<TValue>(utf8Json, options);
        return result;
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TValue? Deserialize<TValue>(JsonNode? node)
    {
        // 由 GetOptions 配置的 JsonSerializerOptions 允许使用源生成的 IJsonTypeInfoResolver 以兼容 AOT 与 裁剪
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var result = node.Deserialize<TValue>(options);
        return result;
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Type GetIOptionsMonitorTSettingsModelType(Type settingsModelType)
    {
        // IOptionsMonitor<TSettingsModel> 为已知类型，在 SettingsProperty 上有声明，为编译时类型，不需要运行时动态创建类型
#pragma warning disable IL2071 // Generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The parameter of method does not have matching annotations.
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var typeOptionsMonitor = typeof(IOptionsMonitor<>).MakeGenericType(settingsModelType);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2071 // Generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The parameter of method does not have matching annotations.
        return typeOptionsMonitor;
    }
}
