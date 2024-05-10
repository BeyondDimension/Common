#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)

namespace System.Text.Json;

public static partial class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// https://github.com/dotnet/runtime/issues/89113
    /// </summary>
    /// <param name="thiz"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "RootDefaultInstance")]
    static extern DefaultJsonTypeInfoResolver GetDefaultJsonTypeInfoResolver(DefaultJsonTypeInfoResolver? @thiz = null);

    /// <summary>
    /// 添加反射实现的类型解析器 <see cref="DefaultJsonTypeInfoResolver"/>
    /// </summary>
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static SystemTextJsonSerializerOptions AddDefaultJsonTypeInfoResolver(
        this SystemTextJsonSerializerOptions jsonSerializerOptions,
        bool isReadOnly = false,
        bool isReflectionEnabled = true)
    {
        if (!isReflectionEnabled)
        {
            return jsonSerializerOptions;
        }
        if (!isReadOnly && jsonSerializerOptions.IsReadOnly)
        {
            jsonSerializerOptions = new(jsonSerializerOptions);
        }

        if (SystemTextJsonSerializer.IsReflectionEnabledByDefault)
        {
            if (!jsonSerializerOptions.TypeInfoResolverChain.OfType<DefaultJsonTypeInfoResolver>().Any())
            {
                jsonSerializerOptions.TypeInfoResolverChain.Add(GetDefaultJsonTypeInfoResolver());
            }
        }

        return jsonSerializerOptions;
    }

    /// <summary>
    /// 将当前的 <see cref="SystemTextJsonSerializerOptions.TypeInfoResolverChain"/> 复制到 dest 上
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    public static void CopyTypeInfoResolverChainTo(this SystemTextJsonSerializerOptions source, SystemTextJsonSerializerOptions dest)
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

#endif