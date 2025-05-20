#if !NO_SYSTEM_TEXT_JSON
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json;

public static partial class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// 添加反射实现的类型解析器 <see cref="DefaultJsonTypeInfoResolver"/>
    /// </summary>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
#endif
    public static JsonSerializerOptions AddDefaultJsonTypeInfoResolver(
        this JsonSerializerOptions jsonSerializerOptions,
        bool isReadOnly = false,
        bool isReflectionEnabled = true)
    {
        if (!isReflectionEnabled)
        {
            return jsonSerializerOptions;
        }

        if (JsonSerializer.IsReflectionEnabledByDefault)
        {
            if (!jsonSerializerOptions.TypeInfoResolverChain.OfType<DefaultJsonTypeInfoResolver>().Any())
            {
                if (!isReadOnly && jsonSerializerOptions.IsReadOnly)
                {
                    jsonSerializerOptions = new(jsonSerializerOptions);
                }

                jsonSerializerOptions.TypeInfoResolverChain.Add(GetDefaultJsonTypeInfoResolver());
            }
        }

        return jsonSerializerOptions;
    }

    /// <summary>
    /// 将当前的 <see cref="JsonSerializerOptions.TypeInfoResolverChain"/> 复制到 dest 上
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    public static void CopyTypeInfoResolverChainTo(this JsonSerializerOptions source, JsonSerializerOptions dest)
    {
        dest.TypeInfoResolverChain.Clear();
        dest.TypeInfoResolver = source.TypeInfoResolver;

        foreach (var typeInfoResolver in source.TypeInfoResolverChain)
        {
            if (typeInfoResolver != dest.TypeInfoResolver)
            {
                dest.TypeInfoResolverChain.Add(typeInfoResolver);
            }
        }
    }
}

static partial class JsonSerializerOptionsExtensions // https://github.com/dotnet/runtime/issues/89113
{
    static DefaultJsonTypeInfoResolver GetDefaultJsonTypeInfoResolver()
    {
        try
        {
            return GetDefaultJsonTypeInfoResolver_V9();
        }
        catch (MissingMethodException)
        {
        }
        return GetDefaultJsonTypeInfoResolver_V8();
    }

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v8.0.11/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Metadata/DefaultJsonTypeInfoResolver.cs#L135
    /// </summary>
    /// <param name="thiz"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "RootDefaultInstance")]
    static extern DefaultJsonTypeInfoResolver GetDefaultJsonTypeInfoResolver_V8(DefaultJsonTypeInfoResolver? @thiz = null);

    /// <summary>
    /// https://github.com/dotnet/runtime/blob/v9.0.0/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Metadata/DefaultJsonTypeInfoResolver.cs#L127
    /// </summary>
    /// <param name="thiz"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "get_DefaultInstance")]
    static extern DefaultJsonTypeInfoResolver GetDefaultJsonTypeInfoResolver_V9(DefaultJsonTypeInfoResolver? @thiz = null);
}
#endif