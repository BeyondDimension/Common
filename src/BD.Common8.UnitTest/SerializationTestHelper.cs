using DotNext.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    /// <summary>
    /// 根据模型类型创建实例
    /// </summary>
    internal static object CreateInstance(Type t, JsonSerializerContext jsc)
    {
        if (t.FullName == voidTypeString)
        {
            return null!;
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
                        return CreateInstance(typeof(List<>).MakeGenericType(t.GenericTypeArguments[0]), jsc);
                    }
                    else if (gTypeDef == typeof(IDictionary<,>) || gTypeDef == typeof(IReadOnlyDictionary<,>))
                    {
                        return CreateInstance(typeof(Dictionary<,>).MakeGenericType(t.GenericTypeArguments[0], t.GenericTypeArguments[1]), jsc);
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
                var obj = JsonSerializer.Deserialize(jsonObjectString, t, jsc);
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

    internal static void Json<T>(JsonSerializerContext jsc, T it, List<Exception> exceptions, Func<T, Type> getType, Func<T, Exception, string>? getErrMsg = null)
    {
        var t = getType(it);
        try
        {
            var obj = CreateInstance(t, jsc);
            Assert.That(obj, Is.Not.Null, $"创建 {t.FullName} 实例失败。");
            // 测试序列化
            var json = JsonSerializer.Serialize(obj, t, jsc);
            Assert.That(json, Is.Not.Null.Or.Empty, $"序列化 {t.FullName} 失败。");
            // 测试反序列化
            var deserializedObj = JsonSerializer.Deserialize(json, t, jsc);
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
}
