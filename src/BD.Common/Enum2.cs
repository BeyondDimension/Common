namespace System;

public static partial class Enum2
{
    /// <summary>
    /// 获取枚举定义的所有常量
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum[] GetAll<TEnum>() where TEnum : struct, Enum =>
#if !NET5_0_OR_GREATER
        (TEnum[])Enum.GetValues(typeof(TEnum));
#else
        Enum.GetValues<TEnum>();
#endif

    [Obsolete("use Enum.GetValues(Type)", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("It might not be possible to create an array of the enum type at runtime. Use the GetValues<TEnum> overload or the GetValuesAsUnderlyingType method instead.")]
    public static Array GetAll(Type enumType) => Enum.GetValues(enumType);

    /// <summary>
    /// 获取枚举定义的所有常量名字符串
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] GetAllStrings<TEnum>()
        where TEnum : struct, Enum
        => GetAll<TEnum>().Select(x => x.ToString()).ToArray();

    /// <inheritdoc cref="GetAllStrings{TEnum}()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("It might not be possible to create an array of the enum type at runtime. Use the GetValues<TEnum> overload or the GetValuesAsUnderlyingType method instead.")]
    public static string[] GetAllStrings(Type enumType)
    {
        return GetAllStringsCore(enumType).ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<string> GetAllStringsCore(Type enumType)
        {
            var array = Enum.GetValues(enumType);
            foreach (var item in array)
            {
                yield return item.ToString()!;
            }
        }
    }

    /// <summary>
    /// 将 Flags 枚举进行拆分
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TEnum> FlagsSplit<TEnum>(TEnum value)
        where TEnum : struct, Enum
        => GetAll<TEnum>().Where(x => value.HasFlag(x)).ToArray();

    /// <inheritdoc cref="FlagsSplit{TEnum}(TEnum)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("It might not be possible to create an array of the enum type at runtime. Use the GetValues<TEnum> overload or the GetValuesAsUnderlyingType method instead.")]
    public static IEnumerable<Enum> FlagsSplit(Enum value)
    {
        var enumType = value.GetType();
        var all = Enum.GetValues(enumType);
        foreach (Enum item in all)
        {
            if (value.HasFlag(item))
            {
                yield return item;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToInt32<TEnum>(TEnum value)
        where TEnum : struct, Enum
        => ConvertibleHelper.Convert<int, TEnum>(value);

    /// <summary>
    /// 返回指定枚举值的描述（通过
    /// <see cref="DescriptionAttribute"/> 指定）
    /// 如果没有指定描述，则返回枚举常数的名称，没有找到枚举常数则返回枚举值
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value">要获取描述的枚举值</param>
    /// <returns>指定枚举值的描述</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetDescription<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        // 获取枚举常数名称
        var name = Enum.GetName(value);
        if (name != null)
        {
            // 获取枚举字段
            var enumType = value.GetType();
            var fieldInfo = enumType.GetField(name);
            if (fieldInfo != null)
            {
                if (Attribute.GetCustomAttribute(fieldInfo,
                    typeof(DescriptionAttribute), false) is DescriptionAttribute description)
                {
                    return description.Description;
                }
            }
        }
        return null;
    }

    /// <inheritdoc cref="GetDescription{TEnum}(TEnum)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetDescription(this Enum value)
    {
        var enumType = value.GetType();
        // 获取枚举常数名称
        var name = Enum.GetName(enumType, value);
        if (name != null)
        {
            // 获取枚举字段
            var fieldInfo = enumType.GetField(name);
            if (fieldInfo != null)
            {
                if (Attribute.GetCustomAttribute(fieldInfo,
                    typeof(DescriptionAttribute), false) is DescriptionAttribute description)
                {
                    return description.Description;
                }
            }
        }
        return null;
    }
}