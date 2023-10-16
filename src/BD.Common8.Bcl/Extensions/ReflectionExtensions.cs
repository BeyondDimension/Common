namespace System.Extensions;

/// <summary>
/// 提供对 Reflection（反射）的扩展函数
/// </summary>
public static partial class ReflectionExtensions
{
    /// <summary>
    /// 检索应用于指定程序集的指定类型的自定义特性，如果找不到将引发 <see cref="ArgumentNullException"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRequiredCustomAttribute<T>(this Assembly assembly) where T : Attribute
    {
        var requiredCustomAttribute = assembly.GetCustomAttribute<T>();
        requiredCustomAttribute.ThrowIsNull();
        return requiredCustomAttribute;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsNullableType(this Type t)
    {
        return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// 类型是否为 <see cref="Nullable{T}"/> 可空类型
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullable(this Type t)
    {
        if (t.IsValueType) // 值类型(struct)进行可空判断
            return t.IsNullableType();
        return true; // 引用类型(class)都可空
    }

    /// <summary>
    /// 判断类型是否为静态类
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStatic(this Type t) => t.IsAbstract && t.IsSealed;

    /// <summary>
    /// 获取类型的 <see cref="TypeCode"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeCode GetTypeCode(this Type type) => Type.GetTypeCode(type);
}