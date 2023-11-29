namespace BD.Common8.Models;

/// <summary>
/// 用于 Select/ComboBox 的数据源模型
/// </summary>
[MPObj]
[MP2Obj(MP2SerializeLayout.Explicit)]
public partial class SelectItemDTO : ITitle, IDisable
{
    /// <summary>
    /// 标题
    /// </summary>
    [MPKey(0)]
    [MP2Key(0)]
    public string Title { get; set; } = "";

    /// <summary>
    /// 是否禁用
    /// </summary>
    [MPKey(LastMKeyIndex)]
    [MP2Key(LastMKeyIndex)]
    public bool Disable { get; set; }

    /// <summary>
    /// 数据源的数量
    /// </summary>
    public const int Count = 100;

    /// <summary>
    /// 最后一个 MessagePack 序列化 下标，继承自此类，新增需要序列化的字段/属性，标记此值+1，+2
    /// </summary>
    protected const int LastMKeyIndex = 1;
}

/// <summary>
/// 用于 Select/ComboBox 的数据源模型
/// </summary>
/// <typeparam name="T"></typeparam>
[MPObj]
[MP2Obj(MP2SerializeLayout.Explicit)]
public partial class SelectItemDTO<T> : SelectItemDTO
{
    /// <summary>
    /// 数据源项的 Id
    /// </summary>
    [MPKey(LastMKeyIndex + 1)]
    [MP2Key(LastMKeyIndex + 1)]
    public T? Id { get; set; }

    //public static readonly SelectItemDTO<T> Null = new()
    //{
    //    Id = default,
    //    Title = SharedStrings.All,
    //};

    //public string ToDisableStatus()
    //{
    //    var equalityComparer = EqualityComparer<T>.Default;
    //    if (equalityComparer.Equals(Id, default) || (IsEnumOrInteger(typeof(T)) && equalityComparer.Equals(Id, ConvertibleHelper.Convert<T, sbyte>(sbyte.MinValue)))) return "default";
    //    if (Disable) return "error";
    //    return "success";

    //    static bool IsEnumOrInteger(Type type) => type.IsEnum || Type.GetTypeCode(type) switch
    //    {
    //        TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 => true,
    //        _ => false,
    //    };
    //}
}
