namespace BD.Common.Models;

/// <summary>
/// 用于 Select/ComboBox 的数据源模型
/// </summary>
#if !BLAZOR
[MPObj]
#endif
public class SelectItemDTO : ITitle, IDisable
{
#if !BLAZOR
    [MPKey(0)]
#endif
    public string Title { get; set; } = "";

#if !BLAZOR
    [MPKey(LastMKeyIndex)]
#endif
    public bool Disable { get; set; }

    public const int Count = 100;

#if !BLAZOR
    /// <summary>
    /// 最后一个 MessagePack 序列化 下标，继承自此类，新增需要序列化的字段/属性，标记此值+1，+2
    /// </summary>
    protected const int LastMKeyIndex = 1;
#endif
}

/// <summary>
/// 用于 Select/ComboBox 的数据源模型
/// </summary>
/// <typeparam name="T"></typeparam>
#if !BLAZOR
[MPObj]
#endif
public class SelectItemDTO<T> : SelectItemDTO
{
#if !BLAZOR
    [MPKey(LastMKeyIndex + 1)]
#endif
    public T? Id { get; set; }

#if BLAZOR
    public static readonly SelectItemDTO<T> Null = new()
    {
        Id = default,
        Title = SharedStrings.All,
    };

    public string ToDisableStatus()
    {
        var equalityComparer = EqualityComparer<T>.Default;
        if (equalityComparer.Equals(Id, default) || (IsEnumOrInteger(typeof(T)) && equalityComparer.Equals(Id, ConvertibleHelper.Convert<T, sbyte>(sbyte.MinValue)))) return "default";
        if (Disable) return "error";
        return "success";

        static bool IsEnumOrInteger(Type type) => type.IsEnum || Type.GetTypeCode(type) switch
        {
            TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 => true,
            _ => false,
        };
    }
#endif
}
