using DotNext.Reflection;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace BD.Common8.UnitTest;

/// <summary>
/// 模型类的序列化测试助手类，用于 JSON 源生成测试与 AOT 兼容
/// </summary>
static class SerializationTestHelper
{
    internal const string jsonObjectString = "{}";

    /// <summary>
    /// 方法返回空的 C# 关键字 <see langword="void"/> 实际上为 BCL 中的值类型 System.Void，此类型在 C# 中不可见
    /// </summary>
    internal const string voidTypeString = "System.Void";

    internal static Func<Type, JsonSerializerOptions, object?>? CreateInstanceDelegate { get; set; }

    /// <summary>
    /// 根据模型类型创建实例
    /// </summary>
    internal static object CreateInstance(Type t, JsonSerializerOptions opt)
    {
        if (t.FullName == voidTypeString)
        {
            return null!;
        }

        {
            try
            {
                var result = CreateInstanceDelegate?.Invoke(t, opt);
                if (result != null)
                {
                    return result;
                }
            }
            catch
            {
            }
        }

        try
        {
            if (t.IsInterface)
            {
                if (t.IsGenericType)
                {
                    var gTypeDef = t.GetGenericTypeDefinition();
                    if (gTypeDef == typeof(IEnumerable<>) || gTypeDef == typeof(IList<>) || gTypeDef == typeof(ICollection<>) || gTypeDef == typeof(IReadOnlyCollection<>) || gTypeDef == typeof(IReadOnlyList<>))
                    {
                        return CreateInstance(typeof(List<>).MakeGenericType(t.GenericTypeArguments[0]), opt);
                    }
                    else if (gTypeDef == typeof(IDictionary<,>) || gTypeDef == typeof(IReadOnlyDictionary<,>))
                    {
                        return CreateInstance(typeof(Dictionary<,>).MakeGenericType(t.GenericTypeArguments[0], t.GenericTypeArguments[1]), opt);
                    }
                }
            }

            if (t.IsArray)
            {
                var ta = t.GetTypeInfo().ImplementedInterfaces.Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))!.GetGenericArguments()[0];
                var array = typeof(Array).GetMethod(nameof(Array.Empty))!.MakeGenericMethod(ta).Invoke(null, null);
                return array!;
            }

            var obj = Activator.CreateInstance(t);
            return obj.ThrowIsNull();
        }
        catch (Exception ex)
        {
            try
            {
                // 先尝试使用源生成执行
                var obj = JsonSerializer.Deserialize(jsonObjectString, t, opt);
                return obj.ThrowIsNull();
            }
            catch
            {
                try
                {
                    var obj = JsonSerializer.Deserialize(jsonObjectString, t);
                    return obj.ThrowIsNull();
                }
                catch
                {
                    // 忽略反序列化的异常，抛出调用构造函数的异常
                    throw ex;
                }
            }
        }
    }

    internal static bool IsSimpleTypes(Type t)
    {
        var typeCode = Type.GetTypeCode(t);
        switch (typeCode)
        {
            case TypeCode.Empty:
                break;
            case TypeCode.DBNull:
                break;
            case TypeCode.Boolean:
                break;
            case TypeCode.Char:
                break;
            case TypeCode.SByte:
                break;
            case TypeCode.Byte:
                break;
            case TypeCode.Int16:
                break;
            case TypeCode.UInt16:
                break;
            case TypeCode.Int32:
                break;
            case TypeCode.UInt32:
                break;
            case TypeCode.Int64:
                break;
            case TypeCode.UInt64:
                break;
            case TypeCode.Single:
                break;
            case TypeCode.Double:
                break;
            case TypeCode.Decimal:
                break;
            case TypeCode.DateTime:
                break;
            case TypeCode.String:
                break;
            default:
                return false;
        }
        return true;
    }

    internal static bool IsNullableSimpleTypes(Type t)
    {
        if (t.IsGenericType)
        {
            var gTypeDef = t.GetGenericTypeDefinition();
            if (gTypeDef == typeof(Nullable<>))
            {
                return IsSimpleTypes(t.GenericTypeArguments[0]);
            }
        }
        return false;
    }

    internal static bool IsArraySimpleTypes(Type t)
    {
        if (t.IsArray)
        {
            var ta = t.GetTypeInfo().ImplementedInterfaces.Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))!.GetGenericArguments()[0];
            return IsSimpleTypes(ta);
        }
        return false;
    }

    /// <summary>
    /// 传入多个模型程序集，返回类型为模型类的类型
    /// </summary>
    internal static IEnumerable<Type> GetModelTypesByModelAssemblies(string? namespaceStartsWith = null, Func<Type, bool>? predicate = null, params IEnumerable<Assembly> assemblies)
    {
        var tJsonSerializerContext = typeof(JsonSerializerContext);
        var tJsonConverter = typeof(JsonConverter);
        var q = from t in assemblies.Select(static x => x.GetTypes()).SelectMany(static x => x)
                where t.Namespace != null && (namespaceStartsWith == null || t.Namespace.StartsWith(namespaceStartsWith)) // 命名空间过滤
                    && t.FullName != null && t.FullName.Contains("+<") == false // 排除源生成的类型
                    && t.IsStatic() == false && t.IsAbstract == false && t.IsClass && !t.IsInterface // 排除静态类和抽象类与结构、接口
                    && t.IsSubclassOf(tJsonSerializerContext) == false // 排除 JsonSerializerContext 的子类
                    && t.IsSubclassOf(tJsonConverter) == false // 排除 JsonConverter 的子类
                    && (predicate == null || predicate(t))
                select t;
        return q;
    }

    internal static void Json<T>(JsonSerializerOptions opt, T it, List<Exception> exceptions, Func<T, Type> getType, Func<T, Exception, string>? getErrMsg = null)
    {
        var t = getType(it);
        try
        {
            var obj = CreateInstance(t, opt);
            Assert.That(obj, Is.Not.Null, $"创建 {t.FullName} 实例失败。");
            // 测试序列化
            var json = JsonSerializer.Serialize(obj, t, opt);
            Assert.That(json, Is.Not.Null.Or.Empty, $"序列化 {t.FullName} 失败。");
            // 测试反序列化
            var deserializedObj = JsonSerializer.Deserialize(json, t, opt);
            Assert.That(deserializedObj, Is.Not.Null, $"反序列化 {t.FullName} 失败。");
        }
        catch (Exception ex)
        {
            var errMsg = getErrMsg?.Invoke(it, ex) ??
$"""
测试类型 {t.FullName} 时发生异常。
    {ex}

""";
            exceptions.Add(new ApplicationException(errMsg));
        }
    }

    internal static bool IsModelType(Type t, [NotNullWhen(true)] out Type? modelType)
    {
        if (t == typeof(Task) || t == typeof(ValueTask) || t == typeof(CancellationToken))
        {
        }
        else
        {
            var typeCode = Type.GetTypeCode(t);
            switch (typeCode)
            {
                case TypeCode.Object:
                    {
                        if (t.IsGenericType)
                        {
                            var genericTypeDefinition = t.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(Task<>) || genericTypeDefinition == typeof(ValueTask<>))
                            {
                                return IsModelType(t.GetGenericArguments()[0], out modelType);
                            }
                        }
                        modelType = t;
                        return true;
                    }
            }
        }
        modelType = null;
        return false;
    }

    /// <summary>
    /// 判断类型是否为 ValueTuple
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    internal static bool IsValueTuple(Type t)
    {
        if (t.IsGenericType)
        {
            var genericTypeDefinition = t.GetGenericTypeDefinition();
            if (genericTypeDefinition.IsValueType)
            {
                if (typeof(ITuple).IsAssignableFrom(genericTypeDefinition))
                {
                    return true; // 是值元组类型
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 判断类型是否为 ValueTuple 的可空版本
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    internal static bool IsValueTupleNullable(Type t)
    {
        if (t.IsGenericType)
        {
            var genericTypeDefinition = t.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(Nullable<>))
            {
                return IsValueTuple(t.GenericTypeArguments[0]);
            }
        }
        return false;
    }

    /// <summary>
    /// 传入多个服务程序集，返回类型为模型类的类型
    /// </summary>
    /// <param name="namespaceStartsWith"></param>
    /// <param name="predicate"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    internal static IEnumerable<Type> GetModelTypesByServiceAssemblies(string? namespaceStartsWith = null, Func<Type, bool>? predicate = null, params IEnumerable<Assembly> assemblies)
    {
        var q = from t in assemblies.Select(static x => x.GetTypes()).SelectMany(static x => x)
                where t.Namespace != null && (namespaceStartsWith == null || t.Namespace.StartsWith(namespaceStartsWith)) // 命名空间过滤
                    && t.FullName != null && t.FullName.Contains("+<") == false // 排除源生成的类型
                    && t.IsInterface // 仅接口
                    && (predicate == null || predicate(t))
                select t;
        return q;
    }

    /// <summary>
    /// 传入多个服务程序集，返回类型为模型类的类型字典，键为模型类型，值为模型类型在服务接口中的位置（属性或方法参数或返回值）
    /// </summary>
    /// <param name="namespaceStartsWith"></param>
    /// <param name="predicate"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    internal static Dictionary<Type, string> GetModelTypeDictByServiceAssemblies(string? namespaceStartsWith = null, Func<Type, bool>? predicate = null, params IEnumerable<Assembly> assemblies)
    {
        var q = GetModelTypesByServiceAssemblies(namespaceStartsWith, predicate, assemblies);
        Dictionary<Type, string> types = new();
        Type[] serviceTypes = [.. q];
        foreach (var serviceType in serviceTypes)
        {
            var properties = serviceType.GetProperties().Select(static x => x.PropertyType);
            foreach (var p in properties)
            {
                if (!serviceTypes.Contains(p) && IsModelType(p, out var modelType) && !types.ContainsKey(modelType))
                {
                    types.Add(modelType, $"{serviceType.FullName}.{p.Name}");
                }
            }
            var methods = serviceType.GetMethods();
            foreach (var m in methods)
            {
                var parameterTypes = m.GetParameterTypes();
                if (parameterTypes != null)
                {
                    foreach (var a in parameterTypes)
                    {
                        if (!serviceTypes.Contains(a) && IsModelType(a, out var modelType) && !types.ContainsKey(modelType))
                        {
                            types.Add(modelType, $"{serviceType.FullName}.{m.Name}");
                        }
                    }
                }
                var returnType = m.ReturnType;
                if (returnType != null && returnType.FullName != voidTypeString)
                {
                    if (!serviceTypes.Contains(returnType) && IsModelType(returnType, out var modelType) && !types.ContainsKey(modelType))
                    {
                        types.Add(modelType, $"{serviceType.FullName}.{m.Name}");
                    }
                }
            }
        }
        return types;
    }

    internal static JsonSerializerOptions GetOptions(params IEnumerable<IJsonTypeInfoResolver> resolvers) => GetOptions(resolvers, true);

    internal static JsonSerializerOptions GetOptions(IEnumerable<IJsonTypeInfoResolver> resolvers, bool isReadOnly = true)
    {
        JsonSerializerOptions opt = new(JsonSerializerDefaults.Web)
        {
            Encoder = NewtonsoftJsonCompatibleEncoder.UnsafeNewtonsoftJsonCompatibleEncoder, // 不转义字符！！！
            AllowTrailingCommas = true,
        };
        opt.TypeInfoResolverChain.Clear();
        foreach (var resolver in resolvers)
        {
            opt.TypeInfoResolverChain.Add(resolver);
        }
        opt = Serializable.CreateOptions(opt, isReadOnly: isReadOnly);
        return opt;
    }
}

/// <summary>
/// JavaScript encoder built on top of <see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping"/> built to be compatible with
/// Newtonsoft.Json.
/// https://github.com/BeyondDimension/System.Text.Json.Extensions/blob/main/RamjotSingh.System.Text.Json.Extensions/Encoders/NewtonsoftJsonCompatibleEncoder.cs
/// </summary>
file sealed class NewtonsoftJsonCompatibleEncoder : JavaScriptEncoder
{
    /// <summary>
    /// Gets a JavaScript encoder instance that is compatible with Newtonsoft.Json's way of processing jsons. This is built on top of
    /// <see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping"/>.
    /// </summary>
    public static readonly JavaScriptEncoder UnsafeNewtonsoftJsonCompatibleEncoder = new NewtonsoftJsonCompatibleEncoder();

    private readonly JavaScriptEncoder defaultEncoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonCompatibleEncoder"/> class.
    /// </summary>
    private NewtonsoftJsonCompatibleEncoder()
    {
    }

    /// <summary>
    /// Gets the maximum number of characters that this encoder can generate for each input code point.
    /// </summary>
    public override int MaxOutputCharactersPerInputCharacter => this.defaultEncoder.MaxOutputCharactersPerInputCharacter;

    /// <summary>
    /// Finds the index of the first character to encode.
    /// </summary>
    /// <param name="text"> The text buffer to search.</param>
    /// <param name="textLength">The number of characters in text.</param>
    /// <returns>The index of the first character to encode.</returns>
    public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
    {
        ReadOnlySpan<char> input = new ReadOnlySpan<char>(text, textLength);
        int idx = 0;

        // Enumerate until we're out of data or saw invalid input
        while (Rune.DecodeFromUtf16(input[idx..], out Rune result, out int charsConsumed) == OperationStatus.Done)
        {
            if (this.WillEncode(result.Value))
            {
                // found a char that needs to be escaped
                break;
            }

            idx += charsConsumed;
        }

        if (idx == input.Length)
        {
            // walked entire input without finding a char which needs escaping
            return -1;
        }

        return idx;
    }

    /// <summary>
    /// Encodes a Unicode scalar value and writes it to a buffer.
    /// </summary>
    /// <param name="unicodeScalar">A Unicode scalar value.</param>
    /// <param name="buffer">A pointer to the buffer to which to write the encoded text.</param>
    /// <param name="bufferLength">The length of the destination buffer in characters.</param>
    /// <param name="numberOfCharactersWritten">When the method returns, indicates the number of characters written to the buffer.</param>
    /// <returns>false if bufferLength is too small to fit the encoded text; otherwise, returns true.</returns>
    public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
    {
        return this.defaultEncoder.TryEncodeUnicodeScalar(unicodeScalar, buffer, bufferLength, out numberOfCharactersWritten);
    }

    /// <summary>
    /// Determines if a given Unicode scalar value will be encoded.
    /// </summary>
    /// <param name="unicodeScalar">A Unicode scalar value.</param>
    /// <returns>true if the unicodeScalar value will be encoded by this encoder; otherwise, returns false.</returns>
    public override bool WillEncode(int unicodeScalar)
    {
        if (UnicodeScalarHelper.IsUnicodeScalarAnEmoji(unicodeScalar))
        {
            return false;
        }
        else
        {
            return this.defaultEncoder.WillEncode(unicodeScalar);
        }
    }
}

/// <summary>
/// Helpers for unicode scalars.
/// https://github.com/BeyondDimension/EmojiNet/blob/main/EmojiNet/RamjotSingh.EmojiNet/UnicodeScalarHelper.cs
/// </summary>
file static class UnicodeScalarHelper
{
    /// <summary>
    /// Detects if the passed unicode scalar corresponds to a well known emoji.
    /// </summary>
    /// <param name="unicodeScalar">Unicode scalar to check.</param>
    /// <returns>True if the passed unicode scalar is an emoji, false otherwise.</returns>
    public static bool IsUnicodeScalarAnEmoji(int unicodeScalar)
    {
        switch (unicodeScalar)
        {
            case 35:
            case 42:
            case 48:
            case 49:
            case 50:
            case 51:
            case 52:
            case 53:
            case 54:
            case 55:
            case 56:
            case 57:
            case 169:
            case 174:
            case 8205:
            case 8252:
            case 8265:
            case 8419:
            case 8482:
            case 8505:
            case 8596:
            case 8597:
            case 8598:
            case 8599:
            case 8600:
            case 8601:
            case 8617:
            case 8618:
            case 8986:
            case 8987:
            case 9000:
            case 9167:
            case 9193:
            case 9194:
            case 9195:
            case 9196:
            case 9197:
            case 9198:
            case 9199:
            case 9200:
            case 9201:
            case 9202:
            case 9203:
            case 9208:
            case 9209:
            case 9210:
            case 9410:
            case 9642:
            case 9643:
            case 9654:
            case 9664:
            case 9723:
            case 9724:
            case 9725:
            case 9726:
            case 9728:
            case 9729:
            case 9730:
            case 9731:
            case 9732:
            case 9742:
            case 9745:
            case 9748:
            case 9749:
            case 9752:
            case 9757:
            case 9760:
            case 9762:
            case 9763:
            case 9766:
            case 9770:
            case 9774:
            case 9775:
            case 9784:
            case 9785:
            case 9786:
            case 9792:
            case 9794:
            case 9800:
            case 9801:
            case 9802:
            case 9803:
            case 9804:
            case 9805:
            case 9806:
            case 9807:
            case 9808:
            case 9809:
            case 9810:
            case 9811:
            case 9823:
            case 9824:
            case 9827:
            case 9829:
            case 9830:
            case 9832:
            case 9851:
            case 9854:
            case 9855:
            case 9874:
            case 9875:
            case 9876:
            case 9877:
            case 9878:
            case 9879:
            case 9881:
            case 9883:
            case 9884:
            case 9888:
            case 9889:
            case 9895:
            case 9898:
            case 9899:
            case 9904:
            case 9905:
            case 9917:
            case 9918:
            case 9924:
            case 9925:
            case 9928:
            case 9934:
            case 9935:
            case 9937:
            case 9939:
            case 9940:
            case 9961:
            case 9962:
            case 9968:
            case 9969:
            case 9970:
            case 9971:
            case 9972:
            case 9973:
            case 9975:
            case 9976:
            case 9977:
            case 9978:
            case 9981:
            case 9986:
            case 9989:
            case 9992:
            case 9993:
            case 9994:
            case 9995:
            case 9996:
            case 9997:
            case 9999:
            case 10002:
            case 10004:
            case 10006:
            case 10013:
            case 10017:
            case 10024:
            case 10035:
            case 10036:
            case 10052:
            case 10055:
            case 10060:
            case 10062:
            case 10067:
            case 10068:
            case 10069:
            case 10071:
            case 10083:
            case 10084:
            case 10133:
            case 10134:
            case 10135:
            case 10145:
            case 10160:
            case 10175:
            case 10548:
            case 10549:
            case 11013:
            case 11014:
            case 11015:
            case 11035:
            case 11036:
            case 11088:
            case 11093:
            case 12336:
            case 12349:
            case 12951:
            case 12953:
            case 65039:
            case 126980:
            case 127183:
            case 127344:
            case 127345:
            case 127358:
            case 127359:
            case 127374:
            case 127377:
            case 127378:
            case 127379:
            case 127380:
            case 127381:
            case 127382:
            case 127383:
            case 127384:
            case 127385:
            case 127386:
            case 127462:
            case 127463:
            case 127464:
            case 127465:
            case 127466:
            case 127467:
            case 127468:
            case 127469:
            case 127470:
            case 127471:
            case 127472:
            case 127473:
            case 127474:
            case 127475:
            case 127476:
            case 127477:
            case 127478:
            case 127479:
            case 127480:
            case 127481:
            case 127482:
            case 127483:
            case 127484:
            case 127485:
            case 127486:
            case 127487:
            case 127489:
            case 127490:
            case 127514:
            case 127535:
            case 127538:
            case 127539:
            case 127540:
            case 127541:
            case 127542:
            case 127543:
            case 127544:
            case 127545:
            case 127546:
            case 127568:
            case 127569:
            case 127744:
            case 127745:
            case 127746:
            case 127747:
            case 127748:
            case 127749:
            case 127750:
            case 127751:
            case 127752:
            case 127753:
            case 127754:
            case 127755:
            case 127756:
            case 127757:
            case 127758:
            case 127759:
            case 127760:
            case 127761:
            case 127762:
            case 127763:
            case 127764:
            case 127765:
            case 127766:
            case 127767:
            case 127768:
            case 127769:
            case 127770:
            case 127771:
            case 127772:
            case 127773:
            case 127774:
            case 127775:
            case 127776:
            case 127777:
            case 127780:
            case 127781:
            case 127782:
            case 127783:
            case 127784:
            case 127785:
            case 127786:
            case 127787:
            case 127788:
            case 127789:
            case 127790:
            case 127791:
            case 127792:
            case 127793:
            case 127794:
            case 127795:
            case 127796:
            case 127797:
            case 127798:
            case 127799:
            case 127800:
            case 127801:
            case 127802:
            case 127803:
            case 127804:
            case 127805:
            case 127806:
            case 127807:
            case 127808:
            case 127809:
            case 127810:
            case 127811:
            case 127812:
            case 127813:
            case 127814:
            case 127815:
            case 127816:
            case 127817:
            case 127818:
            case 127819:
            case 127820:
            case 127821:
            case 127822:
            case 127823:
            case 127824:
            case 127825:
            case 127826:
            case 127827:
            case 127828:
            case 127829:
            case 127830:
            case 127831:
            case 127832:
            case 127833:
            case 127834:
            case 127835:
            case 127836:
            case 127837:
            case 127838:
            case 127839:
            case 127840:
            case 127841:
            case 127842:
            case 127843:
            case 127844:
            case 127845:
            case 127846:
            case 127847:
            case 127848:
            case 127849:
            case 127850:
            case 127851:
            case 127852:
            case 127853:
            case 127854:
            case 127855:
            case 127856:
            case 127857:
            case 127858:
            case 127859:
            case 127860:
            case 127861:
            case 127862:
            case 127863:
            case 127864:
            case 127865:
            case 127866:
            case 127867:
            case 127868:
            case 127869:
            case 127870:
            case 127871:
            case 127872:
            case 127873:
            case 127874:
            case 127875:
            case 127876:
            case 127877:
            case 127878:
            case 127879:
            case 127880:
            case 127881:
            case 127882:
            case 127883:
            case 127884:
            case 127885:
            case 127886:
            case 127887:
            case 127888:
            case 127889:
            case 127890:
            case 127891:
            case 127894:
            case 127895:
            case 127897:
            case 127898:
            case 127899:
            case 127902:
            case 127903:
            case 127904:
            case 127905:
            case 127906:
            case 127907:
            case 127908:
            case 127909:
            case 127910:
            case 127911:
            case 127912:
            case 127913:
            case 127914:
            case 127915:
            case 127916:
            case 127917:
            case 127918:
            case 127919:
            case 127920:
            case 127921:
            case 127922:
            case 127923:
            case 127924:
            case 127925:
            case 127926:
            case 127927:
            case 127928:
            case 127929:
            case 127930:
            case 127931:
            case 127932:
            case 127933:
            case 127934:
            case 127935:
            case 127936:
            case 127937:
            case 127938:
            case 127939:
            case 127940:
            case 127941:
            case 127942:
            case 127943:
            case 127944:
            case 127945:
            case 127946:
            case 127947:
            case 127948:
            case 127949:
            case 127950:
            case 127951:
            case 127952:
            case 127953:
            case 127954:
            case 127955:
            case 127956:
            case 127957:
            case 127958:
            case 127959:
            case 127960:
            case 127961:
            case 127962:
            case 127963:
            case 127964:
            case 127965:
            case 127966:
            case 127967:
            case 127968:
            case 127969:
            case 127970:
            case 127971:
            case 127972:
            case 127973:
            case 127974:
            case 127975:
            case 127976:
            case 127977:
            case 127978:
            case 127979:
            case 127980:
            case 127981:
            case 127982:
            case 127983:
            case 127984:
            case 127987:
            case 127988:
            case 127989:
            case 127991:
            case 127992:
            case 127993:
            case 127994:
            case 128000:
            case 128001:
            case 128002:
            case 128003:
            case 128004:
            case 128005:
            case 128006:
            case 128007:
            case 128008:
            case 128009:
            case 128010:
            case 128011:
            case 128012:
            case 128013:
            case 128014:
            case 128015:
            case 128016:
            case 128017:
            case 128018:
            case 128019:
            case 128020:
            case 128021:
            case 128022:
            case 128023:
            case 128024:
            case 128025:
            case 128026:
            case 128027:
            case 128028:
            case 128029:
            case 128030:
            case 128031:
            case 128032:
            case 128033:
            case 128034:
            case 128035:
            case 128036:
            case 128037:
            case 128038:
            case 128039:
            case 128040:
            case 128041:
            case 128042:
            case 128043:
            case 128044:
            case 128045:
            case 128046:
            case 128047:
            case 128048:
            case 128049:
            case 128050:
            case 128051:
            case 128052:
            case 128053:
            case 128054:
            case 128055:
            case 128056:
            case 128057:
            case 128058:
            case 128059:
            case 128060:
            case 128061:
            case 128062:
            case 128063:
            case 128064:
            case 128065:
            case 128066:
            case 128067:
            case 128068:
            case 128069:
            case 128070:
            case 128071:
            case 128072:
            case 128073:
            case 128074:
            case 128075:
            case 128076:
            case 128077:
            case 128078:
            case 128079:
            case 128080:
            case 128081:
            case 128082:
            case 128083:
            case 128084:
            case 128085:
            case 128086:
            case 128087:
            case 128088:
            case 128089:
            case 128090:
            case 128091:
            case 128092:
            case 128093:
            case 128094:
            case 128095:
            case 128096:
            case 128097:
            case 128098:
            case 128099:
            case 128100:
            case 128101:
            case 128102:
            case 128103:
            case 128104:
            case 128105:
            case 128106:
            case 128107:
            case 128108:
            case 128109:
            case 128110:
            case 128111:
            case 128112:
            case 128113:
            case 128114:
            case 128115:
            case 128116:
            case 128117:
            case 128118:
            case 128119:
            case 128120:
            case 128121:
            case 128122:
            case 128123:
            case 128124:
            case 128125:
            case 128126:
            case 128127:
            case 128128:
            case 128129:
            case 128130:
            case 128131:
            case 128132:
            case 128133:
            case 128134:
            case 128135:
            case 128136:
            case 128137:
            case 128138:
            case 128139:
            case 128140:
            case 128141:
            case 128142:
            case 128143:
            case 128144:
            case 128145:
            case 128146:
            case 128147:
            case 128148:
            case 128149:
            case 128150:
            case 128151:
            case 128152:
            case 128153:
            case 128154:
            case 128155:
            case 128156:
            case 128157:
            case 128158:
            case 128159:
            case 128160:
            case 128161:
            case 128162:
            case 128163:
            case 128164:
            case 128165:
            case 128166:
            case 128167:
            case 128168:
            case 128169:
            case 128170:
            case 128171:
            case 128172:
            case 128173:
            case 128174:
            case 128175:
            case 128176:
            case 128177:
            case 128178:
            case 128179:
            case 128180:
            case 128181:
            case 128182:
            case 128183:
            case 128184:
            case 128185:
            case 128186:
            case 128187:
            case 128188:
            case 128189:
            case 128190:
            case 128191:
            case 128192:
            case 128193:
            case 128194:
            case 128195:
            case 128196:
            case 128197:
            case 128198:
            case 128199:
            case 128200:
            case 128201:
            case 128202:
            case 128203:
            case 128204:
            case 128205:
            case 128206:
            case 128207:
            case 128208:
            case 128209:
            case 128210:
            case 128211:
            case 128212:
            case 128213:
            case 128214:
            case 128215:
            case 128216:
            case 128217:
            case 128218:
            case 128219:
            case 128220:
            case 128221:
            case 128222:
            case 128223:
            case 128224:
            case 128225:
            case 128226:
            case 128227:
            case 128228:
            case 128229:
            case 128230:
            case 128231:
            case 128232:
            case 128233:
            case 128234:
            case 128235:
            case 128236:
            case 128237:
            case 128238:
            case 128239:
            case 128240:
            case 128241:
            case 128242:
            case 128243:
            case 128244:
            case 128245:
            case 128246:
            case 128247:
            case 128248:
            case 128249:
            case 128250:
            case 128251:
            case 128252:
            case 128253:
            case 128255:
            case 128256:
            case 128257:
            case 128258:
            case 128259:
            case 128260:
            case 128261:
            case 128262:
            case 128263:
            case 128264:
            case 128265:
            case 128266:
            case 128267:
            case 128268:
            case 128269:
            case 128270:
            case 128271:
            case 128272:
            case 128273:
            case 128274:
            case 128275:
            case 128276:
            case 128277:
            case 128278:
            case 128279:
            case 128280:
            case 128281:
            case 128282:
            case 128283:
            case 128284:
            case 128285:
            case 128286:
            case 128287:
            case 128288:
            case 128289:
            case 128290:
            case 128291:
            case 128292:
            case 128293:
            case 128294:
            case 128295:
            case 128296:
            case 128297:
            case 128298:
            case 128299:
            case 128300:
            case 128301:
            case 128302:
            case 128303:
            case 128304:
            case 128305:
            case 128306:
            case 128307:
            case 128308:
            case 128309:
            case 128310:
            case 128311:
            case 128312:
            case 128313:
            case 128314:
            case 128315:
            case 128316:
            case 128317:
            case 128329:
            case 128330:
            case 128331:
            case 128332:
            case 128333:
            case 128334:
            case 128336:
            case 128337:
            case 128338:
            case 128339:
            case 128340:
            case 128341:
            case 128342:
            case 128343:
            case 128344:
            case 128345:
            case 128346:
            case 128347:
            case 128348:
            case 128349:
            case 128350:
            case 128351:
            case 128352:
            case 128353:
            case 128354:
            case 128355:
            case 128356:
            case 128357:
            case 128358:
            case 128359:
            case 128367:
            case 128368:
            case 128371:
            case 128372:
            case 128373:
            case 128374:
            case 128375:
            case 128376:
            case 128377:
            case 128378:
            case 128391:
            case 128394:
            case 128395:
            case 128396:
            case 128397:
            case 128400:
            case 128405:
            case 128406:
            case 128420:
            case 128421:
            case 128424:
            case 128433:
            case 128434:
            case 128444:
            case 128450:
            case 128451:
            case 128452:
            case 128465:
            case 128466:
            case 128467:
            case 128476:
            case 128477:
            case 128478:
            case 128481:
            case 128483:
            case 128488:
            case 128495:
            case 128499:
            case 128506:
            case 128507:
            case 128508:
            case 128509:
            case 128510:
            case 128511:
            case 128512:
            case 128513:
            case 128514:
            case 128515:
            case 128516:
            case 128517:
            case 128518:
            case 128519:
            case 128520:
            case 128521:
            case 128522:
            case 128523:
            case 128524:
            case 128525:
            case 128526:
            case 128527:
            case 128528:
            case 128529:
            case 128530:
            case 128531:
            case 128532:
            case 128533:
            case 128534:
            case 128535:
            case 128536:
            case 128537:
            case 128538:
            case 128539:
            case 128540:
            case 128541:
            case 128542:
            case 128543:
            case 128544:
            case 128545:
            case 128546:
            case 128547:
            case 128548:
            case 128549:
            case 128550:
            case 128551:
            case 128552:
            case 128553:
            case 128554:
            case 128555:
            case 128556:
            case 128557:
            case 128558:
            case 128559:
            case 128560:
            case 128561:
            case 128562:
            case 128563:
            case 128564:
            case 128565:
            case 128566:
            case 128567:
            case 128568:
            case 128569:
            case 128570:
            case 128571:
            case 128572:
            case 128573:
            case 128574:
            case 128575:
            case 128576:
            case 128577:
            case 128578:
            case 128579:
            case 128580:
            case 128581:
            case 128582:
            case 128583:
            case 128584:
            case 128585:
            case 128586:
            case 128587:
            case 128588:
            case 128589:
            case 128590:
            case 128591:
            case 128640:
            case 128641:
            case 128642:
            case 128643:
            case 128644:
            case 128645:
            case 128646:
            case 128647:
            case 128648:
            case 128649:
            case 128650:
            case 128651:
            case 128652:
            case 128653:
            case 128654:
            case 128655:
            case 128656:
            case 128657:
            case 128658:
            case 128659:
            case 128660:
            case 128661:
            case 128662:
            case 128663:
            case 128664:
            case 128665:
            case 128666:
            case 128667:
            case 128668:
            case 128669:
            case 128670:
            case 128671:
            case 128672:
            case 128673:
            case 128674:
            case 128675:
            case 128676:
            case 128677:
            case 128678:
            case 128679:
            case 128680:
            case 128681:
            case 128682:
            case 128683:
            case 128684:
            case 128685:
            case 128686:
            case 128687:
            case 128688:
            case 128689:
            case 128690:
            case 128691:
            case 128692:
            case 128693:
            case 128694:
            case 128695:
            case 128696:
            case 128697:
            case 128698:
            case 128699:
            case 128700:
            case 128701:
            case 128702:
            case 128703:
            case 128704:
            case 128705:
            case 128706:
            case 128707:
            case 128708:
            case 128709:
            case 128715:
            case 128716:
            case 128717:
            case 128718:
            case 128719:
            case 128720:
            case 128721:
            case 128722:
            case 128725:
            case 128726:
            case 128727:
            case 128736:
            case 128737:
            case 128738:
            case 128739:
            case 128740:
            case 128741:
            case 128745:
            case 128747:
            case 128748:
            case 128752:
            case 128755:
            case 128756:
            case 128757:
            case 128758:
            case 128759:
            case 128760:
            case 128761:
            case 128762:
            case 128763:
            case 128764:
            case 128992:
            case 128993:
            case 128994:
            case 128995:
            case 128996:
            case 128997:
            case 128998:
            case 128999:
            case 129000:
            case 129001:
            case 129002:
            case 129003:
            case 129292:
            case 129293:
            case 129294:
            case 129295:
            case 129296:
            case 129297:
            case 129298:
            case 129299:
            case 129300:
            case 129301:
            case 129302:
            case 129303:
            case 129304:
            case 129305:
            case 129306:
            case 129307:
            case 129308:
            case 129309:
            case 129310:
            case 129311:
            case 129312:
            case 129313:
            case 129314:
            case 129315:
            case 129316:
            case 129317:
            case 129318:
            case 129319:
            case 129320:
            case 129321:
            case 129322:
            case 129323:
            case 129324:
            case 129325:
            case 129326:
            case 129327:
            case 129328:
            case 129329:
            case 129330:
            case 129331:
            case 129332:
            case 129333:
            case 129334:
            case 129335:
            case 129336:
            case 129337:
            case 129338:
            case 129340:
            case 129341:
            case 129342:
            case 129343:
            case 129344:
            case 129345:
            case 129346:
            case 129347:
            case 129348:
            case 129349:
            case 129351:
            case 129352:
            case 129353:
            case 129354:
            case 129355:
            case 129356:
            case 129357:
            case 129358:
            case 129359:
            case 129360:
            case 129361:
            case 129362:
            case 129363:
            case 129364:
            case 129365:
            case 129366:
            case 129367:
            case 129368:
            case 129369:
            case 129370:
            case 129371:
            case 129372:
            case 129373:
            case 129374:
            case 129375:
            case 129376:
            case 129377:
            case 129378:
            case 129379:
            case 129380:
            case 129381:
            case 129382:
            case 129383:
            case 129384:
            case 129385:
            case 129386:
            case 129387:
            case 129388:
            case 129389:
            case 129390:
            case 129391:
            case 129392:
            case 129393:
            case 129394:
            case 129395:
            case 129396:
            case 129397:
            case 129398:
            case 129399:
            case 129400:
            case 129402:
            case 129403:
            case 129404:
            case 129405:
            case 129406:
            case 129407:
            case 129408:
            case 129409:
            case 129410:
            case 129411:
            case 129412:
            case 129413:
            case 129414:
            case 129415:
            case 129416:
            case 129417:
            case 129418:
            case 129419:
            case 129420:
            case 129421:
            case 129422:
            case 129423:
            case 129424:
            case 129425:
            case 129426:
            case 129427:
            case 129428:
            case 129429:
            case 129430:
            case 129431:
            case 129432:
            case 129433:
            case 129434:
            case 129435:
            case 129436:
            case 129437:
            case 129438:
            case 129439:
            case 129440:
            case 129441:
            case 129442:
            case 129443:
            case 129444:
            case 129445:
            case 129446:
            case 129447:
            case 129448:
            case 129449:
            case 129450:
            case 129451:
            case 129452:
            case 129453:
            case 129454:
            case 129455:
            case 129456:
            case 129457:
            case 129458:
            case 129459:
            case 129460:
            case 129461:
            case 129462:
            case 129463:
            case 129464:
            case 129465:
            case 129466:
            case 129467:
            case 129468:
            case 129469:
            case 129470:
            case 129471:
            case 129472:
            case 129473:
            case 129474:
            case 129475:
            case 129476:
            case 129477:
            case 129478:
            case 129479:
            case 129480:
            case 129481:
            case 129482:
            case 129483:
            case 129485:
            case 129486:
            case 129487:
            case 129488:
            case 129489:
            case 129490:
            case 129491:
            case 129492:
            case 129493:
            case 129494:
            case 129495:
            case 129496:
            case 129497:
            case 129498:
            case 129499:
            case 129500:
            case 129501:
            case 129502:
            case 129503:
            case 129504:
            case 129505:
            case 129506:
            case 129507:
            case 129508:
            case 129509:
            case 129510:
            case 129511:
            case 129512:
            case 129513:
            case 129514:
            case 129515:
            case 129516:
            case 129517:
            case 129518:
            case 129519:
            case 129520:
            case 129521:
            case 129522:
            case 129523:
            case 129524:
            case 129525:
            case 129526:
            case 129527:
            case 129528:
            case 129529:
            case 129530:
            case 129531:
            case 129532:
            case 129533:
            case 129534:
            case 129535:
            case 129648:
            case 129649:
            case 129650:
            case 129651:
            case 129652:
            case 129656:
            case 129657:
            case 129658:
            case 129664:
            case 129665:
            case 129666:
            case 129667:
            case 129668:
            case 129669:
            case 129670:
            case 129680:
            case 129681:
            case 129682:
            case 129683:
            case 129684:
            case 129685:
            case 129686:
            case 129687:
            case 129688:
            case 129689:
            case 129690:
            case 129691:
            case 129692:
            case 129693:
            case 129694:
            case 129695:
            case 129696:
            case 129697:
            case 129698:
            case 129699:
            case 129700:
            case 129701:
            case 129702:
            case 129703:
            case 129704:
            case 129712:
            case 129713:
            case 129714:
            case 129715:
            case 129716:
            case 129717:
            case 129718:
            case 129728:
            case 129729:
            case 129730:
            case 129744:
            case 129745:
            case 129746:
            case 129747:
            case 129748:
            case 129749:
            case 129750:
            case 917602:
            case 917603:
            case 917605:
            case 917607:
            case 917612:
            case 917614:
            case 917619:
            case 917620:
            case 917623:
            case 917631:
                return true;

            default:
                return false;
        }
    }
}